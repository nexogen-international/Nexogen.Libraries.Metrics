using System;
using System.Threading.Tasks;
using Grpc.Core;

namespace Nexogen.Libraries.Metrics.Prometheus.Grpc.Internal
{
    internal class CountingStreamWriter<T> : IServerStreamWriter<T> 
    {
        private readonly IServerStreamWriter<T> innerStream;
        private readonly Action callback;

        public CountingStreamWriter(IServerStreamWriter<T> innerStream, Action callback)
        {
            this.innerStream = innerStream;
            this.callback = callback;
        }

        public WriteOptions WriteOptions
        {
            get => innerStream.WriteOptions;
            set => innerStream.WriteOptions = value;
        }

        public Task WriteAsync(T message)
        {
            callback();
            return innerStream.WriteAsync(message);
        }
    }
}
