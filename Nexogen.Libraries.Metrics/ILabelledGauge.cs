namespace Nexogen.Libraries.Metrics
{
    /// <summary>
    /// A gauge with labels. Labels can be used to differentiate
    /// the characteristics of the measer unit. A labelled gauge has
    /// a set of label names defined during build and the label values are
    /// given when using the gauge. Every unique combination of key-value
    /// label pairs represents a new time series, which can dramatically
    /// increase the amount of data stored.
    /// 
    /// Returns a gauge
    /// after giving the value of labels. The label's count must
    /// be the same as the label names give during build.
    /// </summary>
    public interface ILabelledGauge
    {
        /// <summary>
        /// Assign value to the configured labels.
        /// The count of values should be the same as the count of labels.
        /// A new gauge is created for each tuple of lable values.
        /// </summary>
        /// <param name="labels">The label values</param>
        /// <returns>The gauge unique to the given values</returns>
        IGauge Labels(params string[] labels);
    }
}
