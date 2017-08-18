using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using static System.Threading.Interlocked;
using static Nexogen.Libraries.Metrics.Prometheus.PrometheusConventions;

namespace Nexogen.Libraries.Metrics.Prometheus
{
    public class LabelledCounter : ILabelledCounter, IPrometheusExposable
    {
        private readonly string help;
        private readonly string name;

        private readonly string[] labelNames;
        private readonly ConcurrentDictionary<string[], Counter> children = new ConcurrentDictionary<string[], Counter>(new StringArrayComparer());

        internal LabelledCounter(string help, string name, string[] labelNames)
        {
            this.name = name;
            this.help = help;
            this.labelNames = labelNames;
        }

        public ICounter Labels(params string[] labels)
        {
            if (labelNames.Length != labels.Length)
            {
                throw new ArgumentException("The number of labels should be equal the number of label names");
            }

            // if does not exist, create a new one
            return children.GetOrAdd(labels.Select(l => EscapeLabel(l)).ToArray(),
                key => new Counter(help, name, labelNames, key));
        }

        public async Task ExposeText(TextWriter writer, ExposeOptions options)
        {
            if (!children.IsEmpty)
            {
                await writer.WriteLineAsync($"# HELP {name} {help}");
                await writer.WriteLineAsync($"# TYPE {name} counter");

                foreach (var child in children)
                {
                    await child.Value.ExposeText(writer, options);
                }
            }
        }
    }
}
