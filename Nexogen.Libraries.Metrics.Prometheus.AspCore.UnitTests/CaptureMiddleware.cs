using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Text;

namespace Nexogen.Libraries.Metrics.Prometheus.AspCore.UnitTests
{
    class CaptureMiddleware : CollectMetricsMiddleware
    {
        readonly PathCapture pathCapture;

        public CaptureMiddleware(RequestDelegate next, HttpMetrics metrics, PathCapture pathCapture)
            : base(next, metrics)
        {
            this.pathCapture = pathCapture;
        }

        protected override string GetHttpMetricPath(HttpContext context)
        {
            var path = base.GetHttpMetricPath(context);
            pathCapture.Paths.Add(path);
            return path;
        }
    }
}
