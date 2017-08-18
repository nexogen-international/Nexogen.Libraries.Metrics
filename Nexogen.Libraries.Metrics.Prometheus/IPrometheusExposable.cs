using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace Nexogen.Libraries.Metrics.Prometheus
{
    public interface IPrometheusExposable
    {
        Task ExposeText(TextWriter writer, ExposeOptions options);
    }
}
