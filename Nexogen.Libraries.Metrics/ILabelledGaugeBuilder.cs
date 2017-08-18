namespace Nexogen.Libraries.Metrics
{
    /// <summary>
    /// Common interface for building labelled gauges.
    /// </summary>
    public interface ILabelledGaugeBuilder
    {
        /// <summary>
        /// Builds and registers the gauge to the default registry.
        /// </summary>
        /// <returns>The new gauge</returns>
        /// <exception cref="CollectorBuilderException">Thrown when the registration failed</exception>
        ILabelledGauge Register();

        /// <summary>
        /// Set the name of the gauge, must follow the guidelines of the backend.
        /// </summary>
        /// <param name="name">The name of the gauge</param>
        /// <returns>Returns the builder to allow fluent usage</returns>
        ILabelledGaugeBuilder Name(string name);

        /// <summary>
        /// Sets the help text for the gauge
        /// </summary>
        /// <param name="help">The help text</param>
        /// <returns>Returns the builder to allow fluent usage</returns>
        ILabelledGaugeBuilder Help(string help);

        /// <summary>
        /// Sets the label names of the gauge. Labels are optional and setting
        /// them causes the builder to build a labelled gauge, which requires setting
        /// the label values before use. Labelled gauges are more expensive, but also
        /// more versatile.
        /// </summary>
        /// <param name="labelNames">The list of label names</param>
        /// <returns>A new labelled gauge builder</returns>
        ILabelledGaugeBuilder LabelNames(params string[] labelNames);
    }
}
