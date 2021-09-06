using System;
using System.Diagnostics.CodeAnalysis;
using System.Drawing;
using Jay.Sandbox.Native;

namespace Jay.Sandbox
{
    public class TerminalText : ITextSection
    {
        private readonly TerminalBuffer _buffer;
        private readonly int _width;
        private readonly int _maxIndex;
        
        private int _index;
        private CoxelColors _colors;

        internal int X
        {
            get => _index % _width;
            set
            {
                //int index = value + ((_index / _width) * _width);
                int altIndex = value + (_index - (_index % _width));
                // if (index != altIndex)
                //     Debugger.Break();
                this.Index = altIndex;
            }
        }
        internal int Y
        {
            get => _index / _width;
            set
            {
                var index = (_index % _width) + (value * _width);
                this.Index = index;
            }
        }
        internal int Index
        {
            get => _index;
            set
            {
                if (value >= _maxIndex)
                {
                    //Reset();
                    _buffer.Flush();
                    _index = 0;
                }
                else
                {
                    _index = value;
                }
            }
        }

        /// <inheritdoc />
        public Size Size => _buffer.Size;

        /// <inheritdoc />
        public Point Position => new Point(X, Y);


        public TerminalText(TerminalBuffer buffer)
        {
            _buffer = buffer;
            _width = buffer.Size.Width;
            _maxIndex = _buffer._buffer.Length;
            _colors = Coxel.Default.Color;
        }

        private void Write(ReadOnlySpan<char> text)
        {
            var buffer = _buffer._buffer;
            int t = 0;
            while (t < text.Length)
            {
                ref var x = ref buffer[Index];
                x.Char.UnicodeChar = text[t++];
                x.Color = _colors;
                Index++;
            }
        }
        
        public ITextSection Append(char unicodeCharacter)
        {
            _buffer[Index].Char.UnicodeChar = unicodeCharacter;
            Index++;
            return this;
        }
        
        public ITextSection Append(ReadOnlySpan<char> text)
        {
            Write(text);
            return this;
        }

        public ITextSection Append(string? text)
        {
            Write(text);
            return this;
        }

        public ITextSection Append(object? value)
        {
            Write(value?.ToString());
            return this;
        }
        
        /// <inheritdoc />
        public ITextSection Append<T>([AllowNull] T value)
        {
            Write(value?.ToString());
            return this;
        }

        public ITextSection NewLine()
        {
            // int oldIndex = _index;
            // X = 0;
            // Y++;
            //
            // int calcIndex = (((oldIndex / _width) + 1) * _width) % _maxIndex;
            // if (calcIndex != _index)
            //     Debugger.Break();
            Index = ((_index / _width) + 1) * _width;
            return this;
        }

        /// <inheritdoc />
        public ITextSection Colors(CoxelColor? foreColor, CoxelColor? backColor)
        {
            if (foreColor.TryGetValue(out var fc))
            {
                _colors.Foreground = fc;
            }

            if (backColor.TryGetValue(out var bc))
            {
                _colors.Background = bc;
            }

            return this;
        }

        /// <inheritdoc />
        public ITextSection Colors(CoxelColors colors)
        {
            _colors = colors;
            return this;
        }
        
        
        /// <inheritdoc />
        public ITextSection Colored(CoxelColor? foreColor, CoxelColor? backColor, Action<ITextSection> coloredAction)
        {
            var oldColors = _colors;
            Colors(foreColor, backColor);
            coloredAction(this);
            _colors = oldColors;
            return this;
        }

        /// <inheritdoc />
        public ITextSection Colored(CoxelColors colors, Action<ITextSection> coloredAction)
        {
            var oldColors = _colors;
            _colors = colors;
            coloredAction(this);
            _colors = oldColors;
            return this;
        }

        public ITextSection ColorWrite(CoxelColors colors, object? value)
        {
            var oldColors = _colors;
            _colors = colors;
            Append(value?.ToString());
            _colors = oldColors;
            return this;
        }
        
        public ITextSection ColorWrite<T>(CoxelColors colors, [AllowNull] T value)
        {
            var oldColors = _colors;
            _colors = colors;
            Append(value?.ToString());
            _colors = oldColors;
            return this;
        }

        /// <inheritdoc />
        public IDisposable Colored(CoxelColor? foreColor, CoxelColor? backColor)
        {
            var oldColors = _colors;
            Colors(foreColor, backColor);
            return Disposable.Action(() => _colors = oldColors);
        }

        /// <inheritdoc />
        public IDisposable Colored(CoxelColors colors)
        {
            var oldColors = _colors;
            _colors = colors;
            return Disposable.Action(() => _colors = oldColors);
        }

        /// <inheritdoc />
        public ITextSection ColorAppend<T>(CoxelColor? foreColor, CoxelColor? backColor, [AllowNull] T value)
        {
            var oldColors = _colors;
            Colors(foreColor, backColor);
            Write(value?.ToString());
            _colors = oldColors;
            return this;
        }

        /// <inheritdoc />
        public ITextSection ColorAppend<T>(CoxelColors colors, [AllowNull] T value)
        {
            var oldColors = _colors;
            _colors = colors;
            Write(value?.ToString());
            _colors = oldColors;
            return this;
        }
        
        public void Clear()
        {
            var color = Coxel.Default.Color;
            _buffer.ForEach((ref Coxel coxel) =>
            {
                coxel.Char = ' ';
                coxel.Color = color;
            });
            _buffer.Flush();
        }
        
        public void Clear(Coxel template)
        {
            _buffer.ForEach((ref Coxel coxel) => coxel = template);
            _buffer.Flush();
        }
    }
}