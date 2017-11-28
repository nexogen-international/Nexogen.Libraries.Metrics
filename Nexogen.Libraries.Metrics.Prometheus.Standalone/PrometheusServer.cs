using System;
using System.Collections.Generic;
using System.Text;
using System.Net.Http;
using System.Net;
using Nexogen.Libraries.Metrics.Prometheus;
using System.Threading.Tasks;

namespace Nexogen.Libraries.Metrics.Prometheus.Standalone
{
    /// <summary>
    /// A simple Prometheus metric server using an embedded HTTP server.
    /// </summary>
    internal class PrometheusServer : IDisposable
    {
        private readonly HttpListener listener;
        private readonly IExposable metrics;
        private readonly Action<Exception> errorHandler;

        /// <summary>
        /// Creates and starts a metric server.
        /// </summary>
        /// <param name="metrics">The metric to expose.</param>
        /// <param name="listener">An HttpListener instance to use.</param>
        /// <param name="errorHandler">Error handling callback. Can be null.</param>
        public PrometheusServer(IExposable metrics, HttpListener listener, Action<Exception> errorHandler)
        {
            this.listener = listener ?? throw new ArgumentNullException(nameof(listener));
            this.metrics = metrics ?? throw new ArgumentNullException(nameof(metrics));
            this.errorHandler = errorHandler;

            try
            {
                this.listener.Request += RequestHandler;

                listener.Start();
            }
            catch (Exception ex)
            {
                throw new PrometheusServerException("Cannot start HTTP Listener", ex);
            }
        }

        private async void RequestHandler(object sender, HttpListenerRequestEventArgs context)
        {
            try
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
            }
            catch (Exception ex)
            {
                try
                {
                    errorHandler?.Invoke(new PrometheusServerException("Error while handling HTTP request", ex));
                }
                catch (Exception)
                {
                    // ignore exceptions thrown by user supplied callback
                }
            }
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
