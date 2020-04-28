using System;
using System.Threading;
using System.Threading.Tasks;
using Grpc.Core;
using Grpc.Core.Interceptors;
using Moq;
using Xunit;

namespace Nexogen.Libraries.Metrics.Grpc.UnitTests
{
    public class ClientMetricsInterceptorTest
    {
        private readonly Mock<IGrpcClientMetrics> metricsMock = new Mock<IGrpcClientMetrics>();
        private readonly ClientMetricsInterceptor interceptor;

        public ClientMetricsInterceptorTest()
        {
            interceptor = new ClientMetricsInterceptor(metricsMock.Object);
        }

        [Fact]
        public void BlockingUnaryCall()
        {
            interceptor.BlockingUnaryCall("request", Context(MethodType.Unary, "package.Service", "Method"), (req, ctx) => "response");

            metricsMock.Verify(x => x.Started(MethodType.Unary, "package.Service", "Method"));
            metricsMock.Verify(x => x.Handled(MethodType.Unary, "package.Service", "Method", StatusCode.OK));
        }

        [Fact]
        public void BlockingUnaryCallInternalError()
        {
            Assert.Throws<Exception>(
                () => interceptor.BlockingUnaryCall("request", Context(MethodType.Unary, "package.Service", "Method"), (req, ctx) => throw new Exception()));

            metricsMock.Verify(x => x.Started(MethodType.Unary, "package.Service", "Method"));
            metricsMock.Verify(x => x.Handled(MethodType.Unary, "package.Service", "Method", StatusCode.Internal));
        }

        [Fact]
        public async Task AsyncUnaryCall()
        {
            await interceptor.AsyncUnaryCall("request", Context(MethodType.Unary, "package.Service", "Method"), (req, ctx)
                => new AsyncUnaryCall<string>(Task.FromResult("response"), Task.FromResult(new Metadata()), () => new Status(StatusCode.AlreadyExists, ""), () => new Metadata(), () => {}));

            metricsMock.Verify(x => x.Started(MethodType.Unary, "package.Service", "Method"));
            metricsMock.Verify(x => x.Handled(MethodType.Unary, "package.Service", "Method", StatusCode.AlreadyExists));
        }

        [Fact]
        public async Task AsyncClientStreamingCall()
        {
            var writerMock = new Mock<IClientStreamWriter<string>>();
            
            var call = interceptor.AsyncClientStreamingCall(Context(MethodType.Unary, "package.Service", "Method"), ctx
                => new AsyncClientStreamingCall<string, string>(writerMock.Object, Task.FromResult("response"), Task.FromResult(new Metadata()), () => new Status(StatusCode.AlreadyExists, ""), () => new Metadata(), () => {}));
            await call.RequestStream.WriteAsync("request");
            await call.ResponseAsync;

            metricsMock.Verify(x => x.Started(MethodType.ClientStreaming, "package.Service", "Method"));
            metricsMock.Verify(x => x.StreamMsgSent(MethodType.ClientStreaming, "package.Service", "Method"));
            metricsMock.Verify(x => x.Handled(MethodType.ClientStreaming, "package.Service", "Method", StatusCode.AlreadyExists));
        }

        [Fact]
        public async Task AsyncServerStreamingCall()
        {
            var readerMock = new Mock<IAsyncStreamReader<string>>();
            readerMock.SetupSequence(x => x.MoveNext(CancellationToken.None)).ReturnsAsync(true).ReturnsAsync(false);
            readerMock.Setup(x => x.Current).Returns("element");

            var call = interceptor.AsyncServerStreamingCall("request", Context(MethodType.Unary, "package.Service", "Method"), (req, ctx)
                => new AsyncServerStreamingCall<string>(readerMock.Object, Task.FromResult(new Metadata()), () => new Status(StatusCode.AlreadyExists, ""), () => new Metadata(), () => {}));
            Assert.True(await call.ResponseStream.MoveNext());
            await call.ResponseHeadersAsync;

            metricsMock.Verify(x => x.Started(MethodType.ServerStreaming, "package.Service", "Method"));
            metricsMock.Verify(x => x.StreamMsgReceived(MethodType.ServerStreaming, "package.Service", "Method"));
            metricsMock.Verify(x => x.Handled(MethodType.ServerStreaming, "package.Service", "Method", StatusCode.AlreadyExists));
        }

        [Fact]
        public async Task AsyncDuplexStreamingCall()
        {
            var writerMock = new Mock<IClientStreamWriter<string>>();

            var readerMock = new Mock<IAsyncStreamReader<string>>();
            readerMock.SetupSequence(x => x.MoveNext(CancellationToken.None)).ReturnsAsync(true).ReturnsAsync(false);
            readerMock.Setup(x => x.Current).Returns("element");

            var call = interceptor.AsyncDuplexStreamingCall(Context(MethodType.Unary, "package.Service", "Method"), ctx
                => new AsyncDuplexStreamingCall<string, string>(writerMock.Object, readerMock.Object, Task.FromResult(new Metadata()), () => new Status(StatusCode.AlreadyExists, ""), () => new Metadata(), () => {}));
            await call.RequestStream.WriteAsync("request");
            Assert.True(await call.ResponseStream.MoveNext());
            await call.ResponseHeadersAsync;

            metricsMock.Verify(x => x.Started(MethodType.DuplexStreaming, "package.Service", "Method"));
            metricsMock.Verify(x => x.StreamMsgSent(MethodType.DuplexStreaming, "package.Service", "Method"));
            metricsMock.Verify(x => x.StreamMsgReceived(MethodType.DuplexStreaming, "package.Service", "Method"));
            metricsMock.Verify(x => x.Handled(MethodType.DuplexStreaming, "package.Service", "Method", StatusCode.AlreadyExists));
        }

        private static ClientInterceptorContext<string, string> Context(MethodType type, string service, string method)
            => new ClientInterceptorContext<string, string>(new Method<string, string>(type, service, method, Marshallers.StringMarshaller, Marshallers.StringMarshaller), "localhost", new CallOptions());
    }
}