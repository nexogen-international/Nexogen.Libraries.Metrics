using Microsoft.AspNetCore.Http;
using System;
using System.Diagnostics;
using System.IO;
using System.Threading.Tasks;

namespace Nexogen.Libraries.Metrics.Prometheus.AspCore
{
    internal class ServeMetricsMiddleware
    {
        readonly RequestDelegate next;
        readonly IExposable exposable;

        public ServeMetricsMiddleware(RequestDelegate next, IExposable exposable)
        {
            this.next = next;
            this.exposable = exposable ?? throw new ArgumentNullException(nameof(exposable));
        }

        public async Task Invoke(HttpContext context)
        {
            var sw = Stopwatch.StartNew();
            context.Response.StatusCode = 200;
            context.Response.Headers["Content-Type"] = "text/plain; version=0.0.4; charset=utf-8";

            await using (var writer = new StreamWriter(context.Response.Body, PrometheusConventions.PrometheusEncoding, 128, true))
            {
                writer.NewLine = "\n";

                await writer.WriteLineAsync("# Exposing Nexogen.Libraries.Metrics.Prometheus\n");
                await writer.FlushAsync();

                await exposable.Expose(context.Response.Body, ExposeOptions.Default);
                await writer.WriteLineAsync($"# Elapsed: {sw.ElapsedMilliseconds}ms");
            }
        }
    }
}
