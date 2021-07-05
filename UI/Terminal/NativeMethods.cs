using Jay.CLI.Native;
using System;
using System.Runtime.InteropServices;

namespace Jay.CLI
{
    internal static class NativeMethods
    {
        [DllImport("kernel32.dll", SetLastError = true, CharSet = CharSet.Auto)]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool WriteConsoleOutput(
            IntPtr hConsoleOutput, 
            TermPos[] lpBuffer, 
            USize dwBufferSize, 
            UPoint dwBufferCoord, 
            ref URect lpWriteRegion);
        
        [DllImport("kernel32.dll", SetLastError = true, CharSet = CharSet.Auto)]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool ReadConsoleOutput(IntPtr hConsoleOutput, 
                                                    [Out] TermPos[] lpBuffer, 
                                                    USize dwBufferSize, 
                                                    UPoint dwBufferCoord,
                                                    ref URect lpReadRegion);
        
        [StructLayout(LayoutKind.Sequential)]
        public struct UPoint
        {
            public static readonly UPoint Empty = new UPoint(0, 0);
            
            public ushort X;
            public ushort Y;

            public UPoint(int x, int y)
            {
                this.X = (ushort) x;
                this.Y = (ushort) y;
            }
        };
        
        [StructLayout(LayoutKind.Sequential)]
        public struct USize
        {
            public static readonly USize Empty = new USize(0, 0);
            
            public ushort Width;
            public ushort Height;

            public USize(int width, int height)
            {
                this.Width = (ushort) width;
                this.Height = (ushort) height;
            }
        };
       
        
        [StructLayout(LayoutKind.Sequential)]
        public struct URect
        {
            public ushort Left;
            public ushort Top;
            public ushort Right;
            public ushort Bottom;

            public UPoint Location => new UPoint(Left, Top);
            public USize Size => new USize(Right - Left, Bottom - Top);
            
            public URect(int left, int top, int right, int bottom)
            {
                this.Left = (ushort) left;
                this.Top = (ushort) top;
                this.Right = (ushort) right;
                this.Bottom = (ushort) bottom;
            }
        }
        
        private const int STD_OUTPUT_HANDLE = -11;

        [DllImportAttribute("kernel32.dll", SetLastError = true, CharSet = CharSet.Auto)]
        private static extern IntPtr GetStdHandle(int nStdHandle);
        
        public static IntPtr GetConsoleOutHandle()
        {
            return GetStdHandle(STD_OUTPUT_HANDLE);
        }
    }
}