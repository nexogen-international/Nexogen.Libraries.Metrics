using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using static Nexogen.Libraries.Metrics.Prometheus.PrometheusConventions;

namespace Nexogen.Libraries.Metrics.Prometheus
{
    public class LabelledCounterBuilder : ILabelledCounterBuilder
    {
        private string help;
        private string name;
        private string[] labelNames;
        private PrometheusRegistry registry;

        public LabelledCounterBuilder(PrometheusRegistry registry, string name, string help, string[] labelNames)
        {
            this.help = help;
            this.name = name;
            this.labelNames = labelNames;
            this.registry = registry;
        }

        public ILabelledCounterBuilder Help(string help)
        {
            this.help = EscapeHelp(help);

            return this;
        }

        public ILabelledCounterBuilder LabelNames(params string[] labelNames)
        {
            if (labelNames.Any(l => !IsValidLabel(l)))
            {
                throw new ArgumentException("Label names must follow prometheus conventions");
            }

            this.labelNames = labelNames;

            return this;
        }

        public ILabelledCounterBuilder Name(string name)
        {
            if (!IsValidName(name))
            {
                throw new ArgumentException("Name must follow prometheus conventions");
            }

            this.name = name;

            return this;
        }

        public ILabelledCounter Register()
        {
            if (help == null)
            {
                throw new ArgumentException("Help is required");
            }

            if (name == null)
            {
                throw new ArgumentException("Name is required");
            }

            var counter = new LabelledCounter(help, name, labelNames);
            registry.Register(name, counter);

            return counter;
        }
    }
}
