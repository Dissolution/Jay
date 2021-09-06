using System;
using System.ComponentModel;
using System.Runtime.InteropServices;

namespace Jay.Sandbox.Native
{
    internal static unsafe class NativeMethods
    {
        private const int STD_INPUT_HANDLE = -10;
        private const int STD_OUTPUT_HANDLE = -11;
        private const int STD_ERROR_HANDLE = -12;
        private static readonly IntPtr INVALID_HANDLE_VALUE = new IntPtr(-1);
        
        static NativeMethods()
        {
            
        }

        [DllImport("kernel32.dll", SetLastError = true, CharSet = CharSet.Unicode)]
        internal static extern IntPtr GetStdHandle(int nStdHandle);

        [DllImport("kernel32.dll", SetLastError = true)]
        internal static extern bool GetConsoleScreenBufferInfo(IntPtr hConsoleOutput,
                                                               ref ConsoleScreenBufferInfo lpConsoleScreenBufferInfo);
        
        [DllImport("kernel32.dll", EntryPoint = "WriteConsoleOutputW", CharSet = CharSet.Unicode, SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        internal static extern bool WriteConsoleOutput(IntPtr hConsoleOutput,
                                                       Coxel[] lpBuffer,
                                                       ShortSize dwBufferSize,
                                                       ShortPoint dwBufferShortPoint,
                                                       ref ShortRect lpWriteRegion);

        [DllImport("kernel32.dll", CharSet = CharSet.Unicode, SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        internal static extern bool ReadConsoleOutput(IntPtr hConsoleOutput,
                                                      [Out] Coxel[] lpBuffer,
                                                      ShortSize dwBufferSize,
                                                      ShortPoint dwBufferShortPoint,
                                                      ref ShortRect lpReadRegion);
                                                          
        internal static IntPtr GetConsoleHandle()
        {
            IntPtr handle = GetStdHandle(STD_OUTPUT_HANDLE);
            if (handle == INVALID_HANDLE_VALUE)
            {
                int errorId = Marshal.GetLastWin32Error();
                throw new Win32Exception(errorId);
            }
            return handle;
        }

        [StructLayout(LayoutKind.Explicit, Size = 22)]
        public struct ConsoleScreenBufferInfo
        {
            [FieldOffset(0)]
            public ShortSize Size;
            [FieldOffset(4)]
            public ShortPoint CursorPosition;
            [FieldOffset(8)]
            public CoxelColors Colors;
            [FieldOffset(9)] 
            public CommonLvb CommonLvb;
            [FieldOffset(10)]
            public ShortRect Window;
            [FieldOffset(18)]
            public ShortSize MaximumWindowSize;
        }
    }
}