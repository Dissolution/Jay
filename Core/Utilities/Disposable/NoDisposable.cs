using System;
using System.Threading.Tasks;

namespace Jay
{
    /// <summary>
    /// An <see cref="IDisposable"/> / <see cref="IAsyncDisposable"/> that doesn't do anything.
    /// </summary>
    internal sealed class NoDisposable : IDisposable, IAsyncDisposable
    {
        public void Dispose() { }

        public ValueTask DisposeAsync() => ValueTask.CompletedTask;
    }
}