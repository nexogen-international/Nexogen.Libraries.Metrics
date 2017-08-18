using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Nexogen.Libraries.Metrics.Prometheus.PrometheusConventions;

namespace Nexogen.Libraries.Metrics.Prometheus
{
    internal class Gauge : IGauge, IPrometheusExposable
    {
        private AtomicDouble gauge = new AtomicDouble();

        private readonly string help;
        private readonly string name;
        private readonly string[] labelNames;
        private readonly string[] labels;

        public Gauge(string help, string name, string[] labelNames, string[] labels)
        {
            this.help = help;
            this.name = name;
            this.labelNames = labelNames;
            this.labels = labels;
        }

        public Gauge(string help, string name) : this(help, name, new string[0], new string[0])
        {
        }

        public double Value
        {
            get
            {
                return gauge.Value;
            }

            set
            {
                gauge.Value = value;
            }
        }

        public void Decrement()
        {
            gauge.Add(-1.0);
        }

        public void Decrement(double amount)
        {
            if (amount < 0.0)
            {
                throw new ArgumentException("Gauge can only be decremented by positive numbers");
            }

            gauge.Add(-1.0 * amount);
        }

        public void Increment()
        {
            gauge.Add(1.0);
        }

        public void Increment(double amount)
        {
            if (amount < 0.0)
            {
                throw new ArgumentException("Gauge can only be incremented by positive numbers");
            }

            gauge.Add(amount);
        }

        public async Task ExposeText(TextWriter writer, ExposeOptions options)
        {
            string labelList = BuildLabels(labelNames, labels);

            if (labelList.Length == 0)
            {
                await writer.WriteLineAsync($"# HELP {name} {help}");
                await writer.WriteLineAsync($"# TYPE {name} gauge");
            }

            string epoch;
            if (!options.HasFlag(ExposeOptions.NoTimestamp))
            {
                epoch = $" {DateTimeOffset.Now.ToUnixTimeMilliseconds()}";
            }
            else
            {
                epoch = "";
            }

            await writer.WriteLineAsync($"{name}{labelList} {gauge.Value.ToString(CultureInfo.InvariantCulture)}{epoch}");
        }
    }
}
