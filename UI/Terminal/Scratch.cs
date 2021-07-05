using Jay.CLI.Native;
using Jay.Concurrency;
using Jay.Geometry;
using Jay.Text;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;

namespace Jay.CLI.Scratch
{
    public interface ITerminalReader : IDisposable
    {
        TermPos this[int index] { get; }
        TermPos this[int x, int y] { get; }
        TermPos this[Point point] { get; }
        
        int Width { get; }
        int Height { get; }
        int Length { get; }
        Size Size { get; }

        ReadOnlySpan<TermPos> Slice() => Slice(0, Length);
        ReadOnlySpan<TermPos> Slice(int index) => Slice(index, Length - index);
        ReadOnlySpan<TermPos> Slice(int index, int length);

        string ToString() => ToString(Range.All, Environment.NewLine);
        string ToString(Range range) => ToString(range, Environment.NewLine);
        string ToString(Range range, ReadOnlySpan<char> newLine);

        bool TryCopyTo(Span<char> destination) => TryCopyTo(destination, Range.All, Environment.NewLine);
        bool TryCopyTo(Span<char> destination, Range range) => TryCopyTo(destination, range, Environment.NewLine);
        bool TryCopyTo(Span<char> destination, ReadOnlySpan<char> newLine) => TryCopyTo(destination, Range.All, newLine);
        bool TryCopyTo(Span<char> destination, Range range, ReadOnlySpan<char> newLine);
    }

    public interface ITerminalWriter : ITerminalReader, IDisposable
    {
        new ref TermPos this[int index] { get; }
        new ref TermPos this[int x, int y] { get; }
        new ref TermPos this[Point point] { get; }
        
        new Span<TermPos> Slice() => Slice(0, Length);
        new Span<TermPos> Slice(int index) => Slice(index, Length - index);
        new Span<TermPos> Slice(int index, int length);

        void Write(int index, ReadOnlySpan<char> text);
        void Write(int index, ReadOnlySpan<TermPos> terminalPositions);
    }

    internal class TerminalReader : ITerminalReader,
                                    IDisposable
    {
        protected readonly TerminalBuffer _terminalBuffer;
        protected readonly TermPos[] _buffer;

        TermPos ITerminalReader.this[int index] => _buffer[index];
        TermPos ITerminalReader.this[int x, int y] => _buffer[GetIndex(x, y)];
        TermPos ITerminalReader.this[Point point] => _buffer[GetIndex(point)];

        public int Width => _terminalBuffer.Width;
        public int Height => _terminalBuffer.Height;
        public int Length => _buffer.Length;
        public Size Size => new Size(Width, Height);
        
        internal TerminalReader(TerminalBuffer buffer)
        {
            _terminalBuffer = buffer;
            _buffer = buffer._buffer;
        }

        protected int GetIndex(int x, int y) => x + (y * this.Width);
        protected int GetIndex(Point point) => point.X + (point.Y * this.Width);

        public virtual void Dispose()
        {
            _terminalBuffer.ExitReadLock();
        }

        public ReadOnlySpan<TermPos> Slice(int index, int length)
        {
            return new ReadOnlySpan<TermPos>(_buffer, index, length);
        }

        public string ToString(Range range, ReadOnlySpan<char> newLine)
        {
            using (var text = TextBuilder.Rent())
            {
                (int offset, int length) = range.GetOffsetAndLength(_buffer.Length);
                var maxOffset = offset + length;
                int x = offset;
                while (true)
                {
                    while (x < Width && offset < maxOffset)
                    {
                        text.Append(_buffer[offset++].UnicodeChar);
                        x++;
                    }

                    if (offset >= maxOffset)
                    {
                        break;
                    }
                    x = 0;
                    text.Append(newLine);
                }
                return text.ToString();
            }
        }

        public bool TryCopyTo(Span<char> destination, Range range, ReadOnlySpan<char> newLine)
        {
            throw new NotImplementedException();
        }
    }

    internal class TerminalWriter : TerminalReader,
                                    ITerminalWriter,
                                    IDisposable
    {
        ref TermPos ITerminalWriter.this[int index] => ref _buffer[index];
        ref TermPos ITerminalWriter.this[int x, int y] => ref _buffer[GetIndex(x, y)];
        ref TermPos ITerminalWriter.this[Point point] => ref _buffer[GetIndex(point)];

        
        internal TerminalWriter(TerminalBuffer buffer)
            : base(buffer)
        {
            
        }
     
        public new Span<TermPos> Slice(int index, int length)
        {
            return _buffer.Slice(index, length);
        }

        public void Write(int index, ReadOnlySpan<char> text)
        {
            for (var i = 0; i < text.Length; i++)
            {
                _buffer[index++].UnicodeChar = text[i];
            }
        }

        public void Write(int index, ReadOnlySpan<TermPos> terminalPositions)
        {
            for (var i = 0; i < terminalPositions.Length; i++)
            {
                _buffer[index++] = terminalPositions[i];
            }
        }

        public override void Dispose()
        {
            _terminalBuffer.ExitWriteLock();
        }
    }
    

}