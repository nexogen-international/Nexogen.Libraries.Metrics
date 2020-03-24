namespace Nexogen.Libraries.Metrics.Prometheus.Standalone
{
    /// <summary>
    /// Configuration for <see cref="PrometheusServer"/>.
    /// </summary>
    public class PrometheusServerOptions
    {
        /// <summary>
        /// Port number to expose Prometheus metrics on.
        /// </summary>
        public int Port { get; set; } = 9100;
    }
}
