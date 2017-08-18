using System;
using System.Text;
using System.Linq;
using Xunit;
using FluentAssertions;
using Nexogen.Libraries.Metrics.Extensions.Buckets;

namespace Nexogen.Libraries.Metrics.UnitTests.Extensions
{

    public class ExponentialBucketGeneratorTest
    {
        ExponentialBucketGenerator generator = new ExponentialBucketGenerator();

        [Fact]
        public void ExponentialBuckets_for_IHistogramBuilder_null_builder_throws()
        {
            IHistogramBuilder builder = null;
            Assert.Throws<ArgumentNullException>("builder", () => builder.ExponentialBuckets(1, 10, 3));
        }

        [Fact]
        public void ExponentialBuckets_for_ILabelledHistogramBuilder_null_builder_throws()
        {
            ILabelledHistogramBuilder builder = null;
            Assert.Throws<ArgumentNullException>("builder", () => builder.ExponentialBuckets(1, 10, 3));
        }

        [Theory]
        [InlineDataAttribute(0)]
        [InlineDataAttribute(-1)]
        public void ExponentialBuckets_for_invalid_count_throws(int count)
        {
            Assert.Throws<ArgumentException>("count", () => generator.ExponentialBuckets(1, 10, count));
        }

        [Theory]
        [InlineDataAttribute(-0.1)]
        [InlineDataAttribute(0)]
        [InlineDataAttribute(double.NegativeInfinity)]
        [InlineDataAttribute(double.PositiveInfinity)]
        [InlineDataAttribute(double.NaN)]
        public void ExponentialBuckets_for_invalid_start_throws(double start)
        {
            Assert.Throws<ArgumentException>("start", () => generator.ExponentialBuckets(start, 10, 3));
        }

        [Theory]
        [InlineDataAttribute(-0.1)]
        [InlineDataAttribute(0)]
        [InlineDataAttribute(0.1)]
        [InlineDataAttribute(1)]
        [InlineDataAttribute(double.NegativeInfinity)]
        [InlineDataAttribute(double.PositiveInfinity)]
        [InlineDataAttribute(double.NaN)]
        public void ExponentialBuckets_for_invalid_factor_throws(double factor)
        {
            Assert.Throws<ArgumentException>("factor", () => generator.ExponentialBuckets(1, factor, 3));
        }

        [Fact]
        public void ExponentialBuckets_single_bucket_is_generated_properly()
        {
            var expectedBuckets = new[] {
                        new Bucket(double.NegativeInfinity, 1),
                        new Bucket(1, double.PositiveInfinity)
            };

            var buckets = generator.ExponentialBuckets(1, 10, 1);

            buckets.ShouldAllBeEquivalentTo(expectedBuckets);
        }

        [Fact]
        public void ExponentialBuckets_multiple_buckets_are_generated_properly()
        {
            var expectedBuckets = new[] {
                        new Bucket(double.NegativeInfinity, 1),
                        new Bucket(1, 10),
                        new Bucket(10, 100),
                        new Bucket(100, double.PositiveInfinity)
            };

            var buckets = generator.ExponentialBuckets(1, 10, 3);

            buckets.ShouldAllBeEquivalentTo(expectedBuckets);
        }

    }
}
