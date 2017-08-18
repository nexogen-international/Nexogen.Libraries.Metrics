using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Nexogen.Libraries.Metrics.Extensions.Buckets
{
    /// <summary>
    /// BucketGenerator helps to configure a historgram with buckets of arbitrary (finite, ascending) bounds.
    /// </summary>
    internal class BucketGenerator
    {
        /// <summary>
        /// Generates a continous series of buckets with the specified upper-inclusive bounds, with the first having a lower bound of <see cref="double.NegativeInfinity"/> and upper-incluse bound of the first item of <paramref name="bounds"/>.
        /// An extra bucket is implicitly generated for values over the specified buckets (<see href="https://prometheus.io/docs/instrumenting/writing_clientlibs/#histogram"/>).
        /// </summary>
        /// <param name="bounds">The upper inclusive bounds of the buckets. Must be a non-empty, increasing series.</param>
        public IBucket[] Buckets(params double[] bounds)
        {
            if (bounds == null || !bounds.Any())
            {
                throw new ArgumentException("At least one bucket is required to initialize Histrogram", nameof(bounds));
            }

            if (bounds.Any(b => double.IsInfinity(b) || double.IsNaN(b)))
            {
                throw new ArgumentException("All bounds must be valid real numbers", nameof(bounds));
            }


            var lowerBounds = bounds.Prepend(double.NegativeInfinity).ToArray();

            var buckets = lowerBounds.Zip(lowerBounds.Skip(1), (a, b) => new { Min = a, Max = b })
                                .Append(new { Min = lowerBounds.Last(), Max = double.PositiveInfinity })
                                .Select(b => new Bucket(b.Min, b.Max))
                                .ToArray();

            if (buckets.Any(b => b.Min >= b.Max))
            {
                throw new ArgumentException("Invalid bucket initialization! Values should be unique and ascending.", nameof(bounds));
            }

            return buckets;
        }
    }
}
