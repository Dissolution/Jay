using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Jay
{
    public static class Disposable
    {
        private sealed class ActionDisposable : IDisposable
        {
            private readonly Action? _action;

            public ActionDisposable(Action? action)
            {
                _action = action;
            }

            public void Dispose()
            {
                _action?.Invoke();
            }
        }

        private sealed class TaskDisposable : IAsyncDisposable
        {
            private readonly Task? _task;

            public TaskDisposable(Task? task)
            {
                _task = task;
            }

            public async ValueTask DisposeAsync()
            {
                if (_task is not null)
                {
                    await _task;
                }
            }
        }

        public static IDisposable Action(Action? action)
        {
            return new ActionDisposable(action);
        }

        public static IAsyncDisposable Task(Task? task)
        {
            return new TaskDisposable(task);
        }
    }


    
}
