using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace Nexogen.Libraries.Metrics.Prometheus
{
    public class PrometheusMetrics : IMetrics, IExposable
    {
        private readonly PrometheusRegistry registry;

        public PrometheusMetrics()
        {
            registry = new PrometheusRegistry(new CoreclrExporter(this));
        }

        public async Task Expose(Stream output)
        {
            await registry.Expose(output, ExposeOptions.Default);
        }

        public async Task Expose(Stream output, ExposeOptions options)
        {
            await registry.Expose(output, options);
        }

        public ICounterBuilder Counter()
        {
            return new CounterBuilder(registry);
        }

        public IGaugeBuilder Gauge()
        {
            return new GaugeBuilder(registry);
        }

        public IHistogramBuilder Histogram()
        {
            return new HistogramBuilder(registry);
        }
    }
}
