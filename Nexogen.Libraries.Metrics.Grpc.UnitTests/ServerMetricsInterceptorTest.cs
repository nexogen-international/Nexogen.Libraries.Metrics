using System;
using System.Threading;
using System.Threading.Tasks;
using Grpc.Core;
using Grpc.Core.Testing;
using Grpc.Core.Utils;
using Moq;
using Xunit;

namespace Nexogen.Libraries.Metrics.Grpc.UnitTests
{
    public class ServerMetricsInterceptorTest
    {
        private readonly Mock<IGrpcServerMetrics> metricsMock = new Mock<IGrpcServerMetrics>();
        private readonly ServerMetricsInterceptor interceptor;

        public ServerMetricsInterceptorTest()
        {
            interceptor = new ServerMetricsInterceptor(metricsMock.Object);
        }

        [Fact]
        public async Task UnaryServerHandler()
        {
            await interceptor.UnaryServerHandler("request", Context("package.Service.Method"), (req, ctx) => Task.FromResult("response"));

            metricsMock.Verify(x => x.Started(MethodType.Unary, "package.Service", "Method"));
            metricsMock.Verify(x => x.Handled(MethodType.Unary, "package.Service", "Method", StatusCode.OK));
        }
        
        [Fact]
        public async Task UnaryServerHandlerStatusCode()
        {
            await Assert.ThrowsAsync<RpcException>(
                () => interceptor.UnaryServerHandler("request", Context("package.Service.Method"), (req, ctx) => Task.FromException<string>(new RpcException(new Status(StatusCode.AlreadyExists, "")))));

            metricsMock.Verify(x => x.Started(MethodType.Unary, "package.Service", "Method"));
            metricsMock.Verify(x => x.Handled(MethodType.Unary, "package.Service", "Method", StatusCode.AlreadyExists));
        }

        [Fact]
        public async Task UnaryServerHandlerInternalError()
        {
            await Assert.ThrowsAsync<Exception>(
                () => interceptor.UnaryServerHandler("request", Context("package.Service.Method"), (req, ctx) => Task.FromException<string>(new Exception())));

            metricsMock.Verify(x => x.Started(MethodType.Unary, "package.Service", "Method"));
            metricsMock.Verify(x => x.Handled(MethodType.Unary, "package.Service", "Method", StatusCode.Internal));
        }

        [Fact]
        public async Task ClientStreamingServerHandler()
        {
            var readerMock = new Mock<IAsyncStreamReader<string>>();
            readerMock.SetupSequence(x => x.MoveNext(CancellationToken.None)).ReturnsAsync(true).ReturnsAsync(false);
            readerMock.Setup(x => x.Current).Returns("element");

            await interceptor.ClientStreamingServerHandler(readerMock.Object, Context("package.Service.Method"), (reader, ctx) =>
            {
                Assert.True(reader.MoveNext().Result);
                Assert.Equal("element", reader.Current);
                return Task.FromResult("response");
            });

            metricsMock.Verify(x => x.Started(MethodType.ClientStreaming, "package.Service", "Method"));
            metricsMock.Verify(x => x.StreamMsgReceived(MethodType.ClientStreaming, "package.Service", "Method"));
            metricsMock.Verify(x => x.Handled(MethodType.ClientStreaming, "package.Service", "Method", StatusCode.OK));
        }

        [Fact]
        public async Task ClientStreamingServerHandlerStatusCode()
        {
            var readerMock = new Mock<IAsyncStreamReader<string>>();
            readerMock.SetupSequence(x => x.MoveNext(CancellationToken.None)).ReturnsAsync(true).ReturnsAsync(false);
            readerMock.Setup(x => x.Current).Returns("element");

            await Assert.ThrowsAsync<RpcException>(
                () => interceptor.ClientStreamingServerHandler(readerMock.Object, Context("package.Service.Method"), (reader, ctx) =>
                {
                    Assert.True(reader.MoveNext().Result);
                    Assert.Equal("element", reader.Current);
                    return Task.FromException<string>(new RpcException(new Status(StatusCode.AlreadyExists, "")));
                }));

            metricsMock.Verify(x => x.Started(MethodType.ClientStreaming, "package.Service", "Method"));
            metricsMock.Verify(x => x.StreamMsgReceived(MethodType.ClientStreaming, "package.Service", "Method"));
            metricsMock.Verify(x => x.Handled(MethodType.ClientStreaming, "package.Service", "Method", StatusCode.AlreadyExists));
        }

        [Fact]
        public async Task ClientStreamingServerHandlerInternalError()
        {
            var readerMock = new Mock<IAsyncStreamReader<string>>();
            readerMock.SetupSequence(x => x.MoveNext(CancellationToken.None)).ReturnsAsync(true).ReturnsAsync(false);
            readerMock.Setup(x => x.Current).Returns("element");

            await Assert.ThrowsAsync<Exception>(
                () => interceptor.ClientStreamingServerHandler(readerMock.Object, Context("package.Service.Method"), (reader, ctx) =>
                    {
                        Assert.True(reader.MoveNext().Result);
                        Assert.Equal("element", reader.Current);
                        return Task.FromException<string>(new Exception());
                    }));

            metricsMock.Verify(x => x.Started(MethodType.ClientStreaming, "package.Service", "Method"));
            metricsMock.Verify(x => x.StreamMsgReceived(MethodType.ClientStreaming, "package.Service", "Method"));
            metricsMock.Verify(x => x.Handled(MethodType.ClientStreaming, "package.Service", "Method", StatusCode.Internal));
        }

        [Fact]
        public async Task ServerStreamingServerHandler()
        {
            var writerMock = new Mock<IServerStreamWriter<string>>();

            await interceptor.ServerStreamingServerHandler("request", writerMock.Object, Context("package.Service.Method"), async (request, writer, ctx) => await writer.WriteAsync("response"));

            metricsMock.Verify(x => x.Started(MethodType.ServerStreaming, "package.Service", "Method"));
            metricsMock.Verify(x => x.StreamMsgSent(MethodType.ServerStreaming, "package.Service", "Method"));
            metricsMock.Verify(x => x.Handled(MethodType.ServerStreaming, "package.Service", "Method", StatusCode.OK));
        }

        [Fact]
        public async Task DuplexStreamingServerHandler()
        {
            var readerMock = new Mock<IAsyncStreamReader<string>>();
            readerMock.SetupSequence(x => x.MoveNext(CancellationToken.None)).ReturnsAsync(true).ReturnsAsync(false);
            readerMock.Setup(x => x.Current).Returns("element");

            var writerMock = new Mock<IServerStreamWriter<string>>();

            await interceptor.DuplexStreamingServerHandler(readerMock.Object, writerMock.Object, Context("package.Service.Method"), async (reader, writer, ctx) =>
            {
                Assert.True(await reader.MoveNext());
                Assert.Equal("element", reader.Current);

                await writer.WriteAsync("response");
            });

            metricsMock.Verify(x => x.Started(MethodType.DuplexStreaming, "package.Service", "Method"));
            metricsMock.Verify(x => x.StreamMsgSent(MethodType.DuplexStreaming, "package.Service", "Method"));
            metricsMock.Verify(x => x.StreamMsgReceived(MethodType.DuplexStreaming, "package.Service", "Method"));
            metricsMock.Verify(x => x.Handled(MethodType.DuplexStreaming, "package.Service", "Method", StatusCode.OK));
        }

        private static ServerCallContext Context(string method)
            => TestServerCallContext.Create(method, null, DateTime.UtcNow.AddHours(1), new Metadata(), CancellationToken.None, "127.0.0.1", null, null, metadata => TaskUtils.CompletedTask, () => new WriteOptions(), writeOptions => {});
    }
}