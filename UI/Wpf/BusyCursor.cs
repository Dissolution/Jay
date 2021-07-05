using System;
using System.Windows.Input;
using System.Windows.Threading;

namespace Jay.UI.Wpf
{
	/// <summary>
	/// Utility class for showing a <see cref="Cursors.Wait"/> until the application becomes idle.
	/// </summary>
	public static class BusyCursor
	{
		#region Fields
		/// <summary>
		///   A value indicating whether the UI is currently busy
		/// </summary>
		private static bool _isBusy;

		/// <summary>
		/// Static interval so we don't have to instantiate every time.
		/// </summary>
		private static readonly TimeSpan _interval = TimeSpan.Zero;
		#endregion

		#region Properties
		/// <summary>
		/// Shows a <see cref="Cursors.Wait"/> until this object is disposed.
		/// </summary>
		public static IDisposable Wait => new WaitCursor();
		#endregion

		#region Private Methods
		/// <summary>
		/// Handles the Tick event of the dispatcherTimer control.
		/// </summary>
		/// <param name="sender">The source of the event.</param>
		/// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
		private static void Timer_Tick(object? sender, EventArgs e)
		{
			if (sender is DispatcherTimer dispatcherTimer)
			{
				SetBusyState(false);
				dispatcherTimer.Stop();
			}
		}

		private static void SetBusyState(bool busy)
		{
			if (busy != _isBusy)
			{
				_isBusy = busy;
				Mouse.OverrideCursor = busy ? Cursors.Wait : null;
				if (_isBusy)
				{
					new DispatcherTimer(_interval, DispatcherPriority.ApplicationIdle, Timer_Tick, Dispatcher.CurrentDispatcher);
					//Application.Current.Dispatcher);
				}
			}
		}
		#endregion

		#region Public Methods
		/// <summary>
		/// Shows a <see cref="Cursors.Wait"/> until the application is idle.
		/// </summary>
		public static void SetBusyState()
		{
			SetBusyState(true);
		}
		#endregion


	}
}
