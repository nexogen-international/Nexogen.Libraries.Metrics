using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using static Nexogen.Libraries.Metrics.Prometheus.PrometheusConventions;

namespace Nexogen.Libraries.Metrics.Prometheus
{
    public class HistogramBuilder : IHistogramBuilder
    {
        private string help;
        private string name;
        private IBucket[] buckets;
        private readonly PrometheusRegistry registry;
        private static readonly double[] defaultBucketEdges = { 0, .005, .01, .025, .05, .075, .1, .25, .5, .75, 1, 2.5, 5, 7.5, 10, double.PositiveInfinity };

        public HistogramBuilder(PrometheusRegistry registry)
        {
            this.registry = registry;

            this.buckets = new Bucket[defaultBucketEdges.Length - 1];
            for (int i = 0; i < defaultBucketEdges.Length - 1; i++)
            {
                buckets[i] = new Bucket(defaultBucketEdges[i], defaultBucketEdges[i + 1]);
            }
        }

        public IHistogramBuilder Help(string help)
        {
            this.help = EscapeHelp(help);

            return this;
        }

        public ILabelledHistogramBuilder LabelNames(params string[] labelNames)
        {
            if (!AreValidNames(labelNames))
            {
                throw new ArgumentException("Lable names must follow prometheus conventions");
            }

            return new LabelledHistogramBuilder(buckets, registry, name, help, labelNames);
        }

        public IHistogramBuilder Name(string name)
        {
            if (!IsValidHistogramName(name))
            {
                throw new ArgumentException("Name must follow prometheus conventions");
            }

            this.name = name;

            return this;
        }

        public IHistogramBuilder Buckets(IBucket[] buckets)
        {
            if (buckets == null)
            {
                throw new ArgumentNullException(nameof(buckets));
            }

            this.buckets = buckets;

            return this;
        }

        public IHistogram Register()
        {
            if (help == null)
            {
                throw new ArgumentException("Help is required");
            }

            if (name == null)
            {
                throw new ArgumentException("Name is requried");
            }
            
            var histogram = new Histogram(buckets, help, name);
            registry.Register(name, histogram);

            return histogram;
        }
    }
}
