using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Nexogen.Libraries.Metrics.Prometheus.AspCore.UnitTests
{
    static class Ext
    {
        public static async Task WriteAsync(this HttpResponse response, string text)
        {
            var bytes = Encoding.UTF8.GetBytes(text);
            await response.Body.WriteAsync(bytes, 0, bytes.Length);
        }

        public static IServiceCollection AddTestServices(this IServiceCollection services)
        {
            var metrics = new PrometheusMetrics();
            services.AddSingleton<IMetrics>(metrics);
            services.AddSingleton<HttpMetrics>();
            return services;
        }

        public static IApplicationBuilder UsePrometheus(this IApplicationBuilder app, PathCapture capture)
        {
            return app.UseMiddleware<CaptureMiddleware>(capture);
        }
    }


    public class PathCapture
    {
        public List<string> Paths { get; set; } = new List<string>();
    }
}
