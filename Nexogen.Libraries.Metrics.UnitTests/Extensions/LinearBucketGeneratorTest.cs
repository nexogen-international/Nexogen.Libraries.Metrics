using System;
using System.Text;
using System.Linq;
using Xunit;
using FluentAssertions;
using Nexogen.Libraries.Metrics.Extensions.Buckets;

namespace Nexogen.Libraries.Metrics.UnitTests.Extensions
{

    public class LinearBucketGeneratorTest
    {
        LinearBucketGenerator generator = new LinearBucketGenerator();


        [Fact]
        public void Buckets_for_IHistogramBuilder_null_builder_throws()
        {
            IHistogramBuilder builder = null;
            Assert.Throws<ArgumentNullException>("builder", () => builder.LinearBuckets(12, 34, 1));
        }

        [Fact]
        public void Buckets_for_ILabelledHistogramBuilder_null_builder_throws()
        {
            ILabelledHistogramBuilder builder = null;
            Assert.Throws<ArgumentNullException>("builder", () => builder.LinearBuckets(12, 34, 1));
        }

        [Theory]
        [InlineDataAttribute(0)]
        [InlineDataAttribute(-1)]
        public void LinearBuckets_for_invalid_count_throws(int count)
        {
            Assert.Throws<ArgumentException>("count", () => generator.LinearBuckets(12, 34, count));
        }

        [Theory]
        [InlineDataAttribute(-0.1)]
        [InlineDataAttribute(0)]
        [InlineDataAttribute(double.NegativeInfinity)]
        [InlineDataAttribute(double.PositiveInfinity)]
        [InlineDataAttribute(double.NaN)]
        public void LinerBuckets_for_invalid_width_throws(double width)
        {
            Assert.Throws<ArgumentException>("width", () => generator.LinearBuckets(12, width, 2));
        }

        [Theory]
        [InlineDataAttribute(double.NegativeInfinity)]
        [InlineDataAttribute(double.PositiveInfinity)]
        [InlineDataAttribute(double.NaN)]
        public void LinerBuckets_for_invalid_min_throws(double min)
        {
            Assert.Throws<ArgumentException>("min", () => generator.LinearBuckets(min, 34, 2));
        }

        [Fact]
        public void LinearBuckets_single_bucket_is_generated_properly()
        {
            var expectedBuckets = new[] {
                        new Bucket(double.NegativeInfinity, 12),
                        new Bucket(12, double.PositiveInfinity)
            };

            var buckets = generator.LinearBuckets(12, 34, 1);

            buckets.Should().BeEquivalentTo(expectedBuckets);
        }

        [Fact]
        public void LinearBuckets_multiple_buckets_are_generated_properly()
        {
            var expectedBuckets = new[] {
                        new Bucket(double.NegativeInfinity, -12),
                        new Bucket(-12, 12),
                        new Bucket(12, 36),
                        new Bucket(36, double.PositiveInfinity)
            };

            var buckets = generator.LinearBuckets(-12, 24, 3);

            buckets.Should().BeEquivalentTo(expectedBuckets);
        }

    }
}
