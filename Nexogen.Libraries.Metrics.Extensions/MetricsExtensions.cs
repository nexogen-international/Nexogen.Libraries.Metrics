using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Nexogen.Libraries.Metrics;

namespace Nexogen.Libraries.Metrics.Extensions
{
    public static class MetricsExtensions
    {
        public static IDisposable Timer(this IGauge counter)
        {
            return new GaugeScopeTimer(counter);
        }

        public static IDisposable Timer(this IHistogram histogram)
        {
            return new HistogramScopeTimer(histogram);
        }

        public static void SetToCurrentTime(this IGauge gauge)
        {
            gauge.Value = DateTimeOffset.Now.ToUnixTimeSeconds();
        }

        public static T TrackInProgress<T>(this IGauge gauge, Func<T> fun)
        {
            gauge.Increment();
            var ret = fun.Invoke();
            gauge.Decrement();

            return ret;
        }

        public static void TrackInProgress(this IGauge gauge, Action fun)
        {
            gauge.Increment();
            fun.Invoke();
            gauge.Decrement();
        }
    }
}
