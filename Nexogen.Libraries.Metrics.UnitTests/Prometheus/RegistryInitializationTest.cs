using System.IO;
using System.Threading.Tasks;
using Xunit;
using Nexogen.Libraries.Metrics.Prometheus;

namespace Nexogen.Libraries.Metrics.UnitTests.Prometheus
{
    public class RegistryInitializationTest
    {
        private class DummyPrometheusExposable : IPrometheusExposable
        {
            public Task ExposeText(TextWriter writer, ExposeOptions options)
            {
#if !NET452
                return Task.CompletedTask;
#else
                return Task.FromResult<object>(null);
#endif
            }
        }

        [Fact]
        public void When_registering_a_metric_name_multiple_times_Then_an_exception_is_thown() {
            var registry = new PrometheusRegistry(null);

            registry.Register("name", new DummyPrometheusExposable());

            Assert.ThrowsAny<CollectorBuilderException>( () => registry.Register("name", new DummyPrometheusExposable()));
        }
    }
}
