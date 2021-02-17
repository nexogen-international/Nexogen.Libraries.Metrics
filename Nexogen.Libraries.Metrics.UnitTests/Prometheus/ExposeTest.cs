using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Xunit;
using Nexogen.Libraries.Metrics.Prometheus;

namespace Nexogen.Libraries.Metrics.UnitTests.Prometheus
{
    public class ExposeTest : IDisposable
    {
        static Encoding UTF8 = new UTF8Encoding(false);

        CultureInfo OriginalCulture { get; }
        CultureInfo OriginalUICulture { get; }

        public ExposeTest()
        {
            OriginalCulture = CultureInfo.CurrentCulture;
            OriginalUICulture = CultureInfo.CurrentUICulture;

#if !NET452
            CultureInfo.CurrentCulture = new CultureInfo("hu-HU");
            CultureInfo.CurrentUICulture = new CultureInfo("hu-HU");
#else
            CultureInfo.DefaultThreadCurrentCulture = new CultureInfo("hu-HU");
            CultureInfo.DefaultThreadCurrentUICulture = new CultureInfo("hu-HU");
#endif
        }

        public void Dispose()
        {
#if !NET452
            CultureInfo.CurrentCulture = OriginalCulture;
            CultureInfo.CurrentUICulture = OriginalUICulture;
#else
            CultureInfo.DefaultThreadCurrentCulture = OriginalCulture;
            CultureInfo.DefaultThreadCurrentUICulture = OriginalUICulture;
#endif
        }

        [Fact]
        public async Task Counter_metrics_are_exposed_correctly()
        {
            var metrics = new PrometheusMetrics();

            var counter = metrics.Counter()
                .Name("test_counter_total")
                .Help("This is the help")
                .Register();

            counter.Increment();

            var memstream = new MemoryStream();
            await metrics.Expose(memstream);

            var lines = UTF8.GetString(memstream.ToArray()).Split('\n');

            Assert.Equal(1, lines.Count(s => s == "# HELP test_counter_total This is the help"));
            Assert.Equal(1, lines.Count(s => s == "# TYPE test_counter_total counter"));
            Assert.Equal(1, lines.Count(s => Regex.IsMatch(s, "test_counter_total 1 [0-9]+")));
        }

        [Fact]
        public async Task Counter_metrics_are_exposed_correctly_when_requested_without_timestamp()
        {
            var metrics = new PrometheusMetrics();

            var counter = metrics.Counter()
                .Name("test_counter_total")
                .Help("This is the help")
                .Register();

            counter.Increment();

            var memstream = new MemoryStream();
            await metrics.Expose(memstream, ExposeOptions.NoTimestamp);

            var lines = UTF8.GetString(memstream.ToArray()).Split('\n');

            Assert.Equal(1, lines.Count(s => s == "# HELP test_counter_total This is the help"));
            Assert.Equal(1, lines.Count(s => s == "# TYPE test_counter_total counter"));
            Assert.Equal(1, lines.Count(s => Regex.IsMatch(s, "test_counter_total 1")));
        }

        [Fact]
        public async Task Gauge_metrics_are_exposed_correctly()
        {
            var metrics = new PrometheusMetrics();

            var gauge = metrics.Gauge()
                .Name("test_gauge_total")
                .Help("This is the help")
                .Register();

            gauge.Value = 9374;

            var memstream = new MemoryStream();
            await metrics.Expose(memstream);

            var lines = UTF8.GetString(memstream.ToArray()).Split('\n');

            Assert.Equal(1, lines.Count(s => s == "# HELP test_gauge_total This is the help"));
            Assert.Equal(1, lines.Count(s => s == "# TYPE test_gauge_total gauge"));
            Assert.Equal(1, lines.Count(s => Regex.IsMatch(s, "test_gauge_total 9374 [0-9]+")));
        }

        [Fact]
        public async Task Gauge_metrics_are_exposed_correctly_when_requested_without_timestamp()
        {
            var metrics = new PrometheusMetrics();

            var gauge = metrics.Gauge()
                .Name("test_gauge_total")
                .Help("This is the help")
                .Register();

            gauge.Value = 23525.5;

            var memstream = new MemoryStream();
            await metrics.Expose(memstream, ExposeOptions.NoTimestamp);

            var lines = UTF8.GetString(memstream.ToArray()).Split('\n');

            Assert.Equal(1, lines.Count(s => s == "# HELP test_gauge_total This is the help"));
            Assert.Equal(1, lines.Count(s => s == "# TYPE test_gauge_total gauge"));
            Assert.Equal(1, lines.Count(s => Regex.IsMatch(s, "test_gauge_total 23525.5")));
        }

        [Fact]
        public async Task LinearHistogram_metrics_are_exposed_correctly()
        {
            var metrics = new PrometheusMetrics();

            var histogram = metrics.Histogram()
                .LinearBuckets(-10, 10, 12)
                .Name("test_histogram")
                .Help("This is the help")
                .Register();

            histogram.Observe(21);

            var memstream = new MemoryStream();
            await metrics.Expose(memstream);

            var lines = UTF8.GetString(memstream.ToArray()).Split('\n');

            Assert.Equal(1, lines.Count(s => s == "# HELP test_histogram This is the help"));
            Assert.Equal(1, lines.Count(s => s == "# TYPE test_histogram histogram"));
            Assert.Equal(1, lines.Count(s => s == "test_histogram_bucket{le=\"-10\"} 0"));
            Assert.Equal(1, lines.Count(s => s == "test_histogram_bucket{le=\"0\"} 0"));
            Assert.Equal(1, lines.Count(s => s == "test_histogram_bucket{le=\"10\"} 0"));
            Assert.Equal(1, lines.Count(s => s == "test_histogram_bucket{le=\"20\"} 0"));
            Assert.Equal(1, lines.Count(s => s == "test_histogram_bucket{le=\"30\"} 1"));
            Assert.Equal(1, lines.Count(s => s == "test_histogram_bucket{le=\"40\"} 1"));
            Assert.Equal(1, lines.Count(s => s == "test_histogram_bucket{le=\"50\"} 1"));
            Assert.Equal(1, lines.Count(s => s == "test_histogram_bucket{le=\"60\"} 1"));
            Assert.Equal(1, lines.Count(s => s == "test_histogram_bucket{le=\"70\"} 1"));
            Assert.Equal(1, lines.Count(s => s == "test_histogram_bucket{le=\"80\"} 1"));
            Assert.Equal(1, lines.Count(s => s == "test_histogram_bucket{le=\"90\"} 1"));
            Assert.Equal(1, lines.Count(s => s == "test_histogram_bucket{le=\"100\"} 1"));
            Assert.Equal(1, lines.Count(s => s == "test_histogram_bucket{le=\"+Inf\"} 1"));
            Assert.Equal(1, lines.Count(s => s == "test_histogram_sum 21"));
            Assert.Equal(1, lines.Count(s => s == "test_histogram_count 1"));

            histogram.Observe(51);

            memstream = new MemoryStream();
            await metrics.Expose(memstream);

            lines = UTF8.GetString(memstream.ToArray()).Split('\n');

            Assert.Equal(1, lines.Count(s => s == "# HELP test_histogram This is the help"));
            Assert.Equal(1, lines.Count(s => s == "# TYPE test_histogram histogram"));
            Assert.Equal(1, lines.Count(s => s == "test_histogram_bucket{le=\"-10\"} 0"));
            Assert.Equal(1, lines.Count(s => s == "test_histogram_bucket{le=\"0\"} 0"));
            Assert.Equal(1, lines.Count(s => s == "test_histogram_bucket{le=\"10\"} 0"));
            Assert.Equal(1, lines.Count(s => s == "test_histogram_bucket{le=\"20\"} 0"));
            Assert.Equal(1, lines.Count(s => s == "test_histogram_bucket{le=\"30\"} 1"));
            Assert.Equal(1, lines.Count(s => s == "test_histogram_bucket{le=\"40\"} 1"));
            Assert.Equal(1, lines.Count(s => s == "test_histogram_bucket{le=\"50\"} 1"));
            Assert.Equal(1, lines.Count(s => s == "test_histogram_bucket{le=\"60\"} 2"));
            Assert.Equal(1, lines.Count(s => s == "test_histogram_bucket{le=\"70\"} 2"));
            Assert.Equal(1, lines.Count(s => s == "test_histogram_bucket{le=\"80\"} 2"));
            Assert.Equal(1, lines.Count(s => s == "test_histogram_bucket{le=\"90\"} 2"));
            Assert.Equal(1, lines.Count(s => s == "test_histogram_bucket{le=\"100\"} 2"));
            Assert.Equal(1, lines.Count(s => s == "test_histogram_bucket{le=\"+Inf\"} 2"));
            Assert.Equal(1, lines.Count(s => s == "test_histogram_sum 72"));
            Assert.Equal(1, lines.Count(s => s == "test_histogram_count 2"));
        }

        [Fact]
        public async Task ExponentialHistogram_metrics_are_exposed_correctly()
        {
            var metrics = new PrometheusMetrics();

            var histogram = metrics.Histogram()
                .ExponentialBuckets(1, 2, 8)
                .Name("test_histogram")
                .Help("This is the help")
                .Register();

            histogram.Observe(21);

            var memstream = new MemoryStream();
            await metrics.Expose(memstream);

            var lines = UTF8.GetString(memstream.ToArray()).Split('\n');

            Assert.Equal(1, lines.Count(s => s == "# HELP test_histogram This is the help"));
            Assert.Equal(1, lines.Count(s => s == "# TYPE test_histogram histogram"));
            Assert.Equal(1, lines.Count(s => s == "test_histogram_bucket{le=\"1\"} 0"));
            Assert.Equal(1, lines.Count(s => s == "test_histogram_bucket{le=\"2\"} 0"));
            Assert.Equal(1, lines.Count(s => s == "test_histogram_bucket{le=\"4\"} 0"));
            Assert.Equal(1, lines.Count(s => s == "test_histogram_bucket{le=\"8\"} 0"));
            Assert.Equal(1, lines.Count(s => s == "test_histogram_bucket{le=\"16\"} 0"));
            Assert.Equal(1, lines.Count(s => s == "test_histogram_bucket{le=\"32\"} 1"));
            Assert.Equal(1, lines.Count(s => s == "test_histogram_bucket{le=\"64\"} 1"));
            Assert.Equal(1, lines.Count(s => s == "test_histogram_bucket{le=\"128\"} 1"));
            Assert.Equal(1, lines.Count(s => s == "test_histogram_bucket{le=\"+Inf\"} 1"));
            Assert.Equal(1, lines.Count(s => s == "test_histogram_sum 21"));
            Assert.Equal(1, lines.Count(s => s == "test_histogram_count 1"));

            histogram.Observe(51);

            memstream = new MemoryStream();
            await metrics.Expose(memstream);

            lines = UTF8.GetString(memstream.ToArray()).Split('\n');

            Assert.Equal(1, lines.Count(s => s == "# HELP test_histogram This is the help"));
            Assert.Equal(1, lines.Count(s => s == "# TYPE test_histogram histogram"));
            Assert.Equal(1, lines.Count(s => s == "test_histogram_bucket{le=\"1\"} 0"));
            Assert.Equal(1, lines.Count(s => s == "test_histogram_bucket{le=\"2\"} 0"));
            Assert.Equal(1, lines.Count(s => s == "test_histogram_bucket{le=\"4\"} 0"));
            Assert.Equal(1, lines.Count(s => s == "test_histogram_bucket{le=\"8\"} 0"));
            Assert.Equal(1, lines.Count(s => s == "test_histogram_bucket{le=\"16\"} 0"));
            Assert.Equal(1, lines.Count(s => s == "test_histogram_bucket{le=\"32\"} 1"));
            Assert.Equal(1, lines.Count(s => s == "test_histogram_bucket{le=\"64\"} 2"));
            Assert.Equal(1, lines.Count(s => s == "test_histogram_bucket{le=\"128\"} 2"));
            Assert.Equal(1, lines.Count(s => s == "test_histogram_bucket{le=\"+Inf\"} 2"));
            Assert.Equal(1, lines.Count(s => s == "test_histogram_sum 72"));
            Assert.Equal(1, lines.Count(s => s == "test_histogram_count 2"));
        }

        [Fact]
        public async Task CustomHistogram_metrics_are_exposed_correctly()
        {
            var metrics = new PrometheusMetrics();

            var histogram = metrics.Histogram()
                .Buckets(2, 3, 4, 12, 31, 243)
                .Name("test_histogram")
                .Help("This is the help")
                .Register();

            histogram.Observe(21);

            var memstream = new MemoryStream();
            await metrics.Expose(memstream);

            var lines = UTF8.GetString(memstream.ToArray()).Split('\n');

            Assert.Equal(1, lines.Count(s => s == "# HELP test_histogram This is the help"));
            Assert.Equal(1, lines.Count(s => s == "# TYPE test_histogram histogram"));
            Assert.Equal(1, lines.Count(s => s == "test_histogram_bucket{le=\"3\"} 0"));
            Assert.Equal(1, lines.Count(s => s == "test_histogram_bucket{le=\"4\"} 0"));
            Assert.Equal(1, lines.Count(s => s == "test_histogram_bucket{le=\"12\"} 0"));
            Assert.Equal(1, lines.Count(s => s == "test_histogram_bucket{le=\"31\"} 1"));
            Assert.Equal(1, lines.Count(s => s == "test_histogram_bucket{le=\"243\"} 1"));
            Assert.Equal(1, lines.Count(s => s == "test_histogram_bucket{le=\"+Inf\"} 1"));
            Assert.Equal(1, lines.Count(s => s == "test_histogram_sum 21"));
            Assert.Equal(1, lines.Count(s => s == "test_histogram_count 1"));

            histogram.Observe(51);

            memstream = new MemoryStream();
            await metrics.Expose(memstream);

            lines = UTF8.GetString(memstream.ToArray()).Split('\n');

            Assert.Equal(1, lines.Count(s => s == "# HELP test_histogram This is the help"));
            Assert.Equal(1, lines.Count(s => s == "# TYPE test_histogram histogram"));
            Assert.Equal(1, lines.Count(s => s == "test_histogram_bucket{le=\"3\"} 0"));
            Assert.Equal(1, lines.Count(s => s == "test_histogram_bucket{le=\"4\"} 0"));
            Assert.Equal(1, lines.Count(s => s == "test_histogram_bucket{le=\"12\"} 0"));
            Assert.Equal(1, lines.Count(s => s == "test_histogram_bucket{le=\"31\"} 1"));
            Assert.Equal(1, lines.Count(s => s == "test_histogram_bucket{le=\"243\"} 2"));
            Assert.Equal(1, lines.Count(s => s == "test_histogram_bucket{le=\"+Inf\"} 2"));
            Assert.Equal(1, lines.Count(s => s == "test_histogram_sum 72"));
            Assert.Equal(1, lines.Count(s => s == "test_histogram_count 2"));
        }

        [Fact]
        public async Task LabelledCounter_metrics_are_exposed_correctly()
        {
            var metrics = new PrometheusMetrics();

            var counter = metrics.Counter()
                .Name("test_counter_total")
                .Help("This is the help")
                .LabelNames("method")
                .Register();

            counter.Labels("GET").Increment();
            counter.Labels("POST").Increment();
            counter.Labels("POST").Increment();

            var memstream = new MemoryStream();
            await metrics.Expose(memstream);

            var lines = UTF8.GetString(memstream.ToArray()).Split('\n');

            Assert.Equal(1, lines.Count(s => s == "# HELP test_counter_total This is the help"));
            Assert.Equal(1, lines.Count(s => s == "# TYPE test_counter_total counter"));
            Assert.Equal(1, lines.Count(s => Regex.IsMatch(s, "test_counter_total{method=\"GET\"} 1 [0-9]+")));
            Assert.Equal(1, lines.Count(s => Regex.IsMatch(s, "test_counter_total{method=\"POST\"} 2 [0-9]+")));
        }

        [Fact]
        public async Task LabelledGauge_metrics_are_exposed_correctly()
        {
            var metrics = new PrometheusMetrics();

            var gauge = metrics.Gauge()
                .Name("test_gauge_total")
                .Help("This is the help")
                .LabelNames("method")
                .Register();

            gauge.Labels("GET").Value = 974;
            gauge.Labels("POST").Value = 823;

            var memstream = new MemoryStream();
            await metrics.Expose(memstream);

            var lines = UTF8.GetString(memstream.ToArray()).Split('\n');

            Assert.Equal(1, lines.Count(s => s == "# HELP test_gauge_total This is the help"));
            Assert.Equal(1, lines.Count(s => s == "# TYPE test_gauge_total gauge"));
            Assert.Equal(1, lines.Count(s => Regex.IsMatch(s, "test_gauge_total{method=\"GET\"} 974 [0-9]+")));
            Assert.Equal(1, lines.Count(s => Regex.IsMatch(s, "test_gauge_total{method=\"POST\"} 823 [0-9]+")));
        }


        [Fact]
        public async Task LabelledGauge_replacevalue_replaces_obsoleted_values_and_new_values_are_exposed_correctly()
        {
            var metrics = new PrometheusMetrics();

            var gauge = metrics.Gauge()
                .Name("test_gauge_total")
                .Help("This is the help")
                .LabelNames("method")
                .Register();

            gauge.Labels("GET").Value = 974;
            gauge.Labels("POST").Value = 823;

            gauge.ReplaceMetricValues(new Dictionary<string[], double>()
            {
                { new[] { "POST" }, 151 },
                { new[] { "HEAD" }, 673 },
            });

            var memstream = new MemoryStream();
            await metrics.Expose(memstream);

            var lines = UTF8.GetString(memstream.ToArray()).Split('\n');

            Assert.Equal(1, lines.Count(s => s == "# HELP test_gauge_total This is the help"));
            Assert.Equal(1, lines.Count(s => s == "# TYPE test_gauge_total gauge"));
            Assert.Equal(1, lines.Count(s => Regex.IsMatch(s, "test_gauge_total{method=\"POST\"} 151 [0-9]+")));
            Assert.Equal(1, lines.Count(s => Regex.IsMatch(s, "test_gauge_total{method=\"HEAD\"} 673 [0-9]+")));

            Assert.Equal(0, lines.Count(s => Regex.IsMatch(s, "test_gauge_total{method=\"GET\"} 974 [0-9]+")));        
        }

        [Fact]
        public async Task LabelledLinearHistogram_metrics_are_exposed_correctly()
        {
            var metrics = new PrometheusMetrics();

            var histogram = metrics.Histogram()
                .LinearBuckets(-10, 10, 12)
                .Name("test_histogram")
                .Help("This is the help")
                .LabelNames("method")
                .Register();

            histogram.Labels("GET").Observe(11);
            histogram.Labels("POST").Observe(21);

            var memstream = new MemoryStream();
            await metrics.Expose(memstream);

            var lines = UTF8.GetString(memstream.ToArray()).Split('\n');

            Assert.Equal(1, lines.Count(s => s == "# HELP test_histogram This is the help"));
            Assert.Equal(1, lines.Count(s => s == "# TYPE test_histogram histogram"));
            Assert.Equal(1, lines.Count(s => s == "test_histogram_bucket{method=\"GET\", le=\"-10\"} 0"));
            Assert.Equal(1, lines.Count(s => s == "test_histogram_bucket{method=\"GET\", le=\"0\"} 0"));
            Assert.Equal(1, lines.Count(s => s == "test_histogram_bucket{method=\"GET\", le=\"10\"} 0"));
            Assert.Equal(1, lines.Count(s => s == "test_histogram_bucket{method=\"GET\", le=\"20\"} 1"));
            Assert.Equal(1, lines.Count(s => s == "test_histogram_bucket{method=\"GET\", le=\"30\"} 1"));
            Assert.Equal(1, lines.Count(s => s == "test_histogram_bucket{method=\"GET\", le=\"40\"} 1"));
            Assert.Equal(1, lines.Count(s => s == "test_histogram_bucket{method=\"GET\", le=\"50\"} 1"));
            Assert.Equal(1, lines.Count(s => s == "test_histogram_bucket{method=\"GET\", le=\"60\"} 1"));
            Assert.Equal(1, lines.Count(s => s == "test_histogram_bucket{method=\"GET\", le=\"70\"} 1"));
            Assert.Equal(1, lines.Count(s => s == "test_histogram_bucket{method=\"GET\", le=\"80\"} 1"));
            Assert.Equal(1, lines.Count(s => s == "test_histogram_bucket{method=\"GET\", le=\"90\"} 1"));
            Assert.Equal(1, lines.Count(s => s == "test_histogram_bucket{method=\"GET\", le=\"100\"} 1"));
            Assert.Equal(1, lines.Count(s => s == "test_histogram_bucket{method=\"GET\", le=\"+Inf\"} 1"));
            Assert.Equal(1, lines.Count(s => s == "test_histogram_sum{method=\"GET\"} 11"));
            Assert.Equal(1, lines.Count(s => s == "test_histogram_count{method=\"GET\"} 1"));

            Assert.Equal(1, lines.Count(s => s == "# HELP test_histogram This is the help"));
            Assert.Equal(1, lines.Count(s => s == "# TYPE test_histogram histogram"));
            Assert.Equal(1, lines.Count(s => s == "test_histogram_bucket{method=\"POST\", le=\"0\"} 0"));
            Assert.Equal(1, lines.Count(s => s == "test_histogram_bucket{method=\"POST\", le=\"10\"} 0"));
            Assert.Equal(1, lines.Count(s => s == "test_histogram_bucket{method=\"POST\", le=\"20\"} 0"));
            Assert.Equal(1, lines.Count(s => s == "test_histogram_bucket{method=\"POST\", le=\"30\"} 1"));
            Assert.Equal(1, lines.Count(s => s == "test_histogram_bucket{method=\"POST\", le=\"40\"} 1"));
            Assert.Equal(1, lines.Count(s => s == "test_histogram_bucket{method=\"POST\", le=\"50\"} 1"));
            Assert.Equal(1, lines.Count(s => s == "test_histogram_bucket{method=\"POST\", le=\"60\"} 1"));
            Assert.Equal(1, lines.Count(s => s == "test_histogram_bucket{method=\"POST\", le=\"70\"} 1"));
            Assert.Equal(1, lines.Count(s => s == "test_histogram_bucket{method=\"POST\", le=\"80\"} 1"));
            Assert.Equal(1, lines.Count(s => s == "test_histogram_bucket{method=\"POST\", le=\"90\"} 1"));
            Assert.Equal(1, lines.Count(s => s == "test_histogram_bucket{method=\"POST\", le=\"100\"} 1"));
            Assert.Equal(1, lines.Count(s => s == "test_histogram_bucket{method=\"POST\", le=\"+Inf\"} 1"));
            Assert.Equal(1, lines.Count(s => s == "test_histogram_sum{method=\"POST\"} 21"));
            Assert.Equal(1, lines.Count(s => s == "test_histogram_count{method=\"POST\"} 1"));

            histogram.Labels("GET").Observe(51);
            histogram.Labels("POST").Observe(61);

            memstream = new MemoryStream();
            await metrics.Expose(memstream);

            lines = UTF8.GetString(memstream.ToArray()).Split('\n');

            Assert.Equal(1, lines.Count(s => s == "# HELP test_histogram This is the help"));
            Assert.Equal(1, lines.Count(s => s == "# TYPE test_histogram histogram"));
            Assert.Equal(1, lines.Count(s => s == "test_histogram_bucket{method=\"GET\", le=\"0\"} 0"));
            Assert.Equal(1, lines.Count(s => s == "test_histogram_bucket{method=\"GET\", le=\"10\"} 0"));
            Assert.Equal(1, lines.Count(s => s == "test_histogram_bucket{method=\"GET\", le=\"20\"} 1"));
            Assert.Equal(1, lines.Count(s => s == "test_histogram_bucket{method=\"GET\", le=\"30\"} 1"));
            Assert.Equal(1, lines.Count(s => s == "test_histogram_bucket{method=\"GET\", le=\"40\"} 1"));
            Assert.Equal(1, lines.Count(s => s == "test_histogram_bucket{method=\"GET\", le=\"50\"} 1"));
            Assert.Equal(1, lines.Count(s => s == "test_histogram_bucket{method=\"GET\", le=\"60\"} 2"));
            Assert.Equal(1, lines.Count(s => s == "test_histogram_bucket{method=\"GET\", le=\"70\"} 2"));
            Assert.Equal(1, lines.Count(s => s == "test_histogram_bucket{method=\"GET\", le=\"80\"} 2"));
            Assert.Equal(1, lines.Count(s => s == "test_histogram_bucket{method=\"GET\", le=\"90\"} 2"));
            Assert.Equal(1, lines.Count(s => s == "test_histogram_bucket{method=\"GET\", le=\"100\"} 2"));
            Assert.Equal(1, lines.Count(s => s == "test_histogram_bucket{method=\"GET\", le=\"+Inf\"} 2"));
            Assert.Equal(1, lines.Count(s => s == "test_histogram_sum{method=\"GET\"} 62"));
            Assert.Equal(1, lines.Count(s => s == "test_histogram_count{method=\"GET\"} 2"));

            Assert.Equal(1, lines.Count(s => s == "# HELP test_histogram This is the help"));
            Assert.Equal(1, lines.Count(s => s == "# TYPE test_histogram histogram"));
            Assert.Equal(1, lines.Count(s => s == "test_histogram_bucket{method=\"POST\", le=\"0\"} 0"));
            Assert.Equal(1, lines.Count(s => s == "test_histogram_bucket{method=\"POST\", le=\"10\"} 0"));
            Assert.Equal(1, lines.Count(s => s == "test_histogram_bucket{method=\"POST\", le=\"20\"} 0"));
            Assert.Equal(1, lines.Count(s => s == "test_histogram_bucket{method=\"POST\", le=\"30\"} 1"));
            Assert.Equal(1, lines.Count(s => s == "test_histogram_bucket{method=\"POST\", le=\"40\"} 1"));
            Assert.Equal(1, lines.Count(s => s == "test_histogram_bucket{method=\"POST\", le=\"50\"} 1"));
            Assert.Equal(1, lines.Count(s => s == "test_histogram_bucket{method=\"POST\", le=\"60\"} 1"));
            Assert.Equal(1, lines.Count(s => s == "test_histogram_bucket{method=\"POST\", le=\"70\"} 2"));
            Assert.Equal(1, lines.Count(s => s == "test_histogram_bucket{method=\"POST\", le=\"80\"} 2"));
            Assert.Equal(1, lines.Count(s => s == "test_histogram_bucket{method=\"POST\", le=\"90\"} 2"));
            Assert.Equal(1, lines.Count(s => s == "test_histogram_bucket{method=\"POST\", le=\"100\"} 2"));
            Assert.Equal(1, lines.Count(s => s == "test_histogram_bucket{method=\"POST\", le=\"+Inf\"} 2"));
            Assert.Equal(1, lines.Count(s => s == "test_histogram_sum{method=\"POST\"} 82"));
            Assert.Equal(1, lines.Count(s => s == "test_histogram_count{method=\"POST\"} 2"));
        }

        [Fact]
        public async Task LabelledLogarithmicHistogram_metrics_are_exposed_correctly()
        {
            var metrics = new PrometheusMetrics();

            var histogram = metrics.Histogram()
                .ExponentialBuckets(1, 2, 8)
                .Name("test_histogram")
                .Help("This is the help")
                .LabelNames("method")
                .Register();

            histogram.Labels("GET").Observe(11);
            histogram.Labels("POST").Observe(21);

            var memstream = new MemoryStream();
            await metrics.Expose(memstream);

            var lines = UTF8.GetString(memstream.ToArray()).Split('\n');

            Assert.Equal(1, lines.Count(s => s == "# HELP test_histogram This is the help"));
            Assert.Equal(1, lines.Count(s => s == "# TYPE test_histogram histogram"));
            Assert.Equal(1, lines.Count(s => s == "test_histogram_bucket{method=\"GET\", le=\"1\"} 0"));
            Assert.Equal(1, lines.Count(s => s == "test_histogram_bucket{method=\"GET\", le=\"2\"} 0"));
            Assert.Equal(1, lines.Count(s => s == "test_histogram_bucket{method=\"GET\", le=\"4\"} 0"));
            Assert.Equal(1, lines.Count(s => s == "test_histogram_bucket{method=\"GET\", le=\"8\"} 0"));
            Assert.Equal(1, lines.Count(s => s == "test_histogram_bucket{method=\"GET\", le=\"16\"} 1"));
            Assert.Equal(1, lines.Count(s => s == "test_histogram_bucket{method=\"GET\", le=\"32\"} 1"));
            Assert.Equal(1, lines.Count(s => s == "test_histogram_bucket{method=\"GET\", le=\"64\"} 1"));
            Assert.Equal(1, lines.Count(s => s == "test_histogram_bucket{method=\"GET\", le=\"128\"} 1"));
            Assert.Equal(1, lines.Count(s => s == "test_histogram_bucket{method=\"GET\", le=\"+Inf\"} 1"));
            Assert.Equal(1, lines.Count(s => s == "test_histogram_sum{method=\"GET\"} 11"));
            Assert.Equal(1, lines.Count(s => s == "test_histogram_count{method=\"GET\"} 1"));

            Assert.Equal(1, lines.Count(s => s == "# HELP test_histogram This is the help"));
            Assert.Equal(1, lines.Count(s => s == "# TYPE test_histogram histogram"));
            Assert.Equal(1, lines.Count(s => s == "test_histogram_bucket{method=\"POST\", le=\"1\"} 0"));
            Assert.Equal(1, lines.Count(s => s == "test_histogram_bucket{method=\"POST\", le=\"2\"} 0"));
            Assert.Equal(1, lines.Count(s => s == "test_histogram_bucket{method=\"POST\", le=\"4\"} 0"));
            Assert.Equal(1, lines.Count(s => s == "test_histogram_bucket{method=\"POST\", le=\"8\"} 0"));
            Assert.Equal(1, lines.Count(s => s == "test_histogram_bucket{method=\"POST\", le=\"16\"} 0"));
            Assert.Equal(1, lines.Count(s => s == "test_histogram_bucket{method=\"POST\", le=\"32\"} 1"));
            Assert.Equal(1, lines.Count(s => s == "test_histogram_bucket{method=\"POST\", le=\"64\"} 1"));
            Assert.Equal(1, lines.Count(s => s == "test_histogram_bucket{method=\"POST\", le=\"128\"} 1"));
            Assert.Equal(1, lines.Count(s => s == "test_histogram_bucket{method=\"POST\", le=\"+Inf\"} 1"));
            Assert.Equal(1, lines.Count(s => s == "test_histogram_sum{method=\"POST\"} 21"));
            Assert.Equal(1, lines.Count(s => s == "test_histogram_count{method=\"POST\"} 1"));

            histogram.Labels("GET").Observe(51);
            histogram.Labels("POST").Observe(71);

            memstream = new MemoryStream();
            await metrics.Expose(memstream);

            lines = UTF8.GetString(memstream.ToArray()).Split('\n');

            Assert.Equal(1, lines.Count(s => s == "# HELP test_histogram This is the help"));
            Assert.Equal(1, lines.Count(s => s == "# TYPE test_histogram histogram"));
            Assert.Equal(1, lines.Count(s => s == "test_histogram_bucket{method=\"GET\", le=\"1\"} 0"));
            Assert.Equal(1, lines.Count(s => s == "test_histogram_bucket{method=\"GET\", le=\"2\"} 0"));
            Assert.Equal(1, lines.Count(s => s == "test_histogram_bucket{method=\"GET\", le=\"4\"} 0"));
            Assert.Equal(1, lines.Count(s => s == "test_histogram_bucket{method=\"GET\", le=\"8\"} 0"));
            Assert.Equal(1, lines.Count(s => s == "test_histogram_bucket{method=\"GET\", le=\"16\"} 1"));
            Assert.Equal(1, lines.Count(s => s == "test_histogram_bucket{method=\"GET\", le=\"32\"} 1"));
            Assert.Equal(1, lines.Count(s => s == "test_histogram_bucket{method=\"GET\", le=\"64\"} 2"));
            Assert.Equal(1, lines.Count(s => s == "test_histogram_bucket{method=\"GET\", le=\"128\"} 2"));
            Assert.Equal(1, lines.Count(s => s == "test_histogram_bucket{method=\"GET\", le=\"+Inf\"} 2"));
            Assert.Equal(1, lines.Count(s => s == "test_histogram_sum{method=\"GET\"} 62"));
            Assert.Equal(1, lines.Count(s => s == "test_histogram_count{method=\"GET\"} 2"));

            Assert.Equal(1, lines.Count(s => s == "# HELP test_histogram This is the help"));
            Assert.Equal(1, lines.Count(s => s == "# TYPE test_histogram histogram"));
            Assert.Equal(1, lines.Count(s => s == "test_histogram_bucket{method=\"POST\", le=\"1\"} 0"));
            Assert.Equal(1, lines.Count(s => s == "test_histogram_bucket{method=\"POST\", le=\"2\"} 0"));
            Assert.Equal(1, lines.Count(s => s == "test_histogram_bucket{method=\"POST\", le=\"4\"} 0"));
            Assert.Equal(1, lines.Count(s => s == "test_histogram_bucket{method=\"POST\", le=\"8\"} 0"));
            Assert.Equal(1, lines.Count(s => s == "test_histogram_bucket{method=\"POST\", le=\"16\"} 0"));
            Assert.Equal(1, lines.Count(s => s == "test_histogram_bucket{method=\"POST\", le=\"32\"} 1"));
            Assert.Equal(1, lines.Count(s => s == "test_histogram_bucket{method=\"POST\", le=\"64\"} 1"));
            Assert.Equal(1, lines.Count(s => s == "test_histogram_bucket{method=\"POST\", le=\"128\"} 2"));
            Assert.Equal(1, lines.Count(s => s == "test_histogram_bucket{method=\"POST\", le=\"+Inf\"} 2"));
            Assert.Equal(1, lines.Count(s => s == "test_histogram_sum{method=\"POST\"} 92"));
            Assert.Equal(1, lines.Count(s => s == "test_histogram_count{method=\"POST\"} 2"));
        }

        [Fact]
        public async Task LabelledCustomHistogram_metrics_are_exposed_correctly()
        {
            var metrics = new PrometheusMetrics();

            var histogram = metrics.Histogram()
                .Buckets(2, 3, 4, 12, 31, 243)
                .Name("test_histogram")
                .Help("This is the help")
                .LabelNames("method")
                .Register();

            histogram.Labels("GET").Observe(11);
            histogram.Labels("POST").Observe(21);

            var memstream = new MemoryStream();
            await metrics.Expose(memstream);

            var lines = UTF8.GetString(memstream.ToArray()).Split('\n');

            Assert.Equal(1, lines.Count(s => s == "# HELP test_histogram This is the help"));
            Assert.Equal(1, lines.Count(s => s == "# TYPE test_histogram histogram"));
            Assert.Equal(1, lines.Count(s => s == "test_histogram_bucket{method=\"GET\", le=\"3\"} 0"));
            Assert.Equal(1, lines.Count(s => s == "test_histogram_bucket{method=\"GET\", le=\"4\"} 0"));
            Assert.Equal(1, lines.Count(s => s == "test_histogram_bucket{method=\"GET\", le=\"12\"} 1"));
            Assert.Equal(1, lines.Count(s => s == "test_histogram_bucket{method=\"GET\", le=\"31\"} 1"));
            Assert.Equal(1, lines.Count(s => s == "test_histogram_bucket{method=\"GET\", le=\"243\"} 1"));
            Assert.Equal(1, lines.Count(s => s == "test_histogram_bucket{method=\"GET\", le=\"+Inf\"} 1"));
            Assert.Equal(1, lines.Count(s => s == "test_histogram_sum{method=\"GET\"} 11"));
            Assert.Equal(1, lines.Count(s => s == "test_histogram_count{method=\"GET\"} 1"));

            Assert.Equal(1, lines.Count(s => s == "# HELP test_histogram This is the help"));
            Assert.Equal(1, lines.Count(s => s == "# TYPE test_histogram histogram"));
            Assert.Equal(1, lines.Count(s => s == "test_histogram_bucket{method=\"POST\", le=\"3\"} 0"));
            Assert.Equal(1, lines.Count(s => s == "test_histogram_bucket{method=\"POST\", le=\"4\"} 0"));
            Assert.Equal(1, lines.Count(s => s == "test_histogram_bucket{method=\"POST\", le=\"12\"} 0"));
            Assert.Equal(1, lines.Count(s => s == "test_histogram_bucket{method=\"POST\", le=\"31\"} 1"));
            Assert.Equal(1, lines.Count(s => s == "test_histogram_bucket{method=\"POST\", le=\"243\"} 1"));
            Assert.Equal(1, lines.Count(s => s == "test_histogram_bucket{method=\"POST\", le=\"+Inf\"} 1"));
            Assert.Equal(1, lines.Count(s => s == "test_histogram_sum{method=\"POST\"} 21"));
            Assert.Equal(1, lines.Count(s => s == "test_histogram_count{method=\"POST\"} 1"));

            histogram.Labels("GET").Observe(51);
            histogram.Labels("POST").Observe(61);

            memstream = new MemoryStream();
            await metrics.Expose(memstream);

            lines = UTF8.GetString(memstream.ToArray()).Split('\n');

            Assert.Equal(1, lines.Count(s => s == "# HELP test_histogram This is the help"));
            Assert.Equal(1, lines.Count(s => s == "# TYPE test_histogram histogram"));
            Assert.Equal(1, lines.Count(s => s == "test_histogram_bucket{method=\"GET\", le=\"3\"} 0"));
            Assert.Equal(1, lines.Count(s => s == "test_histogram_bucket{method=\"GET\", le=\"4\"} 0"));
            Assert.Equal(1, lines.Count(s => s == "test_histogram_bucket{method=\"GET\", le=\"12\"} 1"));
            Assert.Equal(1, lines.Count(s => s == "test_histogram_bucket{method=\"GET\", le=\"31\"} 1"));
            Assert.Equal(1, lines.Count(s => s == "test_histogram_bucket{method=\"GET\", le=\"243\"} 2"));
            Assert.Equal(1, lines.Count(s => s == "test_histogram_bucket{method=\"GET\", le=\"+Inf\"} 2"));
            Assert.Equal(1, lines.Count(s => s == "test_histogram_sum{method=\"GET\"} 62"));
            Assert.Equal(1, lines.Count(s => s == "test_histogram_count{method=\"GET\"} 2"));

            Assert.Equal(1, lines.Count(s => s == "# HELP test_histogram This is the help"));
            Assert.Equal(1, lines.Count(s => s == "# TYPE test_histogram histogram"));
            Assert.Equal(1, lines.Count(s => s == "test_histogram_bucket{method=\"POST\", le=\"3\"} 0"));
            Assert.Equal(1, lines.Count(s => s == "test_histogram_bucket{method=\"POST\", le=\"4\"} 0"));
            Assert.Equal(1, lines.Count(s => s == "test_histogram_bucket{method=\"POST\", le=\"12\"} 0"));
            Assert.Equal(1, lines.Count(s => s == "test_histogram_bucket{method=\"POST\", le=\"31\"} 1"));
            Assert.Equal(1, lines.Count(s => s == "test_histogram_bucket{method=\"POST\", le=\"243\"} 2"));
            Assert.Equal(1, lines.Count(s => s == "test_histogram_bucket{method=\"POST\", le=\"+Inf\"} 2"));
            Assert.Equal(1, lines.Count(s => s == "test_histogram_sum{method=\"POST\"} 82"));
            Assert.Equal(1, lines.Count(s => s == "test_histogram_count{method=\"POST\"} 2"));
        }

        [Fact]
        public async Task Counter_metrics_are_exposed_correctly_in_cultures_with_non_point_decimal_separator()
        {
            var metrics = new PrometheusMetrics();

            var counter = metrics.Counter()
                               .Name("test_counter_total")
                               .Help("Quis autem vel eum iure reprehenderit qui in ea voluptate velit esse quam nihil molestiae consequatur")
                               .Register();

            counter.Increment(3.14159);

            var memstream = new MemoryStream();
            await metrics.Expose(memstream);

            var lines = UTF8.GetString(memstream.ToArray()).Split('\n');

            Assert.Equal(1, lines.Count(s => s == "# HELP test_counter_total Quis autem vel eum iure reprehenderit qui in ea voluptate velit esse quam nihil molestiae consequatur"));
            Assert.Equal(1, lines.Count(s => s == "# TYPE test_counter_total counter"));
            Assert.Equal(1, lines.Count(s => Regex.IsMatch(s, "test_counter_total 3.14159 [0-9]+")));
        }

        [Fact]
        public async Task LabelledCounter_metrics_are_exposed_correctly_in_cultures_with_non_point_decimal_separator()
        {
            var metrics = new PrometheusMetrics();

            var counter = metrics.Counter()
                               .Name("test_labelled_counter_total")
                               .LabelNames("consectetur")
                               .Help("Öt szép szűzlány őrült írót nyúz, avagy kínaiul: 五美麗的處女瘋狂作家去皮")
                               .Register();

            counter.Labels("adipiscing").Increment(2.71828);

            var memstream = new MemoryStream();
            await metrics.Expose(memstream);

            var lines = UTF8.GetString(memstream.ToArray()).Split('\n');

            Assert.Equal(1, lines.Count(s => s == "# HELP test_labelled_counter_total Öt szép szűzlány őrült írót nyúz, avagy kínaiul: 五美麗的處女瘋狂作家去皮"));
            Assert.Equal(1, lines.Count(s => s == "# TYPE test_labelled_counter_total counter"));
            Assert.Equal(1, lines.Count(s => Regex.IsMatch(s, "test_labelled_counter_total{consectetur=\"adipiscing\"} 2.71828 [0-9]+")));
        }

        [Fact]
        public async Task Gauge_metrics_are_exposed_correctly_in_cultures_with_non_point_decimal_separator()
        {
            var metrics = new PrometheusMetrics();

            var gauge = metrics.Gauge()
                               .Name("test_gauge_total")
                               .Help("Duis aute irure dolor in reprehenderit in voluptate velit esse")
                               .Register();

            gauge.Value = 3.14159;

            var memstream = new MemoryStream();
            await metrics.Expose(memstream);

            var lines = UTF8.GetString(memstream.ToArray()).Split('\n');

            Assert.Equal(1, lines.Count(s => s == "# HELP test_gauge_total Duis aute irure dolor in reprehenderit in voluptate velit esse"));
            Assert.Equal(1, lines.Count(s => s == "# TYPE test_gauge_total gauge"));
            Assert.Equal(1, lines.Count(s => Regex.IsMatch(s, "test_gauge_total 3.14159 [0-9]+")));
        }

        [Fact]
        public async Task LabelledGauge_metrics_are_exposed_correctly_in_cultures_with_non_point_decimal_separator()
        {
            var metrics = new PrometheusMetrics();

            var gauge = metrics.Gauge()
                               .Name("test_labelled_gauge_total")
                               .LabelNames("lorem")
                               .Help("Sed ut perspiciatis unde omnis iste natus error sit voluptatem accusantium doloremque laudantium")
                               .Register();

            gauge.Labels("ipsum").Value = 2.71828;

            var memstream = new MemoryStream();
            await metrics.Expose(memstream);

            var lines = UTF8.GetString(memstream.ToArray()).Split('\n');

            Assert.Equal(1, lines.Count(s => s == "# HELP test_labelled_gauge_total Sed ut perspiciatis unde omnis iste natus error sit voluptatem accusantium doloremque laudantium"));
            Assert.Equal(1, lines.Count(s => s == "# TYPE test_labelled_gauge_total gauge"));
            Assert.Equal(1, lines.Count(s => Regex.IsMatch(s, "test_labelled_gauge_total{lorem=\"ipsum\"} 2.71828 [0-9]+")));
        }

        [Fact]
        public async Task Histogram_metrics_are_exposed_correctly_in_cultures_with_non_point_decimal_separator()
        {
            var metrics = new PrometheusMetrics();

            var histogram = metrics.Histogram()
                               .Name("test_histogram")
                               .Buckets(-1.1, 2.2, 3.14159, 9)
                               .Help("Nemo enim ipsam voluptatem quia voluptas sit aspernatur aut odit aut fugit")
                               .Register();

            histogram.Observe(-0.3);
            histogram.Observe(2.99);

            var memstream = new MemoryStream();
            await metrics.Expose(memstream);

            var lines = UTF8.GetString(memstream.ToArray()).Split('\n');

            Assert.Equal(1, lines.Count(s => s == "# HELP test_histogram Nemo enim ipsam voluptatem quia voluptas sit aspernatur aut odit aut fugit"));
            Assert.Equal(1, lines.Count(s => s == "# TYPE test_histogram histogram"));
            Assert.Equal(1, lines.Count(s => s == "test_histogram_bucket{le=\"-1.1\"} 0"));
            Assert.Equal(1, lines.Count(s => s == "test_histogram_bucket{le=\"2.2\"} 1"));
            Assert.Equal(1, lines.Count(s => s == "test_histogram_bucket{le=\"3.14159\"} 2"));
            Assert.Equal(1, lines.Count(s => s == "test_histogram_bucket{le=\"9\"} 2"));
            Assert.Equal(1, lines.Count(s => s == "test_histogram_bucket{le=\"+Inf\"} 2"));
            Assert.Equal(1, lines.Count(s => s == "test_histogram_sum 2.6900000000000004"));
            Assert.Equal(1, lines.Count(s => s == "test_histogram_count 2"));
        }

        [Fact]
        public async Task LabelledHistogram_metrics_are_exposed_correctly_in_cultures_with_non_point_decimal_separator()
        {
            var metrics = new PrometheusMetrics();

            var histogram = metrics.Histogram()
                               .Name("test_histogram")
                               .Buckets(-1.1, 2.2, 3.14159, 9)
                               .LabelNames("dolor")
                               .Help("Ut enim ad minima veniam, quis nostrum exercitationem ullam corporis suscipit laboriosam")
                               .Register();

            histogram.Labels("sit amet").Observe(-0.3);
            histogram.Labels("sit amet").Observe(2.99);

            var memstream = new MemoryStream();
            await metrics.Expose(memstream);

            var lines = UTF8.GetString(memstream.ToArray()).Split('\n');

            Assert.Equal(1, lines.Count(s => s == "# HELP test_histogram Ut enim ad minima veniam, quis nostrum exercitationem ullam corporis suscipit laboriosam"));
            Assert.Equal(1, lines.Count(s => s == "# TYPE test_histogram histogram"));
            Assert.Equal(1, lines.Count(s => s == "test_histogram_bucket{dolor=\"sit amet\", le=\"-1.1\"} 0"));
            Assert.Equal(1, lines.Count(s => s == "test_histogram_bucket{dolor=\"sit amet\", le=\"2.2\"} 1"));
            Assert.Equal(1, lines.Count(s => s == "test_histogram_bucket{dolor=\"sit amet\", le=\"3.14159\"} 2"));
            Assert.Equal(1, lines.Count(s => s == "test_histogram_bucket{dolor=\"sit amet\", le=\"9\"} 2"));
            Assert.Equal(1, lines.Count(s => s == "test_histogram_bucket{dolor=\"sit amet\", le=\"+Inf\"} 2"));
            Assert.Equal(1, lines.Count(s => s == "test_histogram_sum{dolor=\"sit amet\"} 2.6900000000000004"));
            Assert.Equal(1, lines.Count(s => s == "test_histogram_count{dolor=\"sit amet\"} 2"));
        }
    }
}
