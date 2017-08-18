using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;

namespace Nexogen.Libraries.Metrics.Extensions
{
    internal class GaugeScopeTimer : IDisposable
    {
        private readonly IGauge counter;
        private readonly Stopwatch stopwatch;

        public GaugeScopeTimer(IGauge counter)
        {
            this.stopwatch = Stopwatch.StartNew();
            this.counter = counter;
        }

        public void Dispose()
        {
            this.counter.Value = stopwatch.ElapsedMilliseconds / 1000.0;
        }
    }
}
