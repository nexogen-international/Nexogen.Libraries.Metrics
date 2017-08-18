using System;
using System.Text;
using System.Linq;
using Xunit;
using FluentAssertions;
using Nexogen.Libraries.Metrics.Extensions.Buckets;

namespace Nexogen.Libraries.Metrics.UnitTests.Extensions
{
    public class CustomBucketGeneratorTest
    {
        BucketGenerator generator = new BucketGenerator();

        [Fact]
        public void Buckets_for_IHistogramBuilder_null_builder_throws()
        {
            IHistogramBuilder builder = null;
            Assert.Throws<ArgumentNullException>("builder", () => builder.Buckets(1, 2, 3));
        }

        [Fact]
        public void Buckets_for_ILabelledHistogramBuilder_null_builder_throws()
        {
            ILabelledHistogramBuilder builder = null;
            Assert.Throws<ArgumentNullException>("builder", () => builder.Buckets(1, 2, 3));
        }

        [Theory]
        [InlineDataAttribute(null)]
        [InlineDataAttribute(new double[] { })]
        [InlineDataAttribute(new double[] { double.NegativeInfinity })]
        [InlineDataAttribute(new double[] { double.PositiveInfinity })]
        [InlineDataAttribute(new double[] { double.NaN })]
        [InlineDataAttribute(new double[] { 1, 2, 4, 3 })]
        public void Buckets_for_invalid_bounds_throws(double[] bounds)
        {
            Assert.Throws<ArgumentException>("bounds", () => generator.Buckets(bounds));
        }

        [Fact]
        public void Buckets_single_bucket_is_generated_properly()
        {
            var expectedBuckets = new[] {
                        new TestBucket(double.NegativeInfinity, 12),
                        new TestBucket(12, double.PositiveInfinity)
            };

            var buckets = generator.Buckets(12);

            buckets.ShouldAllBeEquivalentTo(expectedBuckets);
        }

        [Fact]
        public void Buckets_multiple_buckets_are_generated_properly()
        {
            var expectedBuckets = new[] {
                        new TestBucket(double.NegativeInfinity, -12),
                        new TestBucket(-12, 12),
                        new TestBucket(12, 36),
                        new TestBucket(36, double.PositiveInfinity)
            };

            var buckets = generator.Buckets(-12, 12, 36);

            buckets.ShouldAllBeEquivalentTo(expectedBuckets);
        }

    }
}
