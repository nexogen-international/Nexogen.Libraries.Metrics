namespace Nexogen.Libraries.Metrics.Grpc
{
    /// <summary>
    /// Metrics for gRPC servers.
    /// </summary>
    public class GrpcServerMetrics : GrpcMetricsBase, IGrpcServerMetrics
    {
        /// <summary>
        /// Registers gRPC server metrics.
        /// </summary>
        /// <param name="metrics">Builder to register the metrics in.</param>
        public GrpcServerMetrics(IMetrics metrics) : base(metrics, kind: "server")
        {}
    }
}
