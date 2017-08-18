using System;
using Xunit;
using Nexogen.Libraries.Metrics.Prometheus;

namespace Nexogen.Libraries.Metrics.UnitTests.Prometheus
{
    public class GaugeBuilderTest
    {
        [Fact]
        public void Register_requires_Name_set()
        {
            var gb = new GaugeBuilder(new PrometheusRegistry(null));

            gb.Help("This is a help message");

            Assert.Throws<ArgumentException>(() => gb.Register());
        }

        [Fact]
        public void Register_requires_Help_set()
        {
            var gb = new GaugeBuilder(new PrometheusRegistry(null));

            gb.Name("example_count");

            Assert.Throws<ArgumentException>(() => gb.Register());
        }

        [Theory]
        [InlineData("1_cannot_start_with_number")]
        [InlineData(" cannot_start_with_space")]
        [InlineData("cannot_contain space")]
        [InlineData("cannot_contain_unicode_猫")]
        [InlineData("")]
        public void Name_throws_ArgumentException_on_invalid_names(string name)
        {
            var gb = new GaugeBuilder(new PrometheusRegistry(null));

            Assert.Throws<ArgumentException>(() => gb.Name(name));
        }

        [Theory]
        [InlineData(":colon_in_every_position:")]
        [InlineData("UpperCaseIsOkToo")]
        [InlineData("numbers_12394")]
        public void Name_should_accept_well_formed_names(string name)
        {
            var gb = new GaugeBuilder(new PrometheusRegistry(null));
            gb.Name(name);
        }
    }
}
