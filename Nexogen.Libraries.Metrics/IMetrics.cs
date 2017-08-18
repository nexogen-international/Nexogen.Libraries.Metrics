using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Nexogen.Libraries.Metrics
{
    /// <summary>
    /// Contains the builders for the available metric types and the default registry.
    /// </summary>
    public interface IMetrics
    {
        /// <summary>
        /// Returns a builder for creating a Counter
        /// </summary>
        /// <returns>A new gauge builder</returns>
        ICounterBuilder Counter();

        /// <summary>
        /// Returns a builder for creating a Gauge
        /// </summary>
        /// <returns>A new Gauge builder</returns>
        IGaugeBuilder Gauge();

        /// <summary>
        /// Returns a builder for creating a Histogram
        /// </summary>
        /// <returns>A new Histogram builder</returns>
        IHistogramBuilder Histogram();
    }
}
