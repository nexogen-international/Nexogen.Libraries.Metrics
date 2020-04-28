using System.Linq;
using System.Threading.Tasks;
using Grpc.Core;
using Grpc.Core.Interceptors;
using Nexogen.Libraries.Metrics.Grpc.Internal;

namespace Nexogen.Libraries.Metrics.Grpc
{
    /// <summary>
    /// gRPC server interceptor for collecting Prometheus metrics.
    /// </summary>
    public class ServerMetricsInterceptor : Interceptor
    {
        private IGrpcServerMetrics metrics;

        /// <summary>
        /// Creates a gRPC server interceptor for collecting Prometheus metrics.
        /// </summary>
        public ServerMetricsInterceptor(IGrpcServerMetrics metrics)
        {
            this.metrics = metrics;
        }

        public override async Task<TResponse> UnaryServerHandler<TRequest, TResponse>(TRequest request, ServerCallContext context, UnaryServerMethod<TRequest, TResponse> continuation)
        {
            Started(MethodType.Unary, context);
            try
            {
                var response = await continuation(request, context);
                Handled(MethodType.Unary, context, StatusCode.OK);
                return response;
            }
            catch (RpcException ex)
            {
                Handled(MethodType.Unary, context, ex.StatusCode);
                throw;
            }
            catch
            {
                Handled(MethodType.Unary, context, StatusCode.Internal);
                throw;
            }
        }

        public override async Task<TResponse> ClientStreamingServerHandler<TRequest, TResponse>(IAsyncStreamReader<TRequest> requestStream, ServerCallContext context, ClientStreamingServerMethod<TRequest, TResponse> continuation)
        {
            Started(MethodType.ClientStreaming, context);
            try
            {
                var response = await continuation(
                    new CountingStreamReader<TRequest>(requestStream, () => StreamMsgReceived(MethodType.ClientStreaming, context)),
                    context);
                Handled(MethodType.ClientStreaming, context, StatusCode.OK);
                return response;
            }
            catch (RpcException ex)
            {
                Handled(MethodType.ClientStreaming, context, ex.StatusCode);
                throw;
            }
            catch
            {
                Handled(MethodType.ClientStreaming, context, StatusCode.Internal);
                throw;
            }
        }

        public override async Task ServerStreamingServerHandler<TRequest, TResponse>(TRequest request, IServerStreamWriter<TResponse> responseStream, ServerCallContext context, ServerStreamingServerMethod<TRequest, TResponse> continuation)
        {
            Started(MethodType.ServerStreaming, context);
            try
            {
                await continuation(request,
                    new CountingServerStreamWriter<TResponse>(responseStream, () => StreamMsgSent(MethodType.ServerStreaming, context)),
                    context);
                Handled(MethodType.ServerStreaming, context, StatusCode.OK);
            }
            catch (RpcException ex)
            {
                Handled(MethodType.ServerStreaming, context, ex.StatusCode);
                throw;
            }
            catch
            {
                Handled(MethodType.ServerStreaming, context, StatusCode.Internal);
                throw;
            }
        }

        public override async Task DuplexStreamingServerHandler<TRequest, TResponse>(IAsyncStreamReader<TRequest> requestStream, IServerStreamWriter<TResponse> responseStream, ServerCallContext context, DuplexStreamingServerMethod<TRequest, TResponse> continuation)
        {
            Started(MethodType.DuplexStreaming, context);
            try
            {
                await continuation(
                    new CountingStreamReader<TRequest>(requestStream, () => StreamMsgReceived(MethodType.DuplexStreaming, context)),
                    new CountingServerStreamWriter<TResponse>(responseStream, () => StreamMsgSent(MethodType.DuplexStreaming, context)),
                    context);
                Handled(MethodType.DuplexStreaming, context, StatusCode.OK);
            }
            catch (RpcException ex)
            {
                Handled(MethodType.DuplexStreaming, context, ex.StatusCode);
                throw;
            }
            catch
            {
                Handled(MethodType.DuplexStreaming, context, StatusCode.Internal);
                throw;
            }
        }

        private void Started(MethodType type, ServerCallContext context)
        {
            var (service, method) = SplitName(context.Method);
            metrics.Started(type, service, method);
        }

        private void Handled(MethodType type, ServerCallContext context, StatusCode status)
        {
            var (service, method) = SplitName(context.Method);
            metrics.Handled(type, service, method, status);
        }

        private void StreamMsgReceived(MethodType type, ServerCallContext context)
        {
            var (service, method) = SplitName(context.Method);
            metrics.StreamMsgReceived(type, service, method);
        }

        private void StreamMsgSent(MethodType type, ServerCallContext context)
        {
            var (service, method) = SplitName(context.Method);
            metrics.StreamMsgSent(type, service, method);
        }

        private static (string service, string method) SplitName(string name)
        {
            var split = name.Split('.');
            return (
                service: string.Join(".", split.Take(split.Length - 1)),
                method: split.Last()
            );
        }
    }
}
