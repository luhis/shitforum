using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace UnitTests.Tooling
{
    internal class TestAsyncEnumerator<T> : IAsyncEnumerator<T>
    {
        private readonly IEnumerator<T> inner;

        public TestAsyncEnumerator(IEnumerator<T> inner)
        {
            this.inner = inner;
        }

        void IDisposable.Dispose()
        {
            inner.Dispose();
        }

        T IAsyncEnumerator<T>.Current => inner.Current;

        Task<bool> IAsyncEnumerator<T>.MoveNext(CancellationToken cancellationToken)
        {
            return Task.FromResult(inner.MoveNext());
        }
    }
}