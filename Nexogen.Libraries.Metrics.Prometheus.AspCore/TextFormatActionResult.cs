using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Nexogen.Libraries.Metrics.Prometheus.AspCore
{
    internal class TextFormatActionResult : IActionResult
    {
        private IExposable exposable;

        public TextFormatActionResult(IExposable exposable)
        {
            this.exposable = exposable;
        }

        public async Task ExecuteResultAsync(ActionContext context)
        {
            var sw = Stopwatch.StartNew();
            context.HttpContext.Response.StatusCode = 200;
            context.HttpContext.Response.Headers["Content-Type"] = "text/plain; version=0.0.4; charset=utf-8";

            using (var writer = new StreamWriter(context.HttpContext.Response.Body, PrometheusConventions.UTF8, 128, true))
            {
                writer.NewLine = "\n";

                await writer.WriteLineAsync("# Exposing Nexogen.Libraries.Metrics.Prometheus\n");
                await writer.FlushAsync();

                await exposable.Expose(context.HttpContext.Response.Body, ExposeOptions.Default);
                await writer.WriteLineAsync($"# Elapsed: {sw.ElapsedMilliseconds}ms");
            }
        }
    }
}
