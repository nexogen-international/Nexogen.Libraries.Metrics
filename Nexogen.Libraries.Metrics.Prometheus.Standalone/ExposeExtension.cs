using System;
using System.Collections.Generic;
using System.Text;

namespace Nexogen.Libraries.Metrics.Prometheus.Standalone
{
    /// <summary>
    /// Extensions for IExposable.
    /// </summary>
    public static class ExposeExtension
    {
        /// <summary>
        /// Extension method for starting a standalone metrics server.
        /// </summary>
        /// <param name="exposable">The exposable to server.</param>
        /// <returns>A builder.</returns>
        public static PrometheusServerBuilder Server(this IExposable exposable)
        {
            return new PrometheusServerBuilder(exposable);
        }
    }
}
