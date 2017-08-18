using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Nexogen.Libraries.Metrics
{   /// <summary>
    /// Statistical information to show the frequency of data items in successive numerical intervals
    /// </summary>
    public interface IHistogram
    {
        /// <summary>
        /// Increases number of total items in histogram buckets, based on the specified value
        /// </summary>
        void Observe(double value);
    }
}
