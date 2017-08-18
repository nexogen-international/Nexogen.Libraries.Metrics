using System;
using System.Collections.Concurrent;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Nexogen.Libraries.Metrics.Prometheus.PrometheusConventions;

namespace Nexogen.Libraries.Metrics.Prometheus
{
    public class LabelledHistogram : ILabelledHistogram, IPrometheusExposable
    {
        private readonly string help;
        private readonly string name;
        private readonly Bucket[] buckets;
        private readonly string[] labelNames;
        private readonly ConcurrentDictionary<string[], Histogram> children = new ConcurrentDictionary<string[], Histogram>(new StringArrayComparer());

        internal LabelledHistogram(IBucket[] buckets, string help, string name, string[] labelNames)
        {
            this.name = name;
            this.help = help;
            this.labelNames = labelNames;
            this.buckets = buckets.Select(b => new Bucket(b)).ToArray();
        }

        public IHistogram Labels(params string[] labels)
        {
            if (labelNames.Length != labels.Length)
            {
                throw new ArgumentException("The number of labels should be equal the number of label names");
            }

            Bucket[] bucketsCopy = new Bucket[buckets.Length];
            for (int i = 0; i < buckets.Length; i++)
            {
               bucketsCopy[i] = new Bucket(buckets[i].Min, buckets[i].Max);
            }

            // if does not exist, create a new one
            return children.GetOrAdd(labels.Select(l => EscapeLabel(l)).ToArray(),
                key => new Histogram(bucketsCopy, help, name, labelNames, key));
        }

        public async Task ExposeText(TextWriter writer, ExposeOptions options)
        {
            if (!children.IsEmpty)
            {
                await writer.WriteLineAsync($"# HELP {name} {help}");
                await writer.WriteLineAsync($"# TYPE {name} histogram");

                foreach (var child in children)
                {
                    await child.Value.ExposeText(writer, options);
                }
            }
        }
    }
}
