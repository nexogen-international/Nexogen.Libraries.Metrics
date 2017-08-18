using System;
using System.Collections.Generic;
using System.Text;

namespace Nexogen.Libraries.Metrics.UnitTests.Extensions
{
    class TestBucket : IBucket
    {
        public double Max { get; }

        public double Min { get; }

        public TestBucket(double min, double max)
        {
            Min = min;
            Max = max;
        }
    }
}
