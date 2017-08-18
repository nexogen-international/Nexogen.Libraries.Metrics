using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Nexogen.Libraries.Metrics
{
    /// <summary>
    /// Gauge represents a value that can go up and down.
    /// </summary>
    public interface IGauge
    {
        /// <summary>
        /// The value of the counter can be get or set here.
        /// Accessing the value causes a memory barrier.
        /// </summary>
        double Value { get; set; }

        /// <summary>
        /// Increment by one. Thread safe.
        /// </summary>
        void Increment();

        /// <summary>
        /// Increment by amount. Thread safe.
        /// </summary>
        /// <param name="amount">The amount to increment with. Must be positive.</param>
        void Increment(double amount);

        /// <summary>
        /// Decrement by one. Thread safe.
        /// </summary>
        void Decrement();

        /// <summary>
        /// Decrement by amount. Thread safe.
        /// </summary>
        /// <param name="amount">The amount to decrement with. Must be positive.</param>
        void Decrement(double amount);
    }
}
