using System;
using System.Collections.Generic;
using System.Text;
using System.Net.Http;
using System.Net;
using Nexogen.Libraries.Metrics.Prometheus;

namespace Nexogen.Libraries.Metrics.Prometheus.Standalone
{
    /// <summary>
    /// A simple Prometheus metric server using an embedded HTTP server.
    /// </summary>
    public class PrometheusServer : IDisposable
    {
        HttpListener listener;

        /// <summary>
        /// Creates and starts a metric server.
        /// </summary>
        /// <param name="metrics">The metric to expose</param>
        /// <param name="listener">An HttpListener instance to use</param>
        public PrometheusServer(IExposable metrics, HttpListener listener)
        {
            this.listener = listener ?? throw new ArgumentNullException(nameof(listener));

            if (metrics == null)
            {
                throw new ArgumentNullException(nameof(metrics));
            }

            try
            {
                this.listener.Request += async (sender, context) =>
                {
                    if (context.Request.HttpMethod == HttpMethods.Get)
                    {
                        context.Response.StatusCode = 200;
                        context.Response.Headers.Add("Content-Type", "text/plain; version=0.0.4");
                        await metrics.Expose(context.Response.OutputStream, ExposeOptions.Default);
                    }
                    else
                    {
                        context.Response.NotFound();
                    }

                    context.Response.Close();
                };

                listener.Start();
            }
            catch (Exception ex)
            {
                throw new PrometheusServerException("Cannot start HTTP Listener", ex);
            }
        }

        /// <summary>
        /// Creates and starts a metric server.
        /// </summary>
        /// <param name="metrics">The metric to expose</param>
        /// <param name="address">The IP address to listen on</param>
        /// <param name="port">The TCP port to bind to</param>
        public PrometheusServer(IExposable metrics, IPAddress address, int port)
            : this(metrics, new HttpListener(address, port))
        {
        }

        /// <summary>
        /// Creates and starts a metric server.
        /// </summary>
        /// <param name="metrics">The metric to expose</param>
        /// <param name="port">The TCP port to bind to</param>
        public PrometheusServer(IExposable metrics, int port)
            : this(metrics, IPAddress.Parse("0.0.0.0"), port)
        {
        }

        /// <summary>
        /// Closes the server socket.
        /// </summary>
        public void Dispose()
        {
            listener.Dispose();
        }
    }
}
