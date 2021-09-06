using System;
using System.ComponentModel;
using System.Drawing;
using System.IO.Pipelines;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using Jay.Debugging;
using Jay.Sandbox.Native;
using Microsoft.Toolkit.HighPerformance;

namespace Jay.Sandbox
{
    public delegate void ModifyCoxel(ref Coxel coxel);
    
    public sealed class TerminalBuffer
    {
        public static TerminalBuffer Instance { get; } = new TerminalBuffer();
        
        internal readonly IntPtr _consoleHandle;
        internal readonly Coxel[] _buffer;

        public Size Size { get; }

        public ref Coxel this[int index]
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => ref _buffer[index];
        }

        public ref Coxel this[int x, int y]
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => ref _buffer[x + (y * Size.Width)];
        }

        public Span2D<Coxel> Buffer => new Span2D<Coxel>(_buffer, Size.Height, Size.Width);
        
        public int Length => _buffer.Length;

        private TerminalBuffer()
        {
            _consoleHandle = NativeMethods.GetConsoleHandle();
            var info = new NativeMethods.ConsoleScreenBufferInfo();
            NativeMethods.GetConsoleScreenBufferInfo(_consoleHandle, ref info);
            int width = info.Size.Width;
            int height = info.Size.Height;

            this.Size = new Size(width, height);
            _buffer = new Coxel[width * height];
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private ShortRect GetBufferRect() => new ShortRect(0, 0, Size.Width, Size.Height);

        public void Read()
        {
            var rect = GetBufferRect();
            bool read = NativeMethods.ReadConsoleOutput(_consoleHandle,
                                                        _buffer,
                                                        rect.Size,
                                                        rect.Location,
                                                        ref rect);
            if (!read)
            {
                int errorId = Marshal.GetLastWin32Error();
                var ex = new Win32Exception(errorId);
                Hold.Debug(errorId, ex);
            }
        }

        public void ForEach(ModifyCoxel coxelModifyCoxel)
        {
            var buffer = _buffer;
            int length = buffer.Length;
            for (var i = 0; i < length; i++)
            {
                coxelModifyCoxel(ref buffer[i]);
            }
        }
        
        public void Flush()
        {
            var rect = GetBufferRect();
            bool wrote = NativeMethods.WriteConsoleOutput(_consoleHandle,
                                                          _buffer,
                                                          rect.Size,
                                                          rect.Location,
                                                          ref rect);
            if (!wrote)
            {
                int errorId = Marshal.GetLastWin32Error();
                var ex = new Win32Exception(errorId);
                Hold.Debug(errorId, ex);
            }
        }
        
        public void Clear()
        {
            var color = Coxel.Default.Color;
            ForEach((ref Coxel coxel) =>
            {
                coxel.Char = ' ';
                coxel.Color = color;
            });
            Flush();
        }
        
        public void Clear(Coxel template)
        {
            ForEach((ref Coxel coxel) => coxel = template);
            Flush();
        }
    }
}