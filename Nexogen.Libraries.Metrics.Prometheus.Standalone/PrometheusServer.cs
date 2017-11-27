using System;
using System.Collections.Generic;
using System.Text;
using System.Net.Http;
using System.Net;
using Nexogen.Libraries.Metrics.Prometheus;

namespace Nexogen.Libraries.Metrics.Prometheus.Standalone
{
    public class PrometheusServer
    {
        HttpListener listener;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="metrics"></param>
        /// <param name="listener"></param>
        public PrometheusServer(IExposable metrics, HttpListener listener)
        {
            this.listener = listener;

            this.listener.Request += async (sender, context) =>
            {
                // TODO: Could be configurable in the future throught a builder
                if (context.Request.HttpMethod == HttpMethods.Get
                    && context.Request.Url.AbsolutePath == "/metrics")
                {
                    context.Response.StatusCode = 200;
                    context.Response.Headers.Add("Content-Type", "text/plain; version=0.0.4");
                    await metrics.Expose(context.Response.OutputStream, ExposeOptions.Default);
                }
                else
                {
                    context.Response.NotFound();
                }
            };

            try
            {
                listener.Start();
            }
            catch (Exception ex)
            {
                throw new PrometheusServerException("Cannot start HTTP Listener", ex);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="metrics"></param>
        /// <param name="address"></param>
        /// <param name="port"></param>
        public PrometheusServer(IExposable metrics, IPAddress address, int port)
            : this(metrics, new HttpListener(address, port))
        {
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="metrics"></param>
        /// <param name="port"></param>
        public PrometheusServer(IExposable metrics, int port)
            : this(metrics, IPAddress.Parse("0.0.0.0"), port)
        {
        }
    }
}
