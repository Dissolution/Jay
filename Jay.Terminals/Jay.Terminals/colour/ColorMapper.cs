using System;
using System.Collections.Generic;
using System.Drawing;
using System.Runtime.InteropServices;
using Jay.Consolas.Palette;
using Jay.Terminals.Native;

namespace Jay.Consolas
{
    /// <summary>
    /// Exposes methods used for mapping System.Drawing.Colors to System.ConsoleColors.
    /// Based on code that was originally written by Alex Shvedov, and that was then modified by MercuryP.
    /// </summary>
    internal sealed class ColorMapper
    {
        [StructLayout(LayoutKind.Sequential)]
        private struct COORD
        {
            internal short X;
            internal short Y;
        }

        [StructLayout(LayoutKind.Sequential)]
        private struct SHORT_RECT
        {
            internal short Left;
            internal short Top;
            internal short Right;
            internal short Bottom;
        }

        [StructLayout(LayoutKind.Sequential)]
        private struct CONSOLE_SCREEN_BUFFER_INFO_EX
        {
            internal int cbSize;
            internal COORD dwSize;
            internal COORD dwCursorPosition;
            internal ushort wAttributes;
            internal SHORT_RECT srWindow;
            internal COORD dwMaximumWindowSize;
            internal ushort wPopupAttributes;
            internal bool bFullscreenSupported;
            internal COLORREF black;
            internal COLORREF darkBlue;
            internal COLORREF darkGreen;
            internal COLORREF darkCyan;
            internal COLORREF darkRed;
            internal COLORREF darkMagenta;
            internal COLORREF darkYellow;
            internal COLORREF gray;
            internal COLORREF darkGray;
            internal COLORREF blue;
            internal COLORREF green;
            internal COLORREF cyan;
            internal COLORREF red;
            internal COLORREF magenta;
            internal COLORREF yellow;
            internal COLORREF white;
        }

        private const int STD_OUTPUT_HANDLE = -11;                               // per WinBase.h
        private static readonly IntPtr INVALID_HANDLE_VALUE = new IntPtr(-1);    // per WinBase.h

        [DllImport("kernel32.dll", SetLastError = true)]
        private static extern IntPtr GetStdHandle(int nStdHandle);

        [DllImport("kernel32.dll", SetLastError = true)]
        private static extern bool GetConsoleScreenBufferInfoEx(IntPtr hConsoleOutput, ref CONSOLE_SCREEN_BUFFER_INFO_EX csbe);

        [DllImport("kernel32.dll", SetLastError = true)]
        private static extern bool SetConsoleScreenBufferInfoEx(IntPtr hConsoleOutput, ref CONSOLE_SCREEN_BUFFER_INFO_EX csbe);

        /// <summary>
        /// Maps a <see cref="Color"/> to a <see cref="ConsoleColor"/>.
        /// </summary>
        /// <param name="oldColor">The consoleColor to be replaced.</param>
        /// <param name="newColor">The consoleColor to be mapped.</param>
        public void MapColor(ConsoleColor oldColor, Color newColor)
        {
            var hConsoleOutput = GetStdHandle(STD_OUTPUT_HANDLE); // 7
            var bufferInfo = GetBufferInfo(hConsoleOutput);

            switch (oldColor)
            {
                case ConsoleColor.Black:
                    bufferInfo.black = new COLORREF(newColor);
                    break;
                case ConsoleColor.DarkBlue:
                    bufferInfo.darkBlue = new COLORREF(newColor);
                    break;
                case ConsoleColor.DarkGreen:
                    bufferInfo.darkGreen = new COLORREF(newColor);
                    break;
                case ConsoleColor.DarkCyan:
                    bufferInfo.darkCyan = new COLORREF(newColor);
                    break;
                case ConsoleColor.DarkRed:
                    bufferInfo.darkRed = new COLORREF(newColor);
                    break;
                case ConsoleColor.DarkMagenta:
                    bufferInfo.darkMagenta = new COLORREF(newColor);
                    break;
                case ConsoleColor.DarkYellow:
                    bufferInfo.darkYellow = new COLORREF(newColor);
                    break;
                case ConsoleColor.Gray:
                    bufferInfo.gray = new COLORREF(newColor);
                    break;
                case ConsoleColor.DarkGray:
                    bufferInfo.darkGray = new COLORREF(newColor);
                    break;
                case ConsoleColor.Blue:
                    bufferInfo.blue = new COLORREF(newColor);
                    break;
                case ConsoleColor.Green:
                    bufferInfo.green = new COLORREF(newColor);
                    break;
                case ConsoleColor.Cyan:
                    bufferInfo.cyan = new COLORREF(newColor);
                    break;
                case ConsoleColor.Red:
                    bufferInfo.red = new COLORREF(newColor);
                    break;
                case ConsoleColor.Magenta:
                    bufferInfo.magenta = new COLORREF(newColor);
                    break;
                case ConsoleColor.Yellow:
                    bufferInfo.yellow = new COLORREF(newColor);
                    break;
                case ConsoleColor.White:
                    bufferInfo.white = new COLORREF(newColor);
                    break;
            }

            SetBufferInfo(hConsoleOutput, bufferInfo);
        }

