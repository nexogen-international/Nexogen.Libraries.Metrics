using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Nexogen.Libraries.Metrics
{
    /// <summary>
    /// Common interface for building gauge.
    /// </summary>
    public interface IGaugeBuilder
    {
        /// <summary>
        /// Builds and registers the gauge to the default registry.
        /// </summary>
        /// <returns>The new gauge</returns>
        /// <exception cref="CollectorBuilderException">Thrown when the registration failed</exception>
        IGauge Register();

        /// <summary>
        /// Set the name of the gauge, must follow the guidelines of the backend.
        /// </summary>
        /// <param name="name">The name of the gauge</param>
        /// <returns>Returns the builder to allow fluent usage</returns>
        IGaugeBuilder Name(string name);

        /// <summary>
        /// Sets the help text for the gauge
        /// </summary>
        /// <param name="help">The help text</param>
        /// <returns>Returns the builder to allow fluent usage</returns>
        IGaugeBuilder Help(string help);

        /// <summary>
        /// Sets the label names of the gauge. Labels are optional and setting
        /// them causes the builder to build a labelled gauge, which requires setting
        /// the label values before use. Labelled collectors are more expensive, but also
        /// more versatile.
        /// </summary>
        /// <param name="labelNames">The list of label names</param>
        /// <returns>A new labelled gauge builder</returns>
        ILabelledGaugeBuilder LabelNames(params string[] labelNames);
    }
}
