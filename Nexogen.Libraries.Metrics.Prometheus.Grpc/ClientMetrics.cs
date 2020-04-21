namespace Nexogen.Libraries.Metrics.Prometheus.Grpc
{
    /// <summary>
    /// Metrics for gRPC clients.
    /// </summary>
    public class ClientMetrics : MetricsBase
    {
        /// <summary>
        /// Registers gRPC client metrics.
        /// </summary>
        /// <param name="metrics">Builder to register the metrics in.</param>
        public ClientMetrics(IMetrics metrics) : base(metrics, kind: "client")
        {}
    }
}
