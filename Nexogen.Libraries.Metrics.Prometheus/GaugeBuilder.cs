using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using static Nexogen.Libraries.Metrics.Prometheus.PrometheusConventions;

namespace Nexogen.Libraries.Metrics.Prometheus
{
    public class GaugeBuilder : IGaugeBuilder
    {
        private string help;
        private string name;
        private PrometheusRegistry registry;

        public GaugeBuilder(PrometheusRegistry registry)
        {
            this.registry = registry;
        }

        public IGaugeBuilder Help(string help)
        {
            this.help = EscapeHelp(help);

            return this;
        }

        public ILabelledGaugeBuilder LabelNames(params string[] labelNames)
        {
            if (!AreValidNames(labelNames))
            {
                throw new ArgumentException("Lable names must follow prometheus conventions");
            }

            return new LabelledGaugeBuilder(registry, help, name, labelNames);
        }

        public IGaugeBuilder Name(string name)
        {
            if (!IsValidName(name))
            {
                throw new ArgumentException("Name must follow prometheus conventions");
            }

            this.name = name;

            return this;
        }

        public IGauge Register()
        {
            if (help == null)
            {
                throw new ArgumentException("Help is required");
            }

            if (name == null)
            {
                throw new ArgumentException("Name is required");
            }

            var gauge = new Gauge(help, name);
            registry.Register(name, gauge);

            return gauge;
        }
    }
}
