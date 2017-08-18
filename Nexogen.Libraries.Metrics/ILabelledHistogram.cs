namespace Nexogen.Libraries.Metrics
{
    /// <summary>
    /// A histogram with labels. Labels can be used to differentiate
    /// the characteristics of the measer unit. A labelled histogram has
    /// a set of label names defined during build and the label values are
    /// given when using the histogram. Every unique combination of key-value
    /// label pairs represents a new time series, which can dramatically
    /// increase the amount of data stored.
    /// 
    /// Returns a histogram
    /// after giving the value of labels. The label's count must
    /// be the same as the label names give during build.
    /// </summary>
    public interface ILabelledHistogram
    {
        /// <summary>
        /// Assign value to the configured labels.
        /// The count of values should be the same as the count of labels.
        /// A new histogram is created for each tuple of lable values.
        /// </summary>
        /// <param name="labels">The label values</param>
        /// <returns>The histogram unique to the given values</returns>
        IHistogram Labels(params string[] labels);
    }
}
