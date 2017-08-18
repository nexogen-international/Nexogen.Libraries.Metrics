using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;

namespace Nexogen.Libraries.Metrics.Extensions
{
    internal class HistogramScopeTimer : IDisposable
    {
        private readonly IHistogram histogram;
        private readonly Stopwatch stopwatch;

        public HistogramScopeTimer(IHistogram histogram)
        {
            this.stopwatch = Stopwatch.StartNew();
            this.histogram = histogram;
        }

        public void Dispose()
        {
            this.histogram.Observe(stopwatch.ElapsedMilliseconds / 1000.0);
        }
    }
}
