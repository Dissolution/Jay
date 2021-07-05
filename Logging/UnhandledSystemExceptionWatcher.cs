using System;

namespace Jay.Logging
{
    public sealed class UnhandledSystemExceptionWatcher : UnhandledExceptionWatcher
    {
        public UnhandledSystemExceptionWatcher()
        {
            AppDomain.CurrentDomain.UnhandledException += CurrentDomainOnUnhandledException;
        }

        private void CurrentDomainOnUnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            Exception exception = (e.ExceptionObject as Exception) ?? 
                                  new Exception(e.ExceptionObject?.ToString());
            exception.Data["ExceptionObject"] = e.ExceptionObject;
            exception.Data["IsTerminating"] = e.IsTerminating;
            OnException(sender, "AppDomain.UnhandledException", exception);
        }

        /// <inheritdoc />
        public override void Dispose()
        {
            AppDomain.CurrentDomain.UnhandledException -= CurrentDomainOnUnhandledException;
        }
    }
}