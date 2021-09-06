using System;
using System.Threading;
using System.Threading.Tasks;

namespace Jay.Threading
{
	/// <summary>
	/// Extensions for <see cref="WaitHandle"/>s.
	/// </summary>
	public static class WaitHandleExtensions
	{
		/// <summary>
		/// Blocks the thread until the current <see cref="WaitHandle"/> receives a signal.
		/// </summary>
		/// <param name="waitHandle"></param>
		/// <returns></returns>
		public static bool Wait(this WaitHandle waitHandle)
		{
			return waitHandle.WaitOne();
		}

		/// <summary>
		/// Blocks the thread until the current <see cref="WaitHandle"/> receives a signal, using a <see cref="TimeSpan"/> to specify the timeout.
		/// </summary>
		/// <param name="waitHandle"></param>
		/// <param name="timeout"></param>
		/// <returns></returns>
		public static bool Wait(this WaitHandle waitHandle, TimeSpan timeout)
		{
			return waitHandle.WaitOne(timeout);
		}

		/// <summary>
		/// Blocks the thread until the current <see cref="WaitHandle"/> receives a signal, using a count of Milliseconds to specify the timeout.
		/// </summary>
		/// <param name="waitHandle"></param>
		/// <param name="millisecondsTimeout"></param>
		/// <returns></returns>
		public static bool Wait(this WaitHandle waitHandle, int millisecondsTimeout)
		{
			return waitHandle.WaitOne(millisecondsTimeout);
		}

		/// <summary>
		/// Sets up a <see cref="Task"/>&lt;<see cref="bool"/>&gt; that waits until the <see cref="WaitHandle"/> receives a signal.
		/// </summary>
		/// <param name="waitHandle"></param>
		/// <returns></returns>
		public static Task<bool> WaitAsync(this WaitHandle waitHandle,
		                                   int millisecondsTimeout = -1,
		                                   CancellationToken token = default)
		{
			if (waitHandle is null)
				throw new ArgumentNullException(nameof(waitHandle));

			var taskCompletionSource = new TaskCompletionSource<bool>();
			var tokenRegistration = token.Register(() => taskCompletionSource.SetCanceled());
			var registeredWaitHandle = ThreadPool.RegisterWaitForSingleObject(
			                                                                  //WaitObject
			                                                                  waitHandle,
			                                                                  //WaitOrTimerCallback
			                                                                  (state, timedOut) =>
			                                                                  {
				                                                                  try
				                                                                  {
					                                                                  if (!token.IsCancellationRequested)
						                                                                  taskCompletionSource.SetResult(!timedOut);
					                                                                  else
						                                                                  taskCompletionSource.SetCanceled(token);
				                                                                  }
				                                                                  catch (InvalidOperationException)
				                                                                  {
					                                                                  taskCompletionSource.SetCanceled(token);
				                                                                  }
				                                                                  catch (TaskCanceledException)
				                                                                  {
					                                                                  taskCompletionSource.SetCanceled(token);
				                                                                  }
				                                                                  catch (Exception ex)
				                                                                  {
					                                                                  taskCompletionSource.SetException(ex);
				                                                                  }
			                                                                  },
			                                                                  //State
			                                                                  null,
			                                                                  //TimeoutInterval
			                                                                  millisecondsTimeout,
			                                                                  //Execute Only Once
			                                                                  true);
			//Our task
			var task = taskCompletionSource.Task;
			//Continues with cleanup
			task.ContinueWith(_ =>
			{
				registeredWaitHandle.Unregister(null);
				Result.Dispose(tokenRegistration);
			}, token);
			//Done
			return task;
		}

		/// <summary>
		/// Sets up a <see cref="Task"/>&lt;<see cref="bool"/>&gt; that waits until the <see cref="WaitHandle"/> receives a signal or times out.
		/// </summary>
		/// <param name="waitHandle"></param>
		/// <param name="timeout"></param>
		/// <returns></returns>
		public static Task<bool> WaitAsync(this WaitHandle waitHandle, 
		                                   TimeSpan timeout,
		                                   CancellationToken token = default)
		{
			return WaitAsync(waitHandle, (int) timeout.TotalMilliseconds, token);
		}

		/// <summary>
		/// Sets up a <see cref="Task"/>&lt;<see cref="bool"/>&gt; that waits until the <see cref="WaitHandle"/> receives a signal or is cancelled.
		/// </summary>
		/// <param name="waitHandle"></param>
		/// <param name="token"></param>
		/// <returns></returns>
		public static Task<bool> WaitAsync(this WaitHandle waitHandle, 
		                                   CancellationToken token = default)
		{
			return WaitAsync(waitHandle, -1, token);
		}
	}
}
