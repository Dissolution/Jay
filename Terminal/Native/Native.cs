using System;
using System.Runtime.InteropServices;

namespace Jay.Consolas
{
	internal static class Native
	{
		[DllImport("kernel32.dll", SetLastError = true)]
		internal static extern IntPtr GetConsoleWindow();

		[DllImport("user32.dll", SetLastError = true)]
		internal static extern bool MoveWindow(IntPtr hWnd, int X, int Y, int nWidth, int nHeight, bool bRepaint);

		[DllImport("user32.dll", CharSet = CharSet.Auto, CallingConvention = CallingConvention.StdCall, ExactSpelling = true, SetLastError = true)]
		internal static extern bool GetWindowRect(IntPtr hWnd, ref RECT rect);

		[DllImport("user32.dll")]
		internal static extern bool ShowWindow(IntPtr hWnd, int nCmdShow);
	}
}
