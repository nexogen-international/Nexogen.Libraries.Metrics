using System;
using System.Linq;
using static Nexogen.Libraries.Metrics.Prometheus.PrometheusConventions;

namespace Nexogen.Libraries.Metrics.Prometheus
{
    public class LabelledHistogramBuilder : ILabelledHistogramBuilder
    {
        private string help;
        private string name;
        private string[] labelNames;
        private IBucket[] buckets;
        private readonly PrometheusRegistry registry;

        public LabelledHistogramBuilder(IBucket[] buckets, PrometheusRegistry registry, string name, string help, string[] labelNames)
        {
            this.help = help;
            this.name = name;
            this.labelNames = labelNames;
            this.registry = registry;
            this.buckets = buckets;
        }

        public ILabelledHistogramBuilder Help(string help)
        {
            this.help = EscapeHelp(help);

            return this;
        }

        public ILabelledHistogramBuilder LabelNames(params string[] labelNames)
        {
            if (labelNames.Any(l => !IsValidLabel(l)))
            {
                throw new ArgumentException("Label names must follow prometheus conventions");
            }

            this.labelNames = labelNames;

            return this;
        }

        public ILabelledHistogramBuilder Name(string name)
        {
            if (!IsValidName(name))
            {
                throw new ArgumentException("Name must follow prometheus conventions");
            }

            this.name = name;

            return this;
        }

        public ILabelledHistogramBuilder Buckets(IBucket[] buckets)
        {
            if (buckets == null)
            {
                throw new ArgumentNullException(nameof(buckets));
            }

            this.buckets = buckets;

            return this;
        }

        public ILabelledHistogram Register()
        {
            if (help == null)
            {
                throw new ArgumentException("Help is required");
            }

            if (name == null)
            {
                throw new ArgumentException("Name is required");
            }

            var histogram = new LabelledHistogram(buckets, help, name, labelNames);
            registry.Register(name, histogram);

            return histogram;
        }
    }
}
