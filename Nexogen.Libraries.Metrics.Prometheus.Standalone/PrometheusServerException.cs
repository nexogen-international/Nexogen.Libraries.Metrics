using System;
using System.Collections.Generic;
using System.Text;

namespace Nexogen.Libraries.Metrics.Prometheus.Standalone
{
    /// <summary>
    /// Generic exception to hide internal exceptions from PrometheusServer.
    /// </summary>
    public class PrometheusServerException : Exception
    {
#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member
        public PrometheusServerException()
        {
        }

        public PrometheusServerException(string message) : base(message)
        {
        }

        public PrometheusServerException(string message, Exception innerException) : base(message, innerException)
        {
        }
#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member
    }
}
