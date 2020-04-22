using System.Threading.Tasks;
using Grpc.Core;
using Grpc.Core.Interceptors;
using Nexogen.Libraries.Metrics.Prometheus.Grpc.Internal;

namespace Nexogen.Libraries.Metrics.Prometheus.Grpc
{
    /// <summary>
    /// gRPC server interceptor for collecting Prometheus metrics.
    /// </summary>
    public class ServerMetricsInterceptor : Interceptor
    {
        private ServerMetrics metrics;

        /// <summary>
        /// Creates a gRPC server interceptor for collecting Prometheus metrics.
        /// </summary>
        public ServerMetricsInterceptor(ServerMetrics metrics)
        {
            this.metrics = metrics;
        }

        public override async Task<TResponse> UnaryServerHandler<TRequest, TResponse>(TRequest request, ServerCallContext context, UnaryServerMethod<TRequest, TResponse> continuation)
        {
            metrics.Started(MethodType.Unary, context.Method);
            try
            {
                var response = await continuation(request, context);
                metrics.Handled(MethodType.Unary, context.Method, StatusCode.OK);
                return response;
            }
            catch (RpcException ex)
            {
                metrics.Handled(MethodType.Unary, context.Method, ex.StatusCode);
                throw;
            }
            catch
            {
                metrics.Handled(MethodType.Unary, context.Method, StatusCode.Internal);
                throw;
            }
        }

        public override async Task<TResponse> ClientStreamingServerHandler<TRequest, TResponse>(IAsyncStreamReader<TRequest> requestStream, ServerCallContext context, ClientStreamingServerMethod<TRequest, TResponse> continuation)
        {
            metrics.Started(MethodType.ClientStreaming, context.Method);
            try
            {
                var response = await continuation(
                    new CountingStreamReader<TRequest>(requestStream, () => metrics.StreamMsgReceived(MethodType.ClientStreaming, context.Method)),
                    context);
                metrics.Handled(MethodType.ClientStreaming, context.Method, StatusCode.OK);
                return response;
            }
            catch (RpcException ex)
            {
                metrics.Handled(MethodType.ClientStreaming, context.Method, ex.StatusCode);
                throw;
            }
            catch
            {
                metrics.Handled(MethodType.ClientStreaming, context.Method, StatusCode.Internal);
                throw;
            }
        }

        public override async Task ServerStreamingServerHandler<TRequest, TResponse>(TRequest request, IServerStreamWriter<TResponse> responseStream, ServerCallContext context, ServerStreamingServerMethod<TRequest, TResponse> continuation)
        {
            metrics.Started(MethodType.ServerStreaming, context.Method);
            try
            {
                await continuation(request,
                    new CountingStreamWriter<TResponse>(responseStream, () => metrics.StreamMsgSent(MethodType.ServerStreaming, context.Method)),
                    context);
                metrics.Handled(MethodType.ServerStreaming, context.Method, StatusCode.OK);
            }
            catch (RpcException ex)
            {
                metrics.Handled(MethodType.ServerStreaming, context.Method, ex.StatusCode);
                throw;
            }
            catch
            {
                metrics.Handled(MethodType.ServerStreaming, context.Method, StatusCode.Internal);
                throw;
            }
        }

        public override async Task DuplexStreamingServerHandler<TRequest, TResponse>(IAsyncStreamReader<TRequest> requestStream, IServerStreamWriter<TResponse> responseStream, ServerCallContext context, DuplexStreamingServerMethod<TRequest, TResponse> continuation)
        {
            metrics.Started(MethodType.DuplexStreaming, context.Method);
            try
            {
                await continuation(
                    new CountingStreamReader<TRequest>(requestStream, () => metrics.StreamMsgReceived(MethodType.DuplexStreaming, context.Method)),
                    new CountingStreamWriter<TResponse>(responseStream, () => metrics.StreamMsgSent(MethodType.DuplexStreaming, context.Method)),
                    context);
                metrics.Handled(MethodType.DuplexStreaming, context.Method, StatusCode.OK);
            }
            catch (RpcException ex)
            {
                metrics.Handled(MethodType.DuplexStreaming, context.Method, ex.StatusCode);
                throw;
            }
            catch
            {
                metrics.Handled(MethodType.DuplexStreaming, context.Method, StatusCode.Internal);
                throw;
            }
        }
    }
}
