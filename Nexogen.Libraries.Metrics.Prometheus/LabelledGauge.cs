using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Nexogen.Libraries.Metrics.Prometheus.PrometheusConventions;

namespace Nexogen.Libraries.Metrics.Prometheus
{
    public class LabelledGauge : ILabelledGauge, IPrometheusExposable
    {
        private readonly string name;
        private readonly string help;
        private readonly string[] labelNames;

        private ConcurrentDictionary<string[], Gauge> children = new ConcurrentDictionary<string[], Gauge>(new StringArrayComparer());

        public LabelledGauge(string help, string name, string[] labelNames)
        {
            this.help = help;
            this.name = name;
            this.labelNames = labelNames;
        }

        public IGauge Labels(params string[] labels)
        {
            if (labelNames.Length != labels.Length)
            {
                throw new ArgumentException("The number of labels should be equal the number of label names");
            }

            // if does not exist, create a new one
            return children.GetOrAdd(labels.Select(l => EscapeLabel(l)).ToArray(),
                key => new Gauge(help, name, labelNames, key));
        }

        public void ReplaceMetricValues(IDictionary<string[], double> newMetricValues)
        {
            if (newMetricValues == null)
            {
                throw new ArgumentNullException(nameof(newMetricValues));
            }

            if (newMetricValues.Keys.Any(x => x.Length != labelNames.Length))
            {
                throw new ArgumentException($"The number of labels should be equal to the number of label names for all value");
            }

            var newChildren = new ConcurrentDictionary<string[], Gauge>(new StringArrayComparer());

            foreach (var newMetricValue in newMetricValues)
            {
                var gauge = newChildren.GetOrAdd(newMetricValue.Key.Select(l => EscapeLabel(l)).ToArray(),
                    key => new Gauge(help, name, labelNames, key));

                gauge.Value = newMetricValue.Value;
            }

            this.children = newChildren;
        }

        public async Task ExposeText(TextWriter writer, ExposeOptions options)
        {
            if (!children.IsEmpty)
            {
                await writer.WriteLineAsync($"# HELP {name} {help}");
                await writer.WriteLineAsync($"# TYPE {name} gauge");

                foreach (var child in children.Values)
                {
                    await child.ExposeText(writer, options);
                }
            }
        }
    }
}
