using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Nexogen.Libraries.Metrics
{
    /// <summary>
    /// Counter is a monotonically increasing counter.
    /// </summary>
    public interface ICounter
    {
        /// <summary>
        /// The value of the counter is accessable through this property
        /// </summary>
        double Value { get; }

        /// <summary>
        /// Increments the counter's value by one. Thread safe.
        /// </summary>
        void Increment();

        /// <summary>
        /// Increments the counter value by amount. Thread safe.
        /// </summary>
        /// <param name="amount">The amount to increase the counter width. Must be positive.</param>
        void Increment(double amount);
    }
}
