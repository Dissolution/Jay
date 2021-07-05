using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Input;
using Jay.GhostInput.Native.Enums;
using Jay.GhostInput.Native.Structs;
using Point = System.Drawing.Point;

// ReSharper disable IdentifierTypo
// ReSharper disable InconsistentNaming


namespace Jay.GhostInput.Native
{
	internal static class NativeMethods
	{
		#region SendInput
		[DllImport("user32.dll", CharSet = CharSet.Unicode)]
		internal static extern short VkKeyScanEx(char ch, IntPtr dwhkl);
		[DllImport("user32.dll")]
		internal static extern bool UnloadKeyboardLayout(IntPtr hkl);
		[DllImport("user32.dll")]
		internal static extern IntPtr LoadKeyboardLayout(string pwszKLID, uint Flags);
		/// <summary>
		/// Synthesizes keystrokes, mouse motions, and button clicks.
		/// </summary>
		/// <param name="nInputs">The number of structures in the pInputs array.</param>
		/// <param name="pInputs">An array of INPUT structures. Each structure represents an event to be inserted into the keyboard or mouse input stream.</param>
		/// <param name="cbSize">The size, in bytes, of an INPUT structure. If cbSize is not the size of an INPUT structure, the function fails.</param>
		/// <returns>The function returns the number of events that it successfully inserted into the keyboard or mouse input stream. If the function returns zero, the input was already blocked by another thread. ConvertTo get extended error information, call GetLastError.</returns>
		[DllImport("user32.dll", SetLastError = true)]
		private static extern uint SendInput(
			uint nInputs,
			[MarshalAs(UnmanagedType.LPArray), In] INPUT[] pInputs,
			int cbSize);

		/// <summary>
		/// Synthesizes keystrokes, mouse motions, and button clicks.
		/// </summary>
		/// <param name="inputs">An array of INPUT Structures to be executed.</param>
		/// <returns>The number of events successfully executed.</returns>
		public static Result SendInput(INPUT[] inputs)
		{
			if (inputs is null)
				return new ArgumentNullException(nameof(inputs));
			var length = (uint)inputs.Length;
			if (length == 0)
				return true;
			var size = INPUT.Size;

			uint sent = SendInput(length, inputs, size);
			int error = Marshal.GetLastWin32Error();
			if (error != 0)
				return new Win32Exception(error);
			if (sent != length)
				return new InvalidOperationException($"Tried to send {length} INPUTs, only sent {sent}.");

			//Sent all of them
			return true;
		}
		#endregion

		#region Cursor Position
		/// <summary>
		/// Get the mouse cursor position
		/// </summary>
		/// <param name="lpPoint"></param>
		/// <returns></returns>
		[DllImport("user32.dll")]
		[return: MarshalAs(UnmanagedType.Bool)]
		private static extern bool GetCursorPos(out POINT lpPoint);

		/// <summary>
		/// Gets the mouse cursor position.
		/// </summary>
		/// <returns></returns>
		public static Point GetCursorPosition()
		{
			try
			{
				if (GetCursorPos(out POINT p))
					return new Point(p.X, p.Y);
				return Point.Empty;
			}
			catch// (Exception ex)
			{
				return Point.Empty;
			}
		}

		/// <summary>
		/// Set the mouse cursor position
		/// </summary>
		/// <param name="x"></param>
		/// <param name="y"></param>
		/// <returns></returns>
		[DllImport("user32.dll")]
		[return: MarshalAs(UnmanagedType.Bool)]
		private static extern bool SetCursorPos(int x, int y);

		/// <summary>
		/// Set the Mouse Cursor Position
		/// </summary>
		/// <returns></returns>
		public static Result SetCursorPosition(Point p)
		{
			return SetCursorPos(p.X, p.Y);
		}

		/// <summary>
		/// Set the Mouse Cursor Position
		/// </summary>
		/// <param name="x"></param>
		/// <param name="y"></param>
		/// <returns></returns>
		public static Result SetCursorPosition(int x, int y)
		{
			try
			{
				return SetCursorPos(x, y);
			}
			catch (Exception ex)
			{
				return ex;
			}
		}
		#endregion

		#region Keyboard State
		[DllImport("user32.dll", SetLastError = true)]
		[return: MarshalAs(UnmanagedType.Bool)]
		private static extern bool GetKeyboardState(byte[] lpKeyState);

