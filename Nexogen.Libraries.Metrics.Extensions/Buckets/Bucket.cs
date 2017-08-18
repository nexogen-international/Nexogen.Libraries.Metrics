using System;
using System.Collections.Generic;
using System.Text;

namespace Nexogen.Libraries.Metrics.Extensions.Buckets
{
    internal class Bucket : IBucket
    {
        public double Max { get; }

        public double Min { get; }

        public Bucket(double min, double max)
        {
            Min = min;
            Max = max;
        }
    }
}
