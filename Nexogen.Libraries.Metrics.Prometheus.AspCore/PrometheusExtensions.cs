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
        /// <param name="getHttpPath">Function to retrieve metric path from http context</param>
        /// <returns></returns>
        public static IApplicationBuilder UsePrometheus(this IApplicationBuilder builder)
        {
            return builder.UseMiddleware<CollectMetricsMiddleware>()
                .Map("/metrics", cfg =>
                {
                    cfg.UseMiddleware<ServeMetricsMiddleware>();
                });
        }

        /// <summary>
        /// Gets the last <see cref="RouteBase"/> that
        /// matched the request. We use the route base because the <see cref="IRouter"/>
        /// doesn't expose the template text.        
        /// </summary>
        /// <param name="routers">List of routers that matched the request</param>
        /// <returns></returns>
        internal static RouteBase FindLast(this IList<IRouter> routers)
        {
            if (routers == null || routers.Count == 0)
                return null;
            for (int i = routers.Count - 1; i >= 0; i--)
            {
                if (routers[i] is RouteBase)
                    return routers[i] as RouteBase;
            }
            return null;
        }
    }
}
