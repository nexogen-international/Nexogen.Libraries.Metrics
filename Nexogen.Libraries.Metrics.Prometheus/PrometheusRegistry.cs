using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Collections.Concurrent;

namespace Nexogen.Libraries.Metrics.Prometheus
{
    public class PrometheusRegistry
    {
        private readonly ConcurrentDictionary<string, IPrometheusExposable> exposables = new ConcurrentDictionary<string, IPrometheusExposable>();
        private readonly CoreclrExporter coreclrExporter;

        public PrometheusRegistry(CoreclrExporter coreclrExporter = null)
        {
            this.coreclrExporter = coreclrExporter;
        }

        public async Task Expose(Stream output, ExposeOptions options)
        {
            // collect standard exporter metrics
            if (coreclrExporter != null)
            {
                coreclrExporter.Collect();
            }

            using (var writer = new StreamWriter(new BufferedStream(output), PrometheusConventions.PrometheusEncoding, 4096, true))
            {
                writer.NewLine = "\n";

                foreach (var exposable in exposables)
                {
                    await exposable.Value.ExposeText(writer, options);
                    await writer.WriteLineAsync();
                }
            }
        }

        public void Register(string name, IPrometheusExposable exposable)
        {
            if (!exposables.TryAdd(name, exposable))
            {
                throw new CollectorBuilderException($"A collector is already registered with the name: {name}");
            }
        }
    }
}
