using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using static Nexogen.Libraries.Metrics.Prometheus.PrometheusConventions;

namespace Nexogen.Libraries.Metrics.Prometheus
{
    public class CounterBuilder : ICounterBuilder
    {
        private string help;
        private string name;
        private PrometheusRegistry registry;

        public CounterBuilder(PrometheusRegistry registry)
        {
            this.registry = registry;
        }

        public ICounterBuilder Help(string help)
        {
            this.help = EscapeHelp(help);

            return this;
        }

        public ILabelledCounterBuilder LabelNames(params string[] labelNames)
        {
            if (!AreValidNames(labelNames))
            {
                throw new ArgumentException("Lable names must follow prometheus conventions");
            }

            return new LabelledCounterBuilder(registry, name, help, labelNames);
        }

        public ICounterBuilder Name(string name)
        {
            if (!IsValidName(name))
            {
                throw new ArgumentException("Name must follow prometheus conventions");
            }

            this.name = name;

            return this;
        }

        public ICounter Register()
        {
            if (help == null)
            {
                throw new ArgumentException("Help is required");
            }

            if (name == null)
            {
                throw new ArgumentException("Name is requried");
            }

            var counter = new Counter(help, name);
            registry.Register(name, counter);

            return counter;
        }
    }
}
