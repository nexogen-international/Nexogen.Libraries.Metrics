using Nexogen.Libraries.Metrics.Prometheus;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Nexogen.Libraries.Metrics.Prometheus.PushGateway
{
    /// <summary>
    /// Pushgateway exporter for Prometheus.
    /// </summary>
    public class PushGateway : IDisposable
    {
        private static readonly Regex validLabel = new Regex("[a-zA-Z_:][a-zA-Z0-9_:]*");
        private readonly HttpClient client;
        private readonly string instance;
        private readonly IEnumerable<Tuple<string, string>> defaultLabels;

        /// <summary>
        /// Create a new PushGateway exporter.
        /// </summary>
        /// <param name="endpoint">The URL of the pushgateway</param>
        /// <param name="instance">The name of the individual source of metrics, usually corresponding to a single process</param>
        /// <param name="defaultLabels">Labels which are attached to all metrics exposed on this instance</param>
        public PushGateway(Uri endpoint,
            string instance = null, 
            IEnumerable<Tuple<string, string>> defaultLabels = null
        )
        {
            this.client = new HttpClient {BaseAddress = endpoint ?? throw new ArgumentNullException(nameof(endpoint))};
            this.instance = instance;
            this.defaultLabels = defaultLabels;
        }

        /// <summary>
        /// Create a new PushGateway exporter.
        /// </summary>
        /// <param name="client">The client used to connect to the pushgwateway</param>
        public PushGateway(HttpClient client)
        {
            this.client = client ?? throw new ArgumentNullException(nameof(client));
        }

        /// <summary>
        /// Dispose the pushgateway with its client.
        /// </summary>
        public void Dispose()
        {
            client.Dispose();
        }

        /// <summary>
        /// Pushes a metric to the push gateway.
        /// </summary>
        /// <param name="metrics">The metric to push.</param>
        /// <param name="job">The job name used by prometheus, this should be a unique name for the type of service. Must be a valid Prometheus label.</param>
        public void Push(IExposable metrics, string job)
        {
            Validate(metrics, job);

            // It would be better to have a buffered stream
            using (var ms = new MemoryStream())
            {
                metrics.Expose(ms, ExposeOptions.NoTimestamp).ConfigureAwait(false).GetAwaiter().GetResult();

                ms.Position = 0;
                var response = client.SendAsync(CreateRequestMessage(metrics, job, ms)).ConfigureAwait(false).GetAwaiter().GetResult();
                if (!response.IsSuccessStatusCode)
                {
                    throw new PushGatewayException($"Error pushing to gateway: {response.StatusCode}: {response.ReasonPhrase}");
                }
            }
        }

        /// <summary>
        /// Pushes a metric to the push gateway asynchronously .
        /// </summary>
        /// <param name="metrics">The metric to push.</param>
        /// <param name="job">The job name used by prometheus, this should be a unique name for the type of service. Must be a valid Prometheus label.</param>
        /// <returns>An awaitable Task</returns>
        public async Task PushAsync(IExposable metrics, string job)
        {
            Validate(metrics, job);

            // It would be better to have a buffered stream
            using (var ms = new MemoryStream())
            {
                await metrics.Expose(ms, ExposeOptions.NoTimestamp);

                ms.Position = 0;

                var response = await client.SendAsync(CreateRequestMessage(metrics, job, ms));
                if (!response.IsSuccessStatusCode)
                {
                    var message = await response.Content.ReadAsStringAsync();
                    throw new PushGatewayException($"Error pushing to gateway: {response.StatusCode}: {message}");
                }
            }
        }

        private static void Validate(IExposable metrics, string job)
        {
            if (metrics == null)
            {
                throw new ArgumentNullException(nameof(metrics));
            }

            if (job == null)
            {
                throw new ArgumentNullException(nameof(job));
            }

            if (!validLabel.IsMatch(job))
            {
                throw new ArgumentException("Job name must be a valid Prometheus label");
            }
        }

        private HttpRequestMessage CreateRequestMessage(IExposable metrics, string job, Stream stream)
        {
            var path = string.Format("metrics/job/{0}", WebUtility.UrlEncode(job));
            
            if (!string.IsNullOrEmpty(instance))
            {
                path = string.Format("{0}/instance/{1}", path, WebUtility.UrlEncode(instance));
            }
            
            if (defaultLabels != null)
            {
                foreach (var pair in defaultLabels)
                {
                    if (string.IsNullOrEmpty(pair?.Item1) || string.IsNullOrEmpty(pair.Item2))
                    {
                        continue;
                    }
                    path = string.Format("{0}/{1}/{2}", path, WebUtility.UrlEncode(pair.Item1), WebUtility.UrlEncode(pair.Item2));
                }
            }

            var request = new HttpRequestMessage(HttpMethod.Post, path);
            request.Content = new StreamContent(stream);
            request.Content.Headers.TryAddWithoutValidation("Content-Type", "text/plain; version=0.0.4");

            return request;
        }
    }
}
