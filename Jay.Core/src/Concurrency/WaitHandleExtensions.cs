// ReSharper disable MethodSupportsCancellation

namespace Jay.Concurrency;

public static class WaitHandleExtensions
{
    /// <summary>
    /// Sets up a Task that waits until the WaitHandle receives a signal.
    /// </summary>
    /// <param name="waitHandle"></param>
    /// <returns></returns>
    public static Task<bool> WaitAsync(this WaitHandle waitHandle)
    {
        return WaitAsync(waitHandle, -1, CancellationToken.None);
    }

    /// <summary>
    /// Sets up a Task that waits until the WaitHandle receives a signal or is cancelled.
    /// </summary>
    /// <param name="waitHandle"></param>
    /// <param name="token"></param>
    /// <returns></returns>
    public static Task<bool> WaitAsync(this WaitHandle waitHandle, CancellationToken token)
    {
        return WaitAsync(waitHandle, -1, token);
    }

    /// <summary>
    /// Sets up a Task that waits until the WaitHandle receives a signal or times out.
    /// </summary>
    /// <param name="waitHandle"></param>
    /// <param name="timeout"></param>
    /// <returns></returns>
    public static Task<bool> WaitAsync(this WaitHandle waitHandle, TimeSpan timeout)
    {
        return WaitAsync(waitHandle, (int)timeout.TotalMilliseconds, CancellationToken.None);
    }

    /// <summary>
    /// Sets up a Task that waits until the WaitHandle receives a signal or times out.
    /// </summary>
    /// <param name="waitHandle"></param>
    /// <param name="millisecondsTimeout"></param>
    /// <returns></returns>
    public static Task<bool> WaitAsync(this WaitHandle waitHandle, int millisecondsTimeout)
    {
        return WaitAsync(waitHandle, millisecondsTimeout, CancellationToken.None);
    }

    /// <summary>
    /// Sets up a Task that waits until the WaitHandle has received a signal, a cancellation is signalled, or times out.
    /// </summary>
    /// <param name="waitHandle"></param>
    /// <param name="token"></param>
    /// <param name="timeout"></param>
    /// <returns></returns>
    public static Task<bool> WaitAsync(this WaitHandle waitHandle, TimeSpan timeout, CancellationToken token)
    {
        return WaitAsync(waitHandle, (int)timeout.TotalMilliseconds, token);
    }

    /// <summary>
    /// Sets up a Task that waits until the WaitHandle has received a signal, a cancellation is signalled, or times out.
    /// </summary>
    /// <param name="waitHandle"></param>
    /// <param name="token"></param>
    /// <param name="millisecondsTimeout"></param>
    /// <returns></returns>
    /// <see href="https://gist.github.com/AArnott/1084951"/>
    public static Task<bool> WaitAsync(this WaitHandle waitHandle, int millisecondsTimeout, CancellationToken token)
    {
        if (waitHandle == null)
            throw new ArgumentNullException(nameof(waitHandle));

        var taskCompletionSource = new TaskCompletionSource<bool>();
        var localVariableInitLock = new object();
        lock (localVariableInitLock)
        {
            CancellationTokenRegistration cancellationRegistration;
            cancellationRegistration = token.Register(() => taskCompletionSource.SetCanceled());
            RegisteredWaitHandle? registeredWaitHandle = null;
            registeredWaitHandle = ThreadPool.RegisterWaitForSingleObject(
                waitObject: waitHandle,
                callBack: (_, timedOut) =>
                {
                    try
                    {
                        if (!token.IsCancellationRequested)
                            taskCompletionSource.SetResult(!timedOut);
                        else
                            taskCompletionSource.SetCanceled();
                    }
                    catch (InvalidOperationException)
                    {
                        taskCompletionSource.SetCanceled();
                    }
                    catch (TaskCanceledException)
                    {
                        taskCompletionSource.SetCanceled();
                    }
                    catch (Exception ex)
                    {
                        taskCompletionSource.SetException(ex);
                    }

                    // Unregister
                    lock (localVariableInitLock)
                    {
                        cancellationRegistration.Dispose();
                        // ReSharper disable once AccessToModifiedClosure
                        registeredWaitHandle!.Unregister(null);
                    }
                },
                state: null,
                millisecondsTimeOutInterval: millisecondsTimeout,
                executeOnlyOnce: true);
        }
        return taskCompletionSource.Task;
    }
}