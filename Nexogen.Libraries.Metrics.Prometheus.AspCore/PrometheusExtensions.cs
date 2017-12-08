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
            => services.AddPrometheus(new PrometheusMetrics());

        /// <summary>
        /// Configure DI for using the Prometheus Metrics library using an externally 
        /// created and managed metrics object. The metrics object will be registered
        /// as singletons. T must be both <see cref="IMetrics"/> &amp; <see cref="IExposable"/>
        /// </summary>
        /// <typeparam name="T">The metrics object type</typeparam>
        /// <param name="services">Service collection to add to</param>
        /// <param name="prometheusMetrics">The metrics object</param>
        /// <returns></returns>
        public static IServiceCollection AddPrometheus<T>(this IServiceCollection services, T prometheusMetrics)
            where T : class, IMetrics, IExposable
        {
            if (prometheusMetrics == null)
                throw new ArgumentNullException(nameof(prometheusMetrics));

            services.AddSingleton<IMetrics>(prometheusMetrics);
            services.AddSingleton<IExposable>(prometheusMetrics);

            services.AddSingleton<HttpMetrics>();
            return services;
        }

        /// <summary>
        /// Add Prometheus Metrics support to the application.
        /// </summary>
        /// 
        /// <param name="builder">The IApplicationBuilder instance.</param>
        /// <param name="options">Options to tune metrics behaviour.</param>
        /// <returns></returns>
        public static IApplicationBuilder UsePrometheus(this IApplicationBuilder builder, Action<IPrometheusOptions> options = null)
        {
            options?.Invoke(new PrometheusOptions(builder));

            return builder.Map("/metrics", cfg =>
            {
                cfg.UseMiddleware<ServeMetricsMiddleware>();
            });
        }
    }
}
