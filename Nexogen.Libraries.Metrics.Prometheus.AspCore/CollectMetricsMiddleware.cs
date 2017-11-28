using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using System.Diagnostics;
using System;
using Microsoft.AspNetCore.Routing;
using Microsoft.AspNetCore.Http.Extensions;
using System.Collections.Generic;

namespace Nexogen.Libraries.Metrics.Prometheus.AspCore
{
    internal class CollectMetricsMiddleware
    {
        private readonly RequestDelegate next;
        private readonly HttpMetrics m;

        public CollectMetricsMiddleware(RequestDelegate next, HttpMetrics m)
        {
            this.next = next;
            this.m = m;
        }

        public async Task Invoke(HttpContext httpContext)
        {
            var sw = Stopwatch.StartNew();

            await next(httpContext);

            sw.Stop();

            var method = httpContext.Request.Method;
            var handler = GetHttpMetricPath(httpContext);
            var statusCode = httpContext.Response.StatusCode.ToString();

            m.HttpRequestDurationSeconds
                .Labels(method, handler, statusCode)
                .Observe(sw.Elapsed.TotalSeconds);

            m.HttpRequestsTotal
                .Labels(method, handler, statusCode)
                .Increment();
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
        protected virtual string GetHttpMetricPath(HttpContext context)
        {
            var feature = context.Features.Get<IRoutingFeature>();
            string path = null;
            if (feature != null)
            {
                var lastRouter = FindLast(feature.RouteData.Routers);
                if (lastRouter != null)
                {
                    path = GetRoutePath(context, lastRouter);
                }
            }
            return path ?? context.Request.Path.Value.ToLowerInvariant();
        }

        protected virtual string GetRoutePath(HttpContext context, RouteBase lastRouter)
        {
            var path = lastRouter.ParsedTemplate.TemplateText;
            var pathUri = new Uri(context.Request.GetDisplayUrl(), UriKind.Absolute);
            var difference = pathUri.Segments.Length - lastRouter.ParsedTemplate.Segments.Count;
            //segment 0 == '/'
            //segment values end with '/'
            //difference == (1,2,3) implemented to avoid allocating the string builder
            if (difference == 1)
            {
                path = string.Concat(pathUri.Segments[0], path);
            }
            else if (difference == 2)
            {
                //push 0th & 1st segments
                path = string.Concat(pathUri.Segments[0], pathUri.Segments[1], path);
            }
            else if (difference == 3)
            {
                //push 0th & 1st & 2nd segments
                path = string.Concat(pathUri.Segments[0], pathUri.Segments[1], pathUri.Segments[2], path);
            }
            else if (difference > 3)
            {
                //0 should never hit here
                var builder = new System.Text.StringBuilder();
                for (int i = 0; i < difference; i++)
                {
                    builder.Append(pathUri.Segments[i]);
                }
                builder.Append(path);
                path = builder.ToString();
            }
            return path;
        }



        /// <summary>
        /// Gets the last <see cref="RouteBase"/> that
        /// matched the request. We use the route base because the <see cref="IRouter"/>
        /// doesn't expose the template text.        
        /// </summary>
        /// <param name="routers">List of routers that matched the request</param>
        /// <returns></returns>
        static RouteBase FindLast(IList<IRouter> routers)
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
