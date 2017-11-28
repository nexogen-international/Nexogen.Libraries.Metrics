using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Extensions;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;

namespace Nexogen.Libraries.Metrics.Prometheus.AspCore
{
    public static class PrometheusExtensions
    {
        /// <summary>
        /// Configure DI for using the Prometheus Metrics library.
        /// </summary>
        /// <param name="services"></param>
        /// <returns></returns>
        public static IServiceCollection AddPrometheus(this IServiceCollection services)
        {
            var prometheus = new PrometheusMetrics();

            services.AddSingleton<IMetrics>(prometheus);
            services.AddSingleton<IExposable>(prometheus);

            services.AddSingleton<HttpMetrics, HttpMetrics>();

            return services;
        }

        /// <summary>
        /// Add Prometheus Metrics support to the application.
        /// </summary>
        /// <param name="builder"></param>
        /// <returns></returns>
        public static IApplicationBuilder UsePrometheus(this IApplicationBuilder builder)
        {
            return builder.UseMiddleware<CollectMetricsMiddleware>()
                .Map("/metrics", cfg =>
                {
                    cfg.UseMiddleware<ServeMetricsMiddleware>();
                });
        }
    }
}
