using System;
using System.Collections.Generic;
using System.Text;

namespace Nexogen.Libraries.Metrics.Prometheus
{
    [Flags]
    public enum ExposeOptions
    {
        Default = 0,
        NoTimestamp = 1,
    }
}
