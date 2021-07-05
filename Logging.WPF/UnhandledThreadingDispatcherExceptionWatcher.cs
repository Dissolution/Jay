using System;
using System.Threading;
using System.Windows;
using System.Windows.Threading;

namespace Jay.Logging.WPF
{
    public sealed class UnhandledThreadingDispatcherExceptionWatcher : UnhandledExceptionWatcher
    {
        public UnhandledThreadingDispatcherExceptionWatcher()
        {
            Dispatcher.CurrentDispatcher.UnhandledException += CurrentDispatcherOnUnhandledException;
        }

        private void CurrentDispatcherOnUnhandledException(object sender, DispatcherUnhandledExceptionEventArgs e)
        {
            var exception = e.Exception;
            exception.Data[nameof(e.Handled)] = e.Handled;
            exception.Data[nameof(e.Dispatcher)] = e.Dispatcher?.GetType();
            OnException(sender, "Dispatcher.UnhandledException", exception);
        }

        /// <inheritdoc />
        public override void Dispose()
        {
            Dispatcher.CurrentDispatcher.UnhandledException -= CurrentDispatcherOnUnhandledException;
        }
    }
}