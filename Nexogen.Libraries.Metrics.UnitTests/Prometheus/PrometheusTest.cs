using Xunit;
using Nexogen.Libraries.Metrics.Prometheus;

namespace Nexogen.Libraries.Metrics.UnitTests.Prometheus
{
    public class PrometheusTest
    {
        [Theory]
        [InlineData("9name")]
        [InlineData("")]
        public void IsValidName_should_return_false_for_invalid_names(string name)
        {
            Assert.False(PrometheusConventions.IsValidName(name));
        }

        [Theory]
        [InlineData("name")]
        [InlineData("the_name")]
        [InlineData(":name:")]
        [InlineData("name14")]
        [InlineData("__name_")]
        [InlineData("NamE")]
        public void IsValidName_should_return_true_for_valid_names(string name)
        {
            Assert.True(PrometheusConventions.IsValidName(name));
        }

        [Theory]
        [InlineData("9label")]
        [InlineData("")]
        [InlineData("this:isnotvalid")]
        [InlineData(":label")]
        [InlineData("__label")]
        [InlineData("___label")]
        public void IsValidLabel_should_return_false_for_invalid_labels(string label)
        {
            Assert.False(PrometheusConventions.IsValidLabel(label));
        }

        [Theory]
        [InlineData("name")]
        [InlineData("the_name")]
        [InlineData("name14")]
        [InlineData("_name_")]
        [InlineData("LabeL")]
        public void IsValidLabel_should_return_true_for_valid_labels(string label)
        {
            Assert.True(PrometheusConventions.IsValidName(label));
        }

        [Theory]
        [InlineData("le")]
        public void IsValidHistogramName_should_return_false_for_invalid_names(string name)
        {
            Assert.False(PrometheusConventions.IsValidHistogramName(name));
        }

        [Theory]
        [InlineData("name", "111")]
        [InlineData(" ", "good", "well")]
        [InlineData()]
        public void AreValidNames_should_return_false_when_one_of_the_names_is_invalid(params string[] name)
        {
            Assert.False(PrometheusConventions.AreValidNames(name));
        }

        [Theory]
        [InlineData("name", "le")]
        public void AreValidHistogramNames_should_return_false_when_one_of_the_names_is_invalid(params string[] name)
        {
            Assert.False(PrometheusConventions.AreValidHistogramNames(name));
        }

        [Fact]
        public void Help_text_is_properly_escaped()
        {
            Assert.Equal(@"help\nnewline", PrometheusConventions.EscapeHelp("help\nnewline"));
            Assert.Equal(@"a\\b", PrometheusConventions.EscapeHelp("a\\b"));
            Assert.Equal(@"it's ""OK""", PrometheusConventions.EscapeHelp("it's \"OK\""));
        }

        [Fact]
        public void Label_text_is_properly_escaped()
        {
            Assert.Equal(@"help\nnewline", PrometheusConventions.EscapeLabel("help\nnewline"));
            Assert.Equal(@"a\\b", PrometheusConventions.EscapeLabel("a\\b"));
            Assert.Equal(@"it's not \""OK\""", PrometheusConventions.EscapeLabel("it's not \"OK\""));
        }
    }
}
