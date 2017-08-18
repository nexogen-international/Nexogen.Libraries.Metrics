using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Threading;

namespace Nexogen.Libraries.Metrics.Prometheus
{
    /// <summary>
    /// Represents a bucket in Histograms
    /// </summary>
    internal class Bucket : IBucket
    {
        /// <summary>
        /// Minimal (exclusive) boundary value of the bucket
        /// </summary>
        public double Min { get; }

        /// <summary>
        /// Maximal (inclusive) boundary value of the bucket
        /// </summary>
        public double Max { get; }

        /// <summary>
        /// Total number of items in the bucket
        /// </summary>
        public long ItemCount => Interlocked.Read(ref itemcount);
        private long itemcount;

        /// <summary>
        /// Thread safe method to increment total number of items in the bucket
        /// </summary>
        public void IncrementItemCount() { Interlocked.Increment(ref itemcount); }

        /// <summary>
        /// Bucket initialization with minimal and maximal boundary values
        /// </summary>
        public Bucket(double min, double max)
        {
            this.Min = min;
            this.Max = max;
            this.itemcount = 0;
        }

        /// <summary>
        /// Bucket initialization with minimal and maximal boundary values
        /// </summary>
        public Bucket(IBucket bucket) : this(bucket.Min, bucket.Max)
        {
        }
    }
}