        /// <summary>
        /// Gets a collection of all 16 colors in the console buffer.
        /// </summary>
        /// <returns>Returns all 16 COLORREFs in the console buffer as a dictionary keyed by the COLORREF's alias
        /// in the buffer's ColorTable.</returns>
        public Dictionary<ConsoleColor, COLORREF> GetBufferColors()
        {
			var hConsoleOutput = GetStdHandle(STD_OUTPUT_HANDLE);    // 7
            var bufferInfo = GetBufferInfo(hConsoleOutput);
	        var colors = new Dictionary<ConsoleColor, COLORREF>
	        {
		        {ConsoleColor.Black, bufferInfo.black},
		        {ConsoleColor.DarkBlue, bufferInfo.darkBlue},
		        {ConsoleColor.DarkGreen, bufferInfo.darkGreen},
		        {ConsoleColor.DarkCyan, bufferInfo.darkCyan},
		        {ConsoleColor.DarkRed, bufferInfo.darkRed},
		        {ConsoleColor.DarkMagenta, bufferInfo.darkMagenta},
		        {ConsoleColor.DarkYellow, bufferInfo.darkYellow},
		        {ConsoleColor.Gray, bufferInfo.gray},
		        {ConsoleColor.DarkGray, bufferInfo.darkGray},
		        {ConsoleColor.Blue, bufferInfo.blue},
		        {ConsoleColor.Green, bufferInfo.green},
		        {ConsoleColor.Cyan, bufferInfo.cyan},
		        {ConsoleColor.Red, bufferInfo.red},
		        {ConsoleColor.Magenta, bufferInfo.magenta},
		        {ConsoleColor.Yellow, bufferInfo.yellow},
		        {ConsoleColor.White, bufferInfo.white}
	        };

	        return colors;
        }

        /// <summary>
        /// Sets all 16 colors in the console buffer using colors supplied in a dictionary.
        /// </summary>
        /// <param name="colors">A dictionary containing COLORREFs keyed by the COLORREF's alias in the buffer's 
        /// ColorTable.</param>
        public void SetBatchBufferColors(Dictionary<ConsoleColor, COLORREF> colors)
        {
            var hConsoleOutput = GetStdHandle(STD_OUTPUT_HANDLE); // 7
            var bufferInfo = GetBufferInfo(hConsoleOutput);

            bufferInfo.black = colors[ConsoleColor.Black];
            bufferInfo.darkBlue = colors[ConsoleColor.DarkBlue];
            bufferInfo.darkGreen = colors[ConsoleColor.DarkGreen];
            bufferInfo.darkCyan = colors[ConsoleColor.DarkCyan];
            bufferInfo.darkRed = colors[ConsoleColor.DarkRed];
            bufferInfo.darkMagenta = colors[ConsoleColor.DarkMagenta];
            bufferInfo.darkYellow = colors[ConsoleColor.DarkYellow];
            bufferInfo.gray = colors[ConsoleColor.Gray];
            bufferInfo.darkGray = colors[ConsoleColor.DarkGray];
            bufferInfo.blue = colors[ConsoleColor.Blue];
            bufferInfo.green = colors[ConsoleColor.Green];
            bufferInfo.cyan = colors[ConsoleColor.Cyan];
            bufferInfo.red = colors[ConsoleColor.Red];
            bufferInfo.magenta = colors[ConsoleColor.Magenta];
            bufferInfo.yellow = colors[ConsoleColor.Yellow];
            bufferInfo.white = colors[ConsoleColor.White];

            SetBufferInfo(hConsoleOutput, bufferInfo);
        }

	    public void SetBufferColors(IPalette palette)
	    {
		    var handle = GetStdHandle(STD_OUTPUT_HANDLE);
		    var bufferInfo = GetBufferInfo(handle);
		    bufferInfo.black = palette.Black;
		    bufferInfo.darkBlue = palette.DarkBlue;
		    bufferInfo.darkGreen = palette.DarkGreen;
		    bufferInfo.darkCyan = palette.DarkCyan;
		    bufferInfo.darkRed = palette.DarkRed;
		    bufferInfo.darkMagenta = palette.DarkMagenta;
		    bufferInfo.darkYellow = palette.DarkYellow;
		    bufferInfo.gray = palette.Gray;
		    bufferInfo.darkGray = palette.DarkGray;
		    bufferInfo.blue = palette.Blue;
		    bufferInfo.green = palette.Green;
		    bufferInfo.cyan = palette.Cyan;
		    bufferInfo.red = palette.Red;
		    bufferInfo.magenta = palette.Magenta;
		    bufferInfo.yellow = palette.Yellow;
		    bufferInfo.white = palette.White;

		    SetBufferInfo(handle, bufferInfo);

	    }

        private CONSOLE_SCREEN_BUFFER_INFO_EX GetBufferInfo(IntPtr hConsoleOutput)
        {
            var bufferInfo = new CONSOLE_SCREEN_BUFFER_INFO_EX();
            bufferInfo.cbSize = Marshal.SizeOf(bufferInfo); // 96 = 0x60

	        if (hConsoleOutput == INVALID_HANDLE_VALUE)
		        throw CreateException(Marshal.GetLastWin32Error());

	        if (!GetConsoleScreenBufferInfoEx(hConsoleOutput, ref bufferInfo))
		        throw CreateException(Marshal.GetLastWin32Error());

	        return bufferInfo;
        }

        private void SetBufferInfo(IntPtr hConsoleOutput, CONSOLE_SCREEN_BUFFER_INFO_EX bufferInfo)
        {
            bufferInfo.srWindow.Bottom++;
            bufferInfo.srWindow.Right++;

	        if (!SetConsoleScreenBufferInfoEx(hConsoleOutput, ref bufferInfo))
		        throw CreateException(Marshal.GetLastWin32Error());
        }

        private static Exception CreateException(int errorCode)
        {
            const int ERROR_INVALID_HANDLE = 6;
            if (errorCode == ERROR_INVALID_HANDLE) // Raised if the console is being run via another application, for example.
            {
                return new ConsoleAccessException();
            }

            return new ColorMappingException(errorCode);
        }
    }
}
