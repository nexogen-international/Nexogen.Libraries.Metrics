using System;
using System.Collections.Generic;

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
        /// A new gauge is created for each tuple of label values.
        /// </summary>
        /// <param name="labels">The label values</param>
        /// <returns>The gauge unique to the given values</returns>
        /// <exception cref="ArgumentException">The number of label names aren't equal to the defined labels.</exception>
        IGauge Labels(params string[] labels);


        /// <summary>
        /// Replace the current values with a new value set. 
        /// This method can be useful if you want to update the 
        /// whole set and deprecate old values in the same time. 
        /// The update operation happens as an atomic operation.
        /// </summary>
        /// <param name="newMetricValues">the new values with labels. </param>
        /// <returns>Nothing.</returns>
        /// <exception cref="ArgumentNullException">if newValues is null</exception>
        /// <exception cref="ArgumentException">if label count doesn't match with the defined labels.</exception>
        void ReplaceMetricValues(IDictionary<string[], double> newMetricValues);
    }
}
