using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Jay.Threading
{
    /// <summary>
    /// A <see cref="Task"/> that waits for a <see cref="CancellationToken"/> to trigger.
    /// </summary>
    public sealed class WaitForCancelTask : Task
    {
        private readonly CancellationToken _token;

        /// <summary>
        /// Construct a new <see cref="WaitForCancelTask"/> that waits for the specified <see cref="CancellationToken"/> to be cancelled.
        /// </summary>
        /// <param name="token"></param>
        public WaitForCancelTask(CancellationToken token)
            : base(() => Action(token), token)
        {
            _token = token;
        }

        private static void Action(object o)
        {
            if (o is CancellationToken token)
            {
                try
                {
                    token.WaitHandle.WaitOne();
                }
                catch (TaskCanceledException)
                {
                    return;
                }
                catch (AggregateException aggEx) 
                    when (aggEx.InnerExceptions.Any(x => x is TaskCanceledException))
                {
                    return;
                }
                catch (Exception ex) 
                    when (ex.InnerException is TaskCanceledException)
                {
                    return;
                }
            }
        }

        /// <inheritdoc />
        protected override void Dispose(bool disposing)
        {
            if (_token.WaitHandle is EventWaitHandle waitHandle)
            {
                waitHandle.Set();
            }
            else
            {
                _token.WaitHandle.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}