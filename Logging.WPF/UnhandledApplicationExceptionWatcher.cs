using System;
using System.Threading;
using System.Windows;
using System.Windows.Threading;

namespace Jay.Logging.WPF
{
    public sealed class UnhandledApplicationExceptionWatcher : UnhandledExceptionWatcher
    {
        public UnhandledApplicationExceptionWatcher()
        {
            Application.Current.DispatcherUnhandledException += WpfAppOnDispatcherUnhandledException;
        }

        private void WpfAppOnDispatcherUnhandledException(object sender, DispatcherUnhandledExceptionEventArgs e)
        {
            var exception = e.Exception;
            exception.Data[nameof(e.Handled)] = e.Handled;
            exception.Data[nameof(e.Dispatcher)] = e.Dispatcher?.GetType();
            OnException(sender, "Application.DispatcherUnhandledException", exception);
        }

        /// <inheritdoc />
        public override void Dispose()
        {
            Application.Current.DispatcherUnhandledException -= WpfAppOnDispatcherUnhandledException;
        }
    }
}