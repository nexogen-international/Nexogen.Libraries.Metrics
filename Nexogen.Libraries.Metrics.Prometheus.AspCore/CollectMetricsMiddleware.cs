using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using System.Diagnostics;
using System;

namespace Nexogen.Libraries.Metrics.Prometheus.AspCore
{
    internal class CollectMetricsMiddleware
    {
        private readonly RequestDelegate next;
        private readonly HttpMetrics m;
        private readonly Func<HttpContext, string> pathBuilder;

        public CollectMetricsMiddleware(RequestDelegate next, HttpMetrics m, Func<HttpContext, string> pathBuilder)
        {
            this.next = next;
            this.m = m;
            this.pathBuilder = pathBuilder ?? PrometheusExtensions.GetHttpMetricPath;
        }

        public async Task Invoke(HttpContext httpContext)
        {
            var sw = Stopwatch.StartNew();

            await next(httpContext);

            sw.Stop();

            var method = httpContext.Request.Method;
            var handler = pathBuilder(httpContext);
            var statusCode = httpContext.Response.StatusCode.ToString();

            m.HttpRequestDurationSeconds
                .Labels(method, handler, statusCode)
                .Observe(sw.Elapsed.TotalSeconds);

            m.HttpRequestsTotal
                .Labels(method, handler, statusCode)
                .Increment();
        }
    }
}
