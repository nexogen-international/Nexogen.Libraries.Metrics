using System;
using System.Collections.Generic;
using System.Text;

namespace Nexogen.Libraries.Metrics.Prometheus.AspCore
{
    /// <summary>
    /// Options passed to Prometheus
    /// </summary>
    public interface IPrometheusOptions
    {
        /// <summary>
        /// Collect HTTP Statistics.
        /// </summary>
        IPrometheusOptions CollectHttpMetrics();
    }
}
