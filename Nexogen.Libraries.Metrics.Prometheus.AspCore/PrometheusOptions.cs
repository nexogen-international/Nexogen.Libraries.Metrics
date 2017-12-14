using Microsoft.AspNetCore.Builder;
using System;
using System.Collections.Generic;
using System.Text;

namespace Nexogen.Libraries.Metrics.Prometheus.AspCore
{
    internal class PrometheusOptions : IPrometheusOptions
    {
        private readonly IApplicationBuilder builder;

        public PrometheusOptions(IApplicationBuilder builder)
        {
            this.builder = builder;
        }

        public IPrometheusOptions CollectHttpMetrics()
        {
            builder.UseMiddleware<CollectMetricsMiddleware>();

            return this;
        }
    }
}
