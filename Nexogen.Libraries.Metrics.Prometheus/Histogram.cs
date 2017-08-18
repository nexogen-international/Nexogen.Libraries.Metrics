using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using static Nexogen.Libraries.Metrics.Prometheus.PrometheusConventions;

namespace Nexogen.Libraries.Metrics.Prometheus
{
    public class Histogram : IHistogram, IPrometheusExposable
    {
        private readonly Bucket[] buckets;
        private readonly double[] maxValues;
        private readonly AtomicDouble sum;
        private readonly string help;
        private readonly string name;
        private readonly string[] labels;
        private readonly string[] labelNames;

        public Histogram(IBucket[] buckets, string help, string name, string[] labelNames, string[] labels)
        {
            if (!NameRegex.IsMatch(name))
            {
                throw new ArgumentException($"Invalid metric name: {name}");
            }

            this.name = name;
            this.help = help;
            this.labels = labels;
            this.labelNames = labelNames;
            this.buckets = buckets.Select( b=> new Bucket(b) ).ToArray();

            this.maxValues = buckets.Select(b => b.Max).ToArray();
            this.sum = new AtomicDouble();
        }

        public Histogram(IBucket[] type, string help, string name) 
            : this(type, help, name, new string[0], new string[0])
        {
        }

        public void Observe(double value)
        {
            this.sum.Add(value);

            var pos = Array.BinarySearch(maxValues, value, Comparer<double>.Default);
            if (pos < 0)
            {
                pos = ~pos;
            }

            buckets[pos].IncrementItemCount();
        }

        public async Task ExposeText(TextWriter writer, ExposeOptions options)
        {
            var labelList = BuildLabels(labelNames, labels);
            if (labelList.Length == 0)
            {
                await writer.WriteLineAsync($"# HELP {name} {help}");
                await writer.WriteLineAsync($"# TYPE {name} histogram");
            }

            long bucketsTotal = 0L;
            foreach (var bucket in buckets)
            {
                bucketsTotal += bucket.ItemCount;
                string labelforBucket = BuildHistogramLabels(labelList, bucket.Max, bucketsTotal);

                await writer.WriteLineAsync($"{name}_bucket{labelforBucket}");
            }

            await writer.WriteLineAsync($"{name}_sum{labelList} {this.sum.Value.ToString(CultureInfo.InvariantCulture)}");
            await writer.WriteLineAsync($"{name}_count{labelList} {bucketsTotal}");     
        }
    }
}
