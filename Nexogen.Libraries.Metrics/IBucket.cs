using System;

namespace Nexogen.Libraries.Metrics
{
    /// <summary>
    /// Represents a bucket in Histograms
    /// </summary>
    public interface IBucket
    {
        /// <summary>
        /// Minimal (exclusive) boundary value of the bucket
        /// </summary>
        double Max { get; }

        /// <summary>
        /// Maximal (inclusive) boundary value of the bucket
        /// </summary>
        double Min { get; }
    }
}