using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Nexogen.Libraries.Metrics.Extensions.Buckets
{
    /// <summary>
    /// LinearBucketGenerator helps to configure a historgram with buckets of equal widths.
    /// </summary>
    internal class LinearBucketGenerator
    {
        /// <summary>
        /// Generates <paramref name="count"/> count buckets of equal <paramref name="width"/>, with the first having a lower bound of <see cref="double.NegativeInfinity"/> and upper-incluse bound of <paramref name="min"/>.
        /// An extra bucket is implicitly generated for values over the specified buckets (<see href="https://prometheus.io/docs/instrumenting/writing_clientlibs/#histogram"/>).
        /// </summary>
        /// <param name="min">Inclusive upper bound of lowest bucket</param>
        /// <param name="width">The width of a bucket.</param>
        /// <param name="count">Number of intervals create buckets for. At least 1</param>
        public IBucket[] LinearBuckets(double min, double width, int count) 
        {
            if (Double.IsNaN(min) || Double.IsInfinity(min))
            {
                throw new ArgumentException($"Histogram minimum value must be an ordinary finite number", nameof(min));
            }

            if (Double.IsNaN(width) || Double.IsInfinity(width) || width <= 0)
            {
                throw new ArgumentException($"Bucket width must be a non-zero positive number", nameof(width));
            }

            if (count <= 0)
            {
                throw new ArgumentException("Histogram must have at least one bucket", nameof(count));
            }

            // Create the lower bounds of the buckets
            var lowerBounds = Enumerable.Range(0, count)
                       .Select(i => min + i * width)
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
