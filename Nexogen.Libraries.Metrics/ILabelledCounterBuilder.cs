namespace Nexogen.Libraries.Metrics
{
    /// <summary>
    /// Common interface for building labelled counters.
    /// </summary>
    public interface ILabelledCounterBuilder
    {
        /// <summary>
        /// Builds and registers the counter to the default registry.
        /// </summary>
        /// <returns>The new counter</returns>
        /// <exception cref="CollectorBuilderException">Thrown when the registration failed</exception>
        ILabelledCounter Register();

        /// <summary>
        /// Set the name of the counter, must follow the guidelines of the backend.
        /// </summary>
        /// <param name="name">The name of the counter</param>
        /// <returns>Returns the builder to allow fluent usage</returns>
        ILabelledCounterBuilder Name(string name);

        /// <summary>
        /// Sets the help text for the counter
        /// </summary>
        /// <param name="help">The help text</param>
        /// <returns>Returns the builder to allow fluent usage</returns>
        ILabelledCounterBuilder Help(string help);

        /// <summary>
        /// Sets the label names of the counter. Labels are optional and setting
        /// them causes the builder to build a labelled counter, which requires setting
        /// the label values before use. Labelled counters are more expensive, but also
        /// more versatile.
        /// </summary>
        /// <param name="labelNames">The list of label names</param>
        /// <returns>A new labelled counter builder</returns>
        ILabelledCounterBuilder LabelNames(params string[] labelNames);
    }
}
