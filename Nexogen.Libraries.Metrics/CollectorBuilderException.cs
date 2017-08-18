using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Nexogen.Libraries.Metrics
{
    /// <summary>
    /// This exception is thrown when a <see cref="ICounterBuilder"/>, <see cref="ILabelledCounterBuilder"/>, <see cref="IGaugeBuilder"/>, <see cref="ILabelledGaugeBuilder"/>, <see cref="IHistogramBuilder"/> or <see cref="ILabelledGaugeBuilder"/> encounters an error during initialization.
    /// </summary>
    public class CollectorBuilderException : Exception
    {
        /// <summary>
        /// Initializes a CollectorBuilderException
        /// </summary>
        public CollectorBuilderException() { }
        /// <summary>
        /// CollectorBuilderException
        /// </summary>
        /// <param name="message">The exception message</param>
        public CollectorBuilderException(string message) : base(message) { }
        /// <summary>
        /// CollectorBuilderException
        /// </summary>
        /// <param name="message">The exception message</param>
        /// <param name="inner">The original cause</param>
        public CollectorBuilderException(string message, Exception inner) : base(message, inner) { }
    }
}
