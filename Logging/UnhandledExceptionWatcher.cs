using System;

namespace Jay.Logging
{
    public abstract class UnhandledExceptionWatcher : IDisposable
    {
        public event EventHandler<ExceptionEventArgs>? UnhandledException;

        protected UnhandledExceptionWatcher()
        {
            
        }

        protected void OnException(object? sender, string watcher, Exception exception)
        {
            this.UnhandledException?.Invoke(sender ?? this, new ExceptionEventArgs(watcher, exception));
        }

        /// <inheritdoc />
        public abstract void Dispose();
    }
}