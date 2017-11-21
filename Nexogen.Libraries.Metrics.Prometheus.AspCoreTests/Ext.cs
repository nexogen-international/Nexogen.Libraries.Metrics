using Microsoft.AspNetCore.Http;
using Nexogen.Libraries.Metrics.Prometheus.AspCore;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Nexogen.Libraries.Metrics.Prometheus.AspCoreTests
{
    static class Ext
    {
        public static async Task WriteAsync(this HttpResponse response, string text)
        {
            var bytes = Encoding.UTF8.GetBytes(text);
            await response.Body.WriteAsync(bytes, 0, bytes.Length);
        }

        public static Func<HttpContext, string> GetAndCapturePath(PathCapture capture)
        {
            return (ctx) =>
            {
                var str = PrometheusExtensions.GetHttpMetricPath(ctx);
                capture.Paths.Add(str);
                return str;
            };
        }
    }


    public class PathCapture
    {
        public List<string> Paths { get; set; } = new List<string>();
    }
}
