using System;
using Xunit;
using Nexogen.Libraries.Metrics.Prometheus;

namespace Nexogen.Libraries.Metrics.UnitTests.Prometheus
{
    public class AtomicDoubleTest
    {
        [Fact]
        public void AtomicDouble_has_the_initial_value_of_zero()
        {
            AtomicDouble ad = new AtomicDouble();

            Assert.Equal(0.0, ad.Value);
        }

        [Fact]
        public void Adding_one_to_an_empty_AtomicDouble_results_in_the_value_of_one()
        {
            AtomicDouble ad = new AtomicDouble();

            ad.Add(1.0);

            Assert.Equal(1.0, ad.Value);
        }

        [Fact]
        public void Setting_the_value_of_an_AtomicDouble_results_in_the_value_set()
        {
            AtomicDouble ad = new AtomicDouble();

            ad.Value = 10.02;

            Assert.Equal(10.02, ad.Value);
        }
    }
}