		internal static Result GetKeyboardState(out IReadOnlyDictionary<Key, bool> keyboardState)
		{
			byte[] keys = new byte[256];
			var result = GetKeyboardState(keys);
			if (!result)
			{
				keyboardState = new Dictionary<Key, bool>(0);
				return false;
			}
			var dict = new Dictionary<Key, bool>(256);
			for (var i = 0; i < 256; i++)
			{
				var key = (Key) i;
				var code = (i & 0xFF);
				var pressed = (keys[code] & 0x80) != 0;
				dict[key] = pressed;
			}
			keyboardState = dict;
			return true;
		}
		#endregion

		#region Block Input
		/// <summary>The BlockInput function blocks keyboard and mouse input events from reaching applications
		/// </summary>
		/// <param name="fBlockIt">[in] Specifies the function's purpose. If this parameter is non-zero, keyboard
		/// and mouse input events are blocked. If this parameter is zero, keyboard and mouse events are unblocked.
		/// </param>
		/// <returns>If the function succeeds, the return value is nonzero.
		/// If input is already blocked, the return value is zero.
		/// To get extended error information, call Marshal.GetLastWin32Error()</returns>
		/// <remarks>Note that only the thread that blocked input can successfully unblock input</remarks>
		[DllImport("user32.dll", EntryPoint = "BlockInput", ExactSpelling = true, SetLastError = true)]
		[return: MarshalAs(UnmanagedType.Bool)]
		private static extern bool BlockInput([MarshalAs(UnmanagedType.Bool)] bool fBlockIt);

		/// <summary>
		/// Block keyboard and mouse input events.
		/// </summary>
		/// <returns></returns>
		public static Result BlockInput()
		{
			return BlockInput(true);
		}

		/// <summary>
		/// Stop blocking keyboard and mouse events.
		/// </summary>
		/// <returns></returns>
		public static Result UnblockInput()
		{
			return BlockInput(false);
		}
		#endregion

		#region SendMessage
		[DllImport("user32.dll", CharSet = CharSet.Auto)]
		internal static extern IntPtr SendMessage(IntPtr hWnd, uint Msg, IntPtr wParam, IntPtr lParam);
		#endregion

		#region SetFocus
		/// <summary>
		/// Sets the keyboard focus to the specified window. The window must be attached to the calling thread's message queue.
		/// </summary>
		/// <param name="hWnd"></param>
		/// <returns>If the function succeeds, the hanlde to the window that previously had keyboard focus.
		/// If the hWnd parameter was invalid or null, returns null.
		/// Supports GetLastError()</returns>
		[DllImport("user32.dll", SetLastError = true)]
		private static extern IntPtr SetFocus(IntPtr hWnd);

		/// <summary>
		/// Sets the input focus to the specified window.
		/// </summary>
		/// <param name="windowHandle"></param>
		/// <returns></returns>
		public static Result<IntPtr> SetWindowFocus(IntPtr windowHandle)
		{
			return Result<IntPtr>.Try(() => SetFocus(windowHandle));
		}
		#endregion

		#region GetKeyState
		/// <summary>
		/// The GetKeyState function retrieves the status of the specified Virtual Key.
		/// The status specifies whether the key is up, down, or toggled (on, off -- alternating each time the key is pressed).
		/// </summary>
		/// <param name="key"></param>
		/// <returns></returns>
		[DllImport("user32.dll", SetLastError = true)]
		internal static extern short GetKeyState(Key key);


		[DllImport("user32.dll")]
		internal static extern short GetAsyncKeyState(Key key);
		#endregion


		[DllImport("gdi32.dll")]
		private static extern int GetDeviceCaps(IntPtr hdc, int nIndex);

		/// <summary>
		/// Get the bounds of the entire screen area.
		/// </summary>
		/// <returns></returns>
		public static Rect GetScreenBounds()
		{
			IntPtr desktop = IntPtr.Zero;
			var xDPI = GetDeviceCaps(desktop, (int) DeviceCap.LOGPIXELSX);
			var yDPI = GetDeviceCaps(desktop, (int) DeviceCap.LOGPIXELSY);
			return new Rect(0, 0, xDPI, yDPI);
		}
	}
}
