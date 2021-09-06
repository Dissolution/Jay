using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Jay
{
    public static class Disposable
    {
        private static readonly NoDisposable _noDisposable = new NoDisposable();

        /// <summary>
        /// An <see cref="IDisposable"/> that does nothing when <see cref="M:IDisposable.Dispose"/> is called.
        /// </summary>
        public static IDisposable None => _noDisposable;

        public static IAsyncDisposable NoneAsync => _noDisposable;

        public static IDisposable Combine(params IDisposable?[]? disposables)
        {
            if (disposables is null)
                return _noDisposable;
            return new CombinedDisposable(disposables);
        }
        
        public static IDisposable Combine(IEnumerable<IDisposable?>? disposables)
        {
            if (disposables is null)
                return _noDisposable;
            return new CombinedDisposable(disposables);
        }
        
        public static IAsyncDisposable Combine(params IAsyncDisposable?[]? disposables)
        {
            if (disposables is null)
                return _noDisposable;
            return new CombinedAsyncDisposable(disposables);
        }
        
        public static IAsyncDisposable Combine(IEnumerable<IAsyncDisposable?>? disposables)
        {
            if (disposables is null)
                return _noDisposable;
            return new CombinedAsyncDisposable(disposables);
        }

        public static IDisposable Action(Action? action)
        {
            return new ActionDisposable(action);
        }
        
        public static IAsyncDisposable AsyncAction(Func<Task> asyncAction)
        {
            return new ActionAsyncDisposable(asyncAction);
        }
    }
}