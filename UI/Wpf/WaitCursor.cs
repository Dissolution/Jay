using System;
using System.Diagnostics;
using System.Threading;
using System.Windows.Input;

namespace Jay.UI.Wpf
{
	internal sealed class WaitCursor : IDisposable
	{
		private static int _lockLevel = 0;
		private static Cursor? _cursor;

		public WaitCursor()
		{
			//On the first increment, set to wait
			if (Interlocked.Increment(ref _lockLevel) == 1)
			{
				_cursor = Mouse.OverrideCursor;
				Mouse.OverrideCursor = Cursors.Wait;
			}
			else
			{
				Debug.Assert(_cursor != null);
			}
		}

		~WaitCursor()
		{
			this.Dispose();
		}

		public void Dispose()
		{
			//If we decrement to 0, set back to original
			if (Interlocked.Decrement(ref _lockLevel) == 0)
			{
				Mouse.OverrideCursor = _cursor!;
			}
			GC.SuppressFinalize(this);
		}
	}
}
