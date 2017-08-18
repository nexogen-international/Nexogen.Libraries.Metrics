using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace Nexogen.Libraries.Metrics.Prometheus
{
    /// <summary>
    /// Something that can be exposed through a Stream.
    /// </summary>
    public interface IExposable
    {
        /// <summary>
        /// Expose to a stream using the default format
        /// </summary>
        /// <param name="output">The Stream where the serialized output will be written.</param>
        /// <param name="options">Options for changing output format.</param>
        /// <returns>async task</returns>
        Task Expose(Stream output, ExposeOptions options);
    }
}
