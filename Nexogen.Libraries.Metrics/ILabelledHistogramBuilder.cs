namespace Nexogen.Libraries.Metrics
{
    /// <summary>
    /// Common interface for building labelled histograms.
    /// </summary>
    public interface ILabelledHistogramBuilder
    {
        /// <summary>
        /// Builds and registers the histogram to the default registry.
        /// </summary>
        /// <returns>The new histogram</returns>
        /// <exception cref="CollectorBuilderException">Thrown when the registration failed</exception>
        ILabelledHistogram Register();

        /// <summary>
        /// Set the name of the histogram, must follow the guidelines of the backend.
        /// </summary>
        /// <param name="name">The name of the histogram</param>
        /// <returns>Returns the builder to allow fluent usage</returns>
        ILabelledHistogramBuilder Name(string name);

        /// <summary>
        /// Sets the help text for the histogram
        /// </summary>
        /// <param name="help">The help text</param>
        /// <returns>Returns the builder to allow fluent usage</returns>
        ILabelledHistogramBuilder Help(string help);

        /// <summary>
        /// Sets the buckets for the histogram
        /// </summary>
        /// <param name="buckets">The buckets for for use in histogram</param>
        /// <returns>Returns the builder to allow fluent usage</returns>
        ILabelledHistogramBuilder Buckets(IBucket[] buckets);

        /// <summary>
        /// Sets the label names of the histogram. Labels are optional and setting
        /// them causes the builder to build a labelled histogram, which requires setting
        /// the label values before use. Labelled histograms are more expensive, but also
        /// more versatile.
        /// </summary>
        /// <param name="labelNames">The list of label names</param>
        /// <returns>A new labelled histogram builder</returns>
        ILabelledHistogramBuilder LabelNames(params string[] labelNames);
    }
}
