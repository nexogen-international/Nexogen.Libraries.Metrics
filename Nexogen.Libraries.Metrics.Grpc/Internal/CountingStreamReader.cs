using System;
using System.Threading;
using System.Threading.Tasks;
using Grpc.Core;

namespace Nexogen.Libraries.Metrics.Grpc.Internal
{
    internal class CountingStreamReader<T> : IAsyncStreamReader<T>
        where T : class
    {
        private readonly IAsyncStreamReader<T> innerStream;
        private readonly Action callback;

        public CountingStreamReader(IAsyncStreamReader<T> innerStream, Action callback)
        {
            this.innerStream = innerStream;
            this.callback = callback;
        }

        public T Current => innerStream.Current;

        public async Task<bool> MoveNext(CancellationToken cancellationToken)
        {
            if (await innerStream.MoveNext())
            {
                callback();
                return true;
            }

            return false;
        }
    }
}
