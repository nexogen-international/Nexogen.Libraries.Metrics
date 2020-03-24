using System;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Nexogen.Libraries.Metrics.Prometheus;

namespace Nexogen.Libraries.Metrics.Prometheus.Standalone
{
    /// <summary>
    /// A simple Prometheus metric server using an embedded HTTP server.
    /// </summary>
    public class PrometheusServer : IHostedService
    {
        private readonly string prefix;
        private readonly HttpListener listener;
        private readonly IExposable metrics;
        private readonly ILogger<PrometheusServer> logger;

        public PrometheusServer(IOptions<PrometheusServerOptions> options, IExposable metrics, ILogger<PrometheusServer> logger)
        {
            this.prefix = $"http://*:{options.Value.Port}/";
            this.listener = new HttpListener {Prefixes = {prefix}};
            this.metrics = metrics;
            this.logger = logger;
        }

        public Task StartAsync(CancellationToken cancellationToken)
        {
            try
            {
                listener.Start();
            }
            catch (HttpListenerException ex) when (Environment.OSVersion.Platform == PlatformID.Win32NT)
            {
                logger.LogWarning(ex, "Unable to expose metrics. Please run the following command as admin and then try again:\nnetsh http add urlacl {0} user={1}\\{2}",
                    prefix, Environment.GetEnvironmentVariable("USERDOMAIN"), Environment.GetEnvironmentVariable("USERNAME"));
                return Task.CompletedTask;
            }

            BeginContext();

            return Task.CompletedTask;
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            listener.Abort();

            return Task.CompletedTask;
        }

        private void BeginContext()
        {
            try
            {
                listener.BeginGetContext(ListenerCallback, listener);
            }
            catch (ObjectDisposedException)
            {
                // Do not throw exception on shutdown
            }
        }

        private async void ListenerCallback(IAsyncResult result)
        {
            try
            {
                var context = listener.EndGetContext(result);
                if (context.Request.HttpMethod == "GET")
                {
                    context.Response.StatusCode = 200;
                    context.Response.Headers.Add("Content-Type", "text/plain; version=0.0.4; charset=utf-8");
                    await metrics.Expose(context.Response.OutputStream, ExposeOptions.Default);
                }
                else
                {
                    context.Response.StatusCode = 405; // Method not allowed
                }
                context.Response.Close();
            }
            catch (ObjectDisposedException)
            {
                // Do not throw exception on shutdown
            }
            catch (HttpListenerException ex)
            {
                logger.LogWarning(ex, "HTTP error while providing metrics");
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Unexpected error while providing metrics");
            }
            finally
            {
                BeginContext();
            }
        }
    }
}
