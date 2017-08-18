using Nexogen.Libraries.Metrics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Nexogen.Libraries.Metrics.Prometheus.AspCore
{
    internal class HttpMetrics
    {
        public ILabelledCounter HttpRequestsTotal
        {
            get;
            protected set;
        }

        public ILabelledHistogram HttpRequestDurationSeconds
        {
            get;
            protected set;
        }

        public HttpMetrics(IMetrics m)
        {
            HttpRequestsTotal = m.Counter()
                .Name("http_requests_total")
                .Help("The total count of http requests")
                .LabelNames("method", "handler", "code")
                .Register();

            HttpRequestDurationSeconds = m.Histogram()
                .Name("http_request_duration_seconds")
                .Buckets(0, .005, .01, .025, .05, .075, .1, .25, .5, .75, 1, 2.5, 5, 7.5, 10, 30, 60, 120, 180, 240, 300)
                .Help("Total duration of http request")
                .LabelNames("method", "handler", "code")
                .Register();
        }
    }
}
