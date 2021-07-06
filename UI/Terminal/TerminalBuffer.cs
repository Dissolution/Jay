using System;
using System.Diagnostics;
using System.Drawing;
using System.Threading;
using Jay.UI.Terminal.Native;

namespace Jay.UI.Terminal
{
    public sealed class TerminalBuffer
    {
        private readonly object RING = new object();
        private ReaderWriterLockSlim _lock = new ReaderWriterLockSlim();
        
        private readonly IntPtr _consoleOutHandle;
        internal readonly Coxel[] _buffer;
        
        public int Width { get; }
        public int Height { get; }
        
        public Coxel DefaultTerminalPosition { get; } = new Coxel(' ', TerminalColor.Gray, TerminalColor.Black);

        public ITerminalReader Read
        {
            get
            {
                _lock.EnterReadLock();
                return new TerminalReader(this);
            }
        }

        public ITerminalWriter Write
        {
            get
            {
                _lock.EnterWriteLock();
                return new TerminalWriter(this);
            }
        }
        

        internal TerminalBuffer()
        {
            // TODO:
            this.Width = Console.BufferWidth;
            this.Height = Console.BufferHeight;
            _consoleOutHandle = NativeMethods.GetConsoleOutHandle();
            _buffer = new Coxel[Width * Height];
            
            ReadFromConsole();
        }

        private void ReadFromConsole()
        {
            var bufferRect = GetBufferRect();
            NativeMethods.ReadConsoleOutput(_consoleOutHandle,
                                            _buffer,
                                            bufferRect.Size,
                                            NativeMethods.UPoint.Empty,
                                            ref bufferRect);
            Debug.Assert(bufferRect.Left == 0);
            Debug.Assert(bufferRect.Top == 0);
            Debug.Assert(bufferRect.Right == Width);
            Debug.Assert(bufferRect.Bottom == Height);
        }
        
        private NativeMethods.URect GetBufferRect()
        {
            return new NativeMethods.URect(0, 0, Width, Height);
        }

        private static NativeMethods.URect GetBufferRect(Rectangle rectangle)
        {
            return new NativeMethods.URect(rectangle.Left, rectangle.Top, rectangle.Right, rectangle.Bottom);
        }
        
        private void Flush(NativeMethods.URect rect)
        {
            var wrote = NativeMethods.WriteConsoleOutput(_consoleOutHandle,
                                                         _buffer,
                                                         rect.Size,
                                                         rect.Location,
                                                         ref rect);
            if (!wrote)
            {
                Debugger.Break();
            }
        }

        internal void ExitReadLock()
        {
            _lock.ExitReadLock();
        }

        internal void ExitWriteLock()
        {
            _lock.ExitWriteLock();
        }
        
        public void Flush()
        {
            Flush(GetBufferRect());
        }

        public void Flush(Rectangle area)
        {
            Flush(GetBufferRect(area));
        }

        
    }
}