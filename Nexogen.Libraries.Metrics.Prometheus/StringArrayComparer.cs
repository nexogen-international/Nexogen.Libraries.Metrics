using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Nexogen.Libraries.Metrics.Prometheus
{
    public class StringArrayComparer : IEqualityComparer<string[]>
    {
        public bool Equals(string[] x, string[] y)
        {
            return x.SequenceEqual(y);
        }

        public int GetHashCode(string[] obj)
        {
            // 2^31 aranymetszessel elosztva
            int hc = 1327217884;
            foreach (var o in obj)
            {
                hc ^= o.GetHashCode();
            }

            return hc;
        }
    }
}
