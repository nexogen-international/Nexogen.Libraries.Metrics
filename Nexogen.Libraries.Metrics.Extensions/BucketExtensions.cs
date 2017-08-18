using System;
using Nexogen.Libraries.Metrics.Extensions.Buckets;

namespace Nexogen.Libraries.Metrics
{
    /// <summary>
    /// Contains extension methods to simplify Bucket configuration of <see cref="IHistogramBuilder"/> and <see cref="ILabelledHistogramBuilder"/> instances.
    /// </summary>
    public static class BucketExtensions
    {
        /// <summary>
        /// Generates a continous series of buckets with the specified upper-inclusive bounds, with the first having a lower bound of <see cref="double.NegativeInfinity"/> and upper-incluse bound of the first item of <paramref name="bounds"/>.
        /// An extra bucket is implicitly generated for values over the specified buckets (<see href="https://prometheus.io/docs/instrumenting/writing_clientlibs/#histogram"/>).
        /// </summary>
        /// <param name="builder">The builder to assign the buckets to</param>
        /// <param name="bounds">The upper inclusive bounds of the buckets. Must be a non-empty, increasing series.</param>
        /// <returns>the builder supplied in <paramref name="builder"/></returns>
        public static IHistogramBuilder Buckets(this IHistogramBuilder builder, params double[] bounds)
        {
            if (builder == null)
            {
                throw new ArgumentNullException(nameof(builder));
            }

            var generator = new BucketGenerator();

            var buckets = generator.Buckets(bounds);

            builder.Buckets(buckets);

            return builder;
        }

        /// <summary>
        /// Generates <paramref name="count"/> count buckets of equal <paramref name="width"/>, with the first having a lower bound of <see cref="double.NegativeInfinity"/> and upper-incluse bound of <paramref name="min"/>.
        /// An extra bucket is implicitly generated for values over the specified buckets (<see href="https://prometheus.io/docs/instrumenting/writing_clientlibs/#histogram"/>).
        /// 
        /// The generated buckets are registered to <paramref name="builder"/>.
        /// </summary>
        /// <param name="builder">The builder to assign the buckets to</param>
        /// <param name="min">Inclusive upper bound of lowest bucket</param>
        /// <param name="width">The width of a bucket.</param>
        /// <param name="count">Number of intervals create buckets for. At least 1</param>
        /// <returns>the builder supplied in <paramref name="builder"/></returns>
        public static IHistogramBuilder LinearBuckets(this IHistogramBuilder builder, double min, double width, int count)
        {
            if (builder == null)
            {
                throw new ArgumentNullException(nameof(builder));
            }

            var generator = new LinearBucketGenerator();

            var buckets = generator.LinearBuckets(min, width, count);

            builder.Buckets(buckets);

            return builder;
        }

        /// <summary>
        /// Generates <paramref name="count"/> count buckets, the first having the upper-incluse bound of start, others having an upper-inclusive bound that is the multiple of the previous bucket's upper-incluseive bound by <paramref name="factor"/>.
        /// An extra bucket is implicitly generated for values over the specified buckets (<see href="https://prometheus.io/docs/instrumenting/writing_clientlibs/#histogram"/>).
        /// 
        /// The generated buckets are registered to <paramref name="builder"/>.
        /// </summary>
        /// <param name="builder">The builder to assign the buckets to.</param>
        /// <param name="start">Inclusive upper bound of lowest bucket. Must be positive, non-zero, finite number.</param>
        /// <param name="factor">The factor for scaling upper-inclusive bounds of subsequent buckets. Must be finite, greater than 1.</param>
        /// <param name="count">Number of intervals create buckets for. At least 1</param>
        /// <returns>the builder supplied in <paramref name="builder"/></returns>
        public static IHistogramBuilder ExponentialBuckets(this IHistogramBuilder builder, double start, double factor, int count)
        {
            if (builder == null)
            {
                throw new ArgumentNullException(nameof(builder));
            }

            var generator = new ExponentialBucketGenerator();

            var buckets = generator.ExponentialBuckets(start, factor, count);

            builder.Buckets(buckets);

            return builder;
        }

        /// <summary>
        /// Generates a continous series of buckets with the specified upper-inclusive bounds, with the first having a lower bound of <see cref="double.NegativeInfinity"/> and upper-incluse bound of the first item of <paramref name="bounds"/>.
        /// An extra bucket is implicitly generated for values over the specified buckets (<see href="https://prometheus.io/docs/instrumenting/writing_clientlibs/#histogram"/>).
        /// </summary>
        /// <param name="builder">The builder to assign the buckets to</param>
        /// <param name="bounds">The upper inclusive bounds of the buckets. Must be a non-empty, increasing series.</param>
        /// <returns>the builder supplied in <paramref name="builder"/></returns>
        public static ILabelledHistogramBuilder Buckets(this ILabelledHistogramBuilder builder, params double[] bounds)
        {
            if (builder == null)
            {
                throw new ArgumentNullException(nameof(builder));
            }

            var generator = new BucketGenerator();

            var buckets = generator.Buckets(bounds);

            builder.Buckets(buckets);

            return builder;
        }

        /// <summary>
        /// Generates <paramref name="count"/> count buckets of equal <paramref name="width"/>, with the first having a lower bound of <see cref="double.NegativeInfinity"/> and upper-incluse bound of <paramref name="min"/>.
        /// An extra bucket is implicitly generated for values over the specified buckets (<see href="https://prometheus.io/docs/instrumenting/writing_clientlibs/#histogram"/>).
        /// 
        /// The generated buckets are registered to <paramref name="builder"/>.
        /// </summary>
        /// <param name="builder">The builder to assign the buckets to</param>
        /// <param name="min">Inclusive upper bound of lowest bucket</param>
        /// <param name="width">The width of a bucket.</param>
        /// <param name="count">Number of intervals create buckets for. At least 1</param>
        /// <returns>the builder supplied in <paramref name="builder"/></returns>
        public static ILabelledHistogramBuilder LinearBuckets(this ILabelledHistogramBuilder builder, double min, double width, int count)
        {
            if (builder == null)
            {
                throw new ArgumentNullException(nameof(builder));
            }

            var generator = new LinearBucketGenerator();

            var buckets = generator.LinearBuckets(min, width, count);

            builder.Buckets(buckets);

            return builder;
        }

        /// <summary>
        /// Generates <paramref name="count"/> count buckets, the first having the upper-incluse bound of start, others having an upper-inclusive bound that is the multiple of the previous bucket's upper-incluseive bound by <paramref name="factor"/>.
        /// An extra bucket is implicitly generated for values over the specified buckets (<see href="https://prometheus.io/docs/instrumenting/writing_clientlibs/#histogram"/>).
        /// 
        /// The generated buckets are registered to <paramref name="builder"/>.
        /// </summary>
        /// <param name="builder">The builder to assign the buckets to.</param>
        /// <param name="start">Inclusive upper bound of lowest bucket. Must be positive, non-zero, finite number.</param>
        /// <param name="factor">The factor for scaling upper-inclusive bounds of subsequent buckets. Must be finite, greater than 1.</param>
        /// <param name="count">Number of intervals create buckets for. At least 1</param>
        /// <returns>the builder supplied in <paramref name="builder"/></returns>
        public static ILabelledHistogramBuilder ExponentialBuckets(this ILabelledHistogramBuilder builder, double start, double factor, int count)
        {
            if (builder == null)
            {
                throw new ArgumentNullException(nameof(builder));
            }

            var generator = new ExponentialBucketGenerator();

            var buckets = generator.ExponentialBuckets(start, factor, count);

            builder.Buckets(buckets);

            return builder;
        }
    }
    }
