using Grpc.Core;

namespace Nexogen.Libraries.Metrics.Grpc
{
    public interface IGrpcMetricsBase
    {
        void Started(MethodType type, string service, string method);

        void Handled(MethodType type, string service, string method, StatusCode statusCode);

        void StreamMsgReceived(MethodType type, string service, string method);

        void StreamMsgSent(MethodType type, string service, string method);
    }
}