using System;
using System.Collections.Generic;
using System.Text;

namespace Nexogen.Libraries.Metrics.Prometheus.Standalone
{
    public class PrometheusServerException : Exception
    {
        public PrometheusServerException()
        {
        }

        public PrometheusServerException(string message) : base(message)
        {
        }

        public PrometheusServerException(string message, Exception innerException) : base(message, innerException)
        {
        }
    }
}
