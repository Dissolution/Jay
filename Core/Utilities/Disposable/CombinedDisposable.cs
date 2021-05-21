using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Jay
{
    internal sealed class CombinedDisposable : IDisposable
    {
        private readonly IDisposable?[] _disposables;

        public CombinedDisposable(params IDisposable?[]? disposables)
        {
            _disposables = disposables ?? Array.Empty<IDisposable>();
        }

        public CombinedDisposable(IEnumerable<IDisposable?>? disposables)
        {
            _disposables = disposables?.ToArray() ?? Array.Empty<IDisposable>();
        }

        public void Dispose()
        {
            for (var i = _disposables.Length - 1; i >= 0; i--)
            {
                _disposables[i]?.Dispose();
            }
        }
    }
    
    internal sealed class CombinedAsyncDisposable : IAsyncDisposable
    {
        private readonly IAsyncDisposable?[] _disposables;

        public CombinedAsyncDisposable(params IAsyncDisposable?[]? disposables)
        {
            _disposables = disposables ?? Array.Empty<IAsyncDisposable>();
        }

        public CombinedAsyncDisposable(IEnumerable<IAsyncDisposable?>? disposables)
        {
            _disposables = disposables?.ToArray() ?? Array.Empty<IAsyncDisposable>();
        }

        public async ValueTask DisposeAsync()
        {
            for (var i = _disposables.Length - 1; i >= 0; i--)
            {
                var disposable = _disposables[i];
                if (disposable != null)
                {
                    await disposable.DisposeAsync();
                }
            }
        }
    }
}