using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

namespace Nexogen.Libraries.Metrics.Prometheus.AspCore
{
    [Route("/metrics")]
    [ApiExplorerSettings(IgnoreApi = true)]
    public class MetricsController : Controller
    {
        private IExposable exposable;

        public MetricsController(IExposable metrics)
        {
            exposable = metrics;
        }

        [HttpGet]
        [Produces("text/plain; version=0.0.4; charset=utf-8")]
        public IActionResult Get()
        {
            return new TextFormatActionResult(exposable);
        }
    }
}
