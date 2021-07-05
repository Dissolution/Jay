using System;

namespace Jay.GhostInput.Input
{
	/// <summary>
	/// DefaultSettings for Input Operations
	/// </summary>
	public sealed class InputSettings
	{
		private bool _snapback;
		private bool _block;
		private bool _ignoreMoveToZero;
		private TimeSpan _inputDelay;

		/// <summary>
		/// Should the mouse return to its starting position after Input Operations?
		/// </summary>
		/// <remarks>Default: False</remarks>
		public bool Snapback
		{
			get { lock(Input.LOCK)return _snapback; }
			set { lock(Input.LOCK) _snapback = value; }
		}

		/// <summary>
		/// Should we block all other Input during the Input Operation?
		/// </summary>
		/// <remarks>Default: True</remarks>
		public bool Block
		{
			get { lock (Input.LOCK) return _block; }
			set { lock (Input.LOCK) _block = value; }
		}

		/// <summary>
		/// Should we ignore Mouse Movements to (0,0) (POINT.Zero, the default value)?
		/// </summary>
		/// <remarks>Default: False</remarks>
		public bool IgnoreMoveToZero
		{
			get { lock (Input.LOCK) return _ignoreMoveToZero; }
			set { lock (Input.LOCK) _ignoreMoveToZero = value; }
		}

		/// <summary>
		/// Gets or sets the range of timespans to wait between subsequent inputs.
		/// </summary>
		/// <remarks>Default: TimeSpan.Zero</remarks>
		public TimeSpan InputDelay
		{
			get { lock (Input.LOCK) return _inputDelay; }
			set { lock (Input.LOCK) _inputDelay = value; }
		}

		public InputSettings()
		{
			_snapback = false;
			_block = true;
			_ignoreMoveToZero = false;
			_inputDelay = TimeSpan.Zero;
		}
	}
}
