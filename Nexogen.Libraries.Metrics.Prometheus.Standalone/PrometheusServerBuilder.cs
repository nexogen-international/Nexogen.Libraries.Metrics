using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Text;

namespace Nexogen.Libraries.Metrics.Prometheus.Standalone
{
    /// <summary>
    /// Builder to start a standalone metrics server.
    /// </summary>
    public class PrometheusServerBuilder
    {
        private IExposable exposable;
        private int port = 80;
        private IPAddress address = IPAddress.Parse("0.0.0.0");
        private Action<Exception> errorHandler;

        /// <summary>
        /// Creates a new builder. Should be called through the extension method on IExposable.
        /// </summary>
        /// <param name="exposable">The exposable to server over HTTP.</param>
        public PrometheusServerBuilder(IExposable exposable)
        {
            this.exposable = exposable;
        }

        /// <summary>
        /// Sets the port to listen on.
        /// </summary>
        /// <param name="port">The TCP Port.</param>
        /// <returns>The builder.</returns>
        public PrometheusServerBuilder Port(int port)
        {
            this.port = port;

            return this;
        }

        /// <summary>
        /// Sets the address to listen on.
        /// </summary>
        /// <param name="address">The address.</param>
        /// <returns>THe builder.</returns>
        public PrometheusServerBuilder Address(string address)
        {
            this.address = IPAddress.Parse(address);

            return this;
        }

        /// <summary>
        /// Sets the address to listen on.
        /// </summary>
        /// <param name="address">The address.</param>
        /// <returns>THe builder.</returns>
        public PrometheusServerBuilder Address(IPAddress address)
        {
            this.address = address;

            return this;
        }

        /// <summary>
        /// Sets the error handling callback of the metrics server. The action will be called with an PrometheusServerException instance as argument.
        /// </summary>
        /// <param name="errorHandler">The error handling action.</param>
        /// <returns>The builder.</returns>
        public PrometheusServerBuilder OnError(Action<Exception> errorHandler)
        {
            this.errorHandler = errorHandler;

            return this;
        }

        /// <summary>
        /// Creates and starts the server. Dispose it to stop listening.
        /// </summary>
        /// <returns>The server's handle.</returns>
        public IDisposable Start()
        {
            return new PrometheusServer(exposable, new HttpListener(address, port), errorHandler);
        }
    }
}
