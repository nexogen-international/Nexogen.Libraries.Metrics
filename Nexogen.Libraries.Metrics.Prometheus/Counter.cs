using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using static Nexogen.Libraries.Metrics.Prometheus.PrometheusConventions;

namespace Nexogen.Libraries.Metrics.Prometheus
{
    internal class Counter : ICounter, IPrometheusExposable
    {
        private AtomicDouble counter = new AtomicDouble();
        private readonly string help;
        private readonly string name;
        private readonly string[] labels;
        private readonly string[] labelNames;

        public double Value
        {
            get
            {
                return counter.Value;
            }
        }

        public Counter(string help, string name, string[] labelNames, string[] labels)
        {
            if (!IsValidName(name))
            {
                throw new ArgumentException($"Invalid metric name: {name}");
            }

            this.name = name;
            this.help = help;
            this.labels = labels;
            this.labelNames = labelNames;
        }

        public Counter(string help, string name) : this(help, name, new string[0], new string[0])
        {
        }

        public void Increment()
        {
            counter.Add(1.0);
        }

        public void Increment(double amount)
        {
            if (amount < 0.0)
            {
                throw new ArgumentException("Counter value cannot decrease");
            }

            counter.Add(amount);
        }

        public async Task ExposeText(TextWriter writer, ExposeOptions options)
        {
            string labelList = BuildLabels(labelNames, labels);

            if (labelList.Length == 0)
            {
                await writer.WriteLineAsync($"# HELP {name} {help}");
                await writer.WriteLineAsync($"# TYPE {name} counter");
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

            await writer.WriteLineAsync($"{name}{labelList} {counter.Value.ToString(CultureInfo.InvariantCulture)}{epoch}");
        }
    }
}
