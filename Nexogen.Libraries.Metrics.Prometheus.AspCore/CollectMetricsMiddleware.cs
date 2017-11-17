using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using System.Diagnostics;

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
            var handler =  httpContext.Request.Path.Value.ToLower();
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
