namespace Nexogen.Libraries.Metrics.Prometheus.Grpc
{
    /// <summary>
    /// Metrics for gRPC servers.
    /// </summary>
    public class ServerMetrics : MetricsBase
    {
        /// <summary>
        /// Registers gRPC server metrics.
        /// </summary>
        /// <param name="metrics">Builder to register the metrics in.</param>
        public ServerMetrics(IMetrics metrics) : base(metrics, kind: "server")
        {}
    }
}
