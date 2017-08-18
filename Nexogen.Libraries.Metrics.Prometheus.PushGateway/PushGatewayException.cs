using System;

namespace Nexogen.Libraries.Prometheus.PushGateway
{
    internal class PushGatewayException : Exception
    {
        public PushGatewayException()
        {
        }

        public PushGatewayException(string message) : base(message)
        {
        }

        public PushGatewayException(string message, Exception innerException) : base(message, innerException)
        {
        }
    }
}