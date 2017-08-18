using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Nexogen.Libraries.Metrics.Extensions.Buckets
{
    /// <summary>
    /// ExponentialBucketGenerator helps to configure a historgram with buckets of equal widths.
    /// </summary>
    internal class ExponentialBucketGenerator
    {
        /// <summary>
        /// Generates <paramref name="count"/> count buckets, the first having the upper-incluse bound of start, others having an upper-inclusive bound that is the multiple of the previous bucket's upper-incluseive bound by <paramref name="factor"/>.
        /// An extra bucket is implicitly generated for values over the specified buckets (<see href="https://prometheus.io/docs/instrumenting/writing_clientlibs/#histogram"/>).
        /// </summary>
        /// <param name="start">Inclusive upper bound of lowest bucket. Must be positive, non-zero, finite number.</param>
        /// <param name="factor">The factor for scaling upper-inclusive bounds of subsequent buckets. Must be finite, greater than 1.</param>
        /// <param name="count">Number of intervals create buckets for. At least 1.</param>
        /// <returns></returns>
        public IBucket[]  ExponentialBuckets( double start, double factor, int count)
        {
            if (double.IsNaN(start) || double.IsInfinity(start) || start <= 0)
            {
                throw new ArgumentException($"Exponential Histogram start value must not be non-negative, finite number.", nameof(start));
            }

            if (double.IsNaN(factor) || double.IsInfinity(factor) || factor <= 1)
            {
                throw new ArgumentException($"Exponential Histogram factor must be a finite number greater than 1!", nameof(factor));
            }

            if (count <= 0)
            {
                throw new ArgumentException($"Exponential Histogram bucket count must be a finite number over at least 1", nameof(count));
            }

            var lowerBounds = Enumerable.Range(0, count)
                       .Select(i => start * Math.Pow(factor, i))
                       .Prepend(double.NegativeInfinity)
                       .ToArray();

            var buckets = lowerBounds.Zip(lowerBounds.Skip(1), (a, b) => new { Min = a, Max = b })
                                .Append(new { Min = lowerBounds.Last(), Max = double.PositiveInfinity })
                                .Select(b => new Bucket(b.Min, b.Max))
                                .ToArray();

            return buckets;
        }

    }
}
