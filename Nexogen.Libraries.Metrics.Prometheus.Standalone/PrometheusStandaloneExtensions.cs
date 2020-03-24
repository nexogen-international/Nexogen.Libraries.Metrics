using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Nexogen.Libraries.Metrics;
using Nexogen.Libraries.Metrics.Prometheus;

namespace Nexogen.Libraries.Metrics.Prometheus.Standalone
{
    public static class PrometheusStandaloneExtensions
    {
        /// <summary>
        /// Add standalone Prometheus server.
        /// </summary>
        /// <param name="services">The service collection</param>
        /// <param name="configuration">Optional configuration source for <see cref="PrometheusServerOptions"/></param>
        public static IServiceCollection AddPrometheusStandalone(this IServiceCollection services, IConfiguration configuration = null)
            => services.AddPrometheusStandalone(new PrometheusMetrics(), configuration);

        /// <summary>
        /// Add standalone Prometheus server.
        /// </summary>
        /// <param name="services">The service collection</param>
        /// <param name="prometheusMetrics">The metrics object</param>
        /// <param name="configuration">Optional configuration source for <see cref="PrometheusServerOptions"/></param>
        public static IServiceCollection AddPrometheusStandalone<T>(this IServiceCollection services, T prometheusMetrics, IConfiguration configuration = null)
            where T : class, IMetrics, IExposable
            => services.Configure<PrometheusServerOptions>(configuration ?? new ConfigurationBuilder().Build())
                           .AddSingleton<IMetrics>(prometheusMetrics)
                           .AddSingleton<IExposable>(prometheusMetrics)
                           .AddHostedService<PrometheusServer>();
    }
}
