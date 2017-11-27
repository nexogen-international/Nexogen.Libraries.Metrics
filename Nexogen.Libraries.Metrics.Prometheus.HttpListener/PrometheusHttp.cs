using System;
using System.Net;
using System.Net.Http;

namespace Nexogen.Libraries.Metrics.Prometheus.HttpListener
{
    public class PrometheusHttp
    {
        HttpListener listener;

        public PrometheusHttp()
        {
            listener = new HttpListener(IPAddress.Parse("127.0.0.1"), 8081);
        }
    }
}
