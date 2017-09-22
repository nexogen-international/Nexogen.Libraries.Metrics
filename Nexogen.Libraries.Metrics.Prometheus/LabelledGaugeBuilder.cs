using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using static Nexogen.Libraries.Metrics.Prometheus.PrometheusConventions;

namespace Nexogen.Libraries.Metrics.Prometheus
{
    public class LabelledGaugeBuilder : ILabelledGaugeBuilder
    {
        private string help;
        private string name;
        private string[] labelNames = new string[0];
        private PrometheusRegistry registry;

        public LabelledGaugeBuilder(PrometheusRegistry registry, string help, string name, string[] labelNames)
        {
            this.help = help;
            this.name = name;
            this.labelNames = labelNames;
            this.registry = registry;
        }

        public ILabelledGaugeBuilder Help(string help)
        {
            this.help = EscapeHelp(help);

            return this;
        }

        public ILabelledGaugeBuilder LabelNames(params string[] labelNames)
        {
            if (labelNames.Any(l => !IsValidLabel(l)))
            {
                throw new ArgumentException("Label names must follow prometheus conventions");
            }

            this.labelNames = labelNames;

            return this;
        }

        public ILabelledGaugeBuilder Name(string name)
        {
            if (!IsValidName(name))
            {
                throw new ArgumentException("Name must follow prometheus conventions");
            }

            this.name = name;

            return this;
        }

        public ILabelledGauge Register()
        {
            if (help == null)
            {
                throw new ArgumentException("Help is required");
            }

            if (name == null)
            {
                throw new ArgumentException("Name is required");
            }

            var gauge = new LabelledGauge(help, name, labelNames);
            registry.Register(name, gauge);

            return gauge;
        }
    }
}
