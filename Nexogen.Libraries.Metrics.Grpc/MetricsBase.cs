using System.Linq;
using Grpc.Core;

namespace Nexogen.Libraries.Metrics.Prometheus.Grpc
{
    /// <summary>
    /// Common base class for gRPC-related metrics.
    /// </summary>
    public abstract class MetricsBase
    {
        private static readonly string[] labelNames = {"grpc_type", "grpc_service", "grpc_method"};
        private static readonly string[] labelNamesWithCode = {"grpc_type", "grpc_service", "grpc_method", "grpc_code"};
    
        /// <summary>
        /// Registers gRPC metrics.
        /// </summary>
        /// <param name="metrics">Builder to register the metrics in.</param>
        /// <param name="kind">The kind of metrics: server or client.</param>
        protected MetricsBase(IMetrics metrics, string kind)
        {
            started = metrics
                .Counter()
                .Name($"grpc_{kind}_started_total")
                .Help($"Total number of RPCs started on the {kind}.")
                .LabelNames(labelNames)
                .Register();

            handled = metrics
                .Counter()
                .Name($"grpc_{kind}_handled_total")
                .Help($"Total number of RPCs completed on the {kind}, regardless of success or failure.")
                .LabelNames(labelNamesWithCode)
                .Register();

            streamMsgReceived = metrics
                .Counter()
                .Name($"grpc_{kind}_msg_received_total")
                .Help($"Total number of RPC stream messages received by the {kind}.")
                .LabelNames(labelNames)
                .Register();

            streamMsgSent = metrics
                .Counter()
                .Name($"grpc_{kind}_msg_sent_total")
                .Help($"Total number of gRPC stream messages sent by the {kind}.")
                .LabelNames(labelNames)
                .Register();
        }

        private readonly ILabelledCounter started;

        /// <summary>
        /// Call when an RPC started.
        /// </summary>
        /// <param name="type">The type of RPC method.</param>
        /// <param name="method">The full gRPC method name (including the service name).</param>
        public void Started(MethodType type, string method)
            => started.Labels(GetLabels(type, method)).Increment();

        private readonly ILabelledCounter handled;

        /// <summary>
        /// Call when an RPC completed, regardless of success or failure.
        /// </summary>
        /// <param name="type">The type of RPC method.</param>
        /// <param name="method">The full gRPC method name (including the service name).</param>
        /// <param name="statusCode">The gRPC status code.</param>
        public void Handled(MethodType type, string method, StatusCode statusCode)
            => handled.Labels(GetLabels(type, method, statusCode)).Increment();

        private readonly ILabelledCounter streamMsgReceived;

        /// <summary>
        /// Call when a stream message was received.
        /// </summary>
        /// <param name="type">The type of RPC method.</param>
        /// <param name="method">The full gRPC method name (including the service name).</param>
        public void StreamMsgReceived(MethodType type, string method)
            => streamMsgReceived.Labels(GetLabels(type, method)).Increment();

        private readonly ILabelledCounter streamMsgSent;

        /// <summary>
        /// Call when a stream message was sent.
        /// </summary>
        /// <param name="type">The type of RPC method.</param>
        /// <param name="method">The full gRPC method name (including the service name).</param>
        public void StreamMsgSent(MethodType type, string method)
            => streamMsgSent.Labels(GetLabels(type, method)).Increment();

        private static string[] GetLabels(MethodType type, string method)
        {
            var split = method.Split('/');
            return new[]
            {
                type switch
                {
                    MethodType.Unary => "unary",
                    MethodType.ClientStreaming => "client_stream",
                    MethodType.ServerStreaming => "server_stream",
                    MethodType.DuplexStreaming => "bidi_stream",
                    _ => "unknown"
                },
                split[0],
                split[1]
            };
        }

        private string[] GetLabels(MethodType type, string method, StatusCode statusCode)
            => GetLabels(type, method).Append(statusCode.ToString()).ToArray();
    }
}
