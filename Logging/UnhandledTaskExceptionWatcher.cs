using System;
using System.Threading.Tasks;

namespace Jay.Logging
{
    public sealed class UnhandledTaskExceptionWatcher : UnhandledExceptionWatcher
    {
        public UnhandledTaskExceptionWatcher()
        {
            TaskScheduler.UnobservedTaskException += TaskSchedulerOnUnobservedTaskException;
        }

        private void TaskSchedulerOnUnobservedTaskException(object? sender, UnobservedTaskExceptionEventArgs e)
        {
            Exception exception = e.Exception;
            exception.Data["Observed"] = e.Observed;
            OnException(sender, "TaskScheduler.UnobservedTaskException", exception);
        }


        /// <inheritdoc />
        public override void Dispose()
        {
            TaskScheduler.UnobservedTaskException -= TaskSchedulerOnUnobservedTaskException;
        }
    }
}