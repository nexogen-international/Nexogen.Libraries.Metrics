using Grpc.Core;
using Grpc.Core.Interceptors;
using Nexogen.Libraries.Metrics.Grpc.Internal;

namespace Nexogen.Libraries.Metrics.Grpc
{
    /// <summary>
    /// gRPC client interceptor for collecting Prometheus metrics.
    /// </summary>
    public class ClientMetricsInterceptor : Interceptor
    {
        private IGrpcClientMetrics metrics;

        /// <summary>
        /// Creates a gRPC client interceptor for collecting Prometheus metrics.
        /// </summary>
        public ClientMetricsInterceptor(IGrpcClientMetrics metrics)
        {
            this.metrics = metrics;
        }

        public override TResponse BlockingUnaryCall<TRequest, TResponse>(TRequest request, ClientInterceptorContext<TRequest, TResponse> context, BlockingUnaryCallContinuation<TRequest, TResponse> continuation)
        {
            Started(MethodType.Unary, context);
            try
            {
                var response = base.BlockingUnaryCall(request, context, continuation);
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

        public override AsyncUnaryCall<TResponse> AsyncUnaryCall<TRequest, TResponse>(TRequest request, ClientInterceptorContext<TRequest, TResponse> context, AsyncUnaryCallContinuation<TRequest, TResponse> continuation)
        {
            Started(MethodType.Unary, context);
            var call = continuation(request, context);
            return new AsyncUnaryCall<TResponse>(
                call.ResponseAsync.ContinueWith(task =>
                {
                    Handled(MethodType.Unary, context, call.GetStatus().StatusCode);
                    return task.Result;
                }),
                call.ResponseHeadersAsync, call.GetStatus, call.GetTrailers, call.Dispose);
        }

        public override AsyncClientStreamingCall<TRequest, TResponse> AsyncClientStreamingCall<TRequest, TResponse>(ClientInterceptorContext<TRequest, TResponse> context, AsyncClientStreamingCallContinuation<TRequest, TResponse> continuation)
        {
            Started(MethodType.ClientStreaming, context);
            var call = continuation(context);
            return new AsyncClientStreamingCall<TRequest, TResponse>(
                new CountingClientStreamWriter<TRequest>(call.RequestStream, () => StreamMsgSent(MethodType.ClientStreaming, context)),
                call.ResponseAsync.ContinueWith(task =>
                {
                    Handled(MethodType.ClientStreaming, context, call.GetStatus().StatusCode);
                    return task.Result;
                }),
                call.ResponseHeadersAsync, call.GetStatus, call.GetTrailers, call.Dispose);
        }

        public override AsyncServerStreamingCall<TResponse> AsyncServerStreamingCall<TRequest, TResponse>(TRequest request, ClientInterceptorContext<TRequest, TResponse> context, AsyncServerStreamingCallContinuation<TRequest, TResponse> continuation)
        {
            Started(MethodType.ServerStreaming, context);
            var call = continuation(request, context);
            return new AsyncServerStreamingCall<TResponse>(
                new CountingStreamReader<TResponse>(call.ResponseStream, () => StreamMsgReceived(MethodType.ServerStreaming, context)),
                call.ResponseHeadersAsync.ContinueWith(task =>
                {
                    Handled(MethodType.ServerStreaming, context, call.GetStatus().StatusCode);
                    return task.Result;
                }),
                call.GetStatus, call.GetTrailers, call.Dispose);
        }

        public override AsyncDuplexStreamingCall<TRequest, TResponse> AsyncDuplexStreamingCall<TRequest, TResponse>(ClientInterceptorContext<TRequest, TResponse> context, AsyncDuplexStreamingCallContinuation<TRequest, TResponse> continuation)
        {
            Started(MethodType.DuplexStreaming, context);
            var call = continuation(context);
            return new AsyncDuplexStreamingCall<TRequest, TResponse>(
                new CountingClientStreamWriter<TRequest>(call.RequestStream, () => StreamMsgSent(MethodType.DuplexStreaming, context)),
                new CountingStreamReader<TResponse>(call.ResponseStream, () => StreamMsgReceived(MethodType.DuplexStreaming, context)),
                call.ResponseHeadersAsync.ContinueWith(task =>
                {
                    Handled(MethodType.DuplexStreaming, context, call.GetStatus().StatusCode);
                    return task.Result;
                }),
                call.GetStatus, call.GetTrailers, call.Dispose);
        }

        private void Started<TRequest, TResponse>(MethodType type, ClientInterceptorContext<TRequest, TResponse> context)
            where TRequest : class
            where TResponse : class
            => metrics.Started(type, context.Method.ServiceName, context.Method.Name);

        private void Handled<TRequest, TResponse>(MethodType type, ClientInterceptorContext<TRequest, TResponse> context, StatusCode status)
            where TRequest : class
            where TResponse : class
            => metrics.Handled(type, context.Method.ServiceName, context.Method.Name, status);

        private void StreamMsgReceived<TRequest, TResponse>(MethodType type, ClientInterceptorContext<TRequest, TResponse> context)
            where TRequest : class
            where TResponse : class
            => metrics.StreamMsgReceived(type, context.Method.ServiceName, context.Method.Name);

        private void StreamMsgSent<TRequest, TResponse>(MethodType type, ClientInterceptorContext<TRequest, TResponse> context)
            where TRequest : class
            where TResponse : class
            => metrics.StreamMsgSent(type, context.Method.ServiceName, context.Method.Name);
    }
}
