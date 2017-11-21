using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.DependencyInjection;
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
        public static IApplicationBuilder UsePrometheus(this IApplicationBuilder builder, System.Func<HttpContext, string> getHttpPath = null)
        {
            return builder.UseMiddleware<CollectMetricsMiddleware>(getHttpPath)
                .Map("/metrics", cfg =>
                {
                    cfg.UseMiddleware<ServeMetricsMiddleware>();
                });
        }

        /// <summary>
        /// Gets the path from a http context. If the path was handled by routing
        /// middleware we attempt to get the route template falling back to Request.Path
        /// if we are unable to find the template
        ///  
        /// Based on the implementation of <see cref="RouterMiddleware"/> define here <a href="https://github.com/aspnet/Routing/blob/dev/src/Microsoft.AspNetCore.Routing/RouterMiddleware.cs"/>
        /// when the context matches a route then we add an IRoutingFeature to the <see cref="HttpContext.Features"/> collection with the <see cref="IRoutingFeature.RouteData"/>
        /// set to those provided by the matched route. So to get the last matched route we need to get walk backwards from the <see cref="RouteData.Routers"/> collection. Because we
        /// are template based we need to check if the router is a <see cref="RouteBase"/> and if so extract the <see cref="RouteBase.ParsedTemplate"/>. 
        /// 
        /// Why not use the last router? because MVC for examples adds it's internal router into the RouteData collection.
        /// </summary>
        /// <param name="context">The current  http context</param>
        /// <returns></returns>
        public static string GetHttpMetricPath(HttpContext context)
        {
            var feature = context.Features.Get<IRoutingFeature>();
            string path = null;
            if(feature != null)
            {
                var lastRouter = feature.RouteData.Routers.FindLast();
                if (lastRouter != null)
                {
                    path = lastRouter.ParsedTemplate.TemplateText;
                }
            }
            return path ?? context.Request.Path.Value.ToLowerInvariant();
        }

        /// <summary>
        /// Gets the last <see cref="RouteBase"/> that
        /// matched the request. We use the route base because the <see cref="IRouter"/>
        /// doesn't expose the template text.        
        /// </summary>
        /// <param name="routers">List of routers that matched the request</param>
        /// <returns></returns>
        static RouteBase FindLast(this IList<IRouter> routers)
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
