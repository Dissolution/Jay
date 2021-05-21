using System;
using System.Threading.Tasks;

namespace Jay
{
    internal sealed class ActionDisposable : IDisposable
    {
        private Action? _action;

        public ActionDisposable(Action? action)
        {
            _action = action;
        }
            
        public void Dispose()
        {
            _action?.Invoke();
            _action = null;
        }
    }

    internal sealed class ActionAsyncDisposable : IAsyncDisposable
    {
        private Func<Task>? _asyncAction;

        public ActionAsyncDisposable(Func<Task>? asyncAction)
        {
            _asyncAction = asyncAction;
        }

        public async ValueTask DisposeAsync()
        {
            if (_asyncAction != null)
            {
                await _asyncAction.Invoke();
                _asyncAction = null;
            }
        }
    }
}