using System;
using System.Threading;
using System.Windows.Forms;

namespace Jay.Logging.WinForms
{
    public sealed class UnhandledApplicationExceptionWatcher : UnhandledExceptionWatcher
    {
        public UnhandledApplicationExceptionWatcher()
        {
            //System.Windows.Forms
            Application.SetUnhandledExceptionMode(UnhandledExceptionMode.CatchException);
            Application.ThreadException += ApplicationOnThreadException;
        }

        private void ApplicationOnThreadException(object sender, ThreadExceptionEventArgs e)
        {
            OnException(sender, "Application.ThreadException", e.Exception);
        }

        /// <inheritdoc />
        public override void Dispose()
        {
            Application.SetUnhandledExceptionMode(UnhandledExceptionMode.Automatic);
            Application.ThreadException -= ApplicationOnThreadException;
        }
    }
}