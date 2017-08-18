using Nexogen.Libraries.Prometheus.PushGateway;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Xunit;
using FluentAssertions;
using Nexogen.Libraries.Metrics.Prometheus;

namespace Nexogen.Libraries.Metrics.UnitTests
{
    public class FakeMessageHandler : HttpMessageHandler
    {
        public string Content { get; private set; }

        protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            var response = new HttpResponseMessage();
            response.StatusCode = System.Net.HttpStatusCode.Accepted;

            Content = await request.Content.ReadAsStringAsync();

            return response;
        }
    }

    public class PushgatewayTest
    {
        [Fact]
        public async Task Exposing_a_metric_includes_custom_metrics_without_timestamp()
        {
            var mh = new FakeMessageHandler();
            using (var pgw = new PushGateway(new HttpClient(mh) { BaseAddress = new Uri("http://example.com/") }))
            {

                var m = new PrometheusMetrics();
                var c = m.Counter().Name("test_counter").Help("This is the help text").Register();

                c.Increment();

                await pgw.PushAsync(m, "exampleapp");

                mh.Content.Should().Contain("# HELP test_counter This is the help text\n");
                mh.Content.Should().Contain("test_counter 1\n");
            }
        }

        [Fact]
        public void Passing_null_to_the_constructors_throws_ArgumentNullException()
        {
            Assert.Throws<ArgumentNullException>(() => new PushGateway(null as HttpClient));
            Assert.Throws<ArgumentNullException>(() => new PushGateway(null as Uri));
        }

        [Fact]
        public void Passing_null_to_Push_throws_ArgumentNullException()
        {
            var mh = new FakeMessageHandler();
            using (var pushgateway = new PushGateway(new HttpClient(mh) { BaseAddress = new Uri("http://example.com/") }))
            {
                var m = new PrometheusMetrics();

                Assert.Throws<ArgumentNullException>(() => pushgateway.Push(null, "jobname"));
                Assert.Throws<ArgumentNullException>(() => pushgateway.Push(m, null));
            }
        }

        [Fact]
        public void Passing_null_to_PushAsync_throws_ArgumentNullException()
        {
            var mh = new FakeMessageHandler();
            using (var pushgateway = new PushGateway(new HttpClient(mh) { BaseAddress = new Uri("http://example.com/") }))
            {
                var m = new PrometheusMetrics();

                Assert.ThrowsAsync<ArgumentNullException>(() => pushgateway.PushAsync(null, "jobname"));
                Assert.ThrowsAsync<ArgumentNullException>(() => pushgateway.PushAsync(m, null));
            }
        }
    }
}
