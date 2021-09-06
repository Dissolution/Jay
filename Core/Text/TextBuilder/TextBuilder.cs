using InlineIL;
using Jay.Debugging;
using System;
using System.Buffers;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Runtime.CompilerServices;
using Jay.Exceptions;
using JetBrains.Annotations;

namespace Jay.Text
{
    /// <summary>
    /// A shared builder of text (<see cref="char"/>, <see cref="string"/>, <see cref="ReadOnlySpan{char}"/>) similar to
    /// <see cref="System.Text.StringBuilder"/>
    /// </summary>
    public sealed partial class TextBuilder : TextWriter<TextBuilder>, 
                                              ITextBuilder<TextBuilder>
    {
        private string _newLine = Environment.NewLine;
        
        private char[] _characters;
        private int _length;

        internal Span<char> Written
        {
    
            get => new Span<char>(_characters, 0, _length);
        }

        internal Span<char> Available
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => new Span<char>(_characters, _length, _characters.Length - _length);
        }

        public ref char this[int index]
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => ref _characters[index];
        }

        public Span<char> this[Range range]
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => _characters[range];
        }

        public int Length
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => _length;
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            internal set => _length = value;
        }

        internal int CurrentCapacity
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => _characters.Length;
        }
        
        public TextBuilder()
        {
            _characters = _charArrayPool.Rent(DefaultCapacity);
            _length = 0;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal int Adding(int charCount)
        {
            var len = _length;
            var newLen = len + charCount;
            if (newLen > _characters.Length)
            {
                SwapExpand(ref _characters, newLen * 2);
            }
            _length = newLen;
            return len;
        }

        internal void EnsureCapacity(int minCapacity)
        {
            SwapExpand(ref _characters, minCapacity);
        }

        #region Write

        public override void Write(char c)
        {
            _characters[Adding(1)] = c;
        }
        

        public override void Write(ReadOnlySpan<char> text)
        {
            int textLen = text.Length;
            if (textLen > 0)
            {
                int len = _length;
                int newLen = len + textLen;
                if (newLen > _characters.Length)
                {
                    SwapExpand(ref _characters, newLen * 2);
                }
                TextHelper.Copy(text, Available);
                _length = newLen;
            }
        }

        /// <inheritdoc />
        public override void Write(params char[] text)
        {
            int textLen = text.Length;
            if (textLen > 0)
            {
                int len = _length;
                int newLen = len + textLen;
                if (newLen > _characters.Length)
                {
                    SwapExpand(ref _characters, newLen * 2);
                }
                TextHelper.Copy(text, Available);
                _length = newLen;
            }
        }

        /// <inheritdoc />
        public override void Write(string? text)
        {
            if (text != null)
            {
                int textLen = text.Length;
                if (textLen > 0)
                {
                    int len = _length;
                    int newLen = len + textLen;
                    if (newLen > _characters.Length)
                    {
                        SwapExpand(ref _characters, newLen * 2);
                    }

                    TextHelper.Copy(text, Available);
                    _length = newLen;
                }
            }
        }

        /// <inheritdoc />
        public override void Write(object? obj)
        {
            Write(obj?.ToString());
        }

        /// <inheritdoc />
        public override void Write<T>([AllowNull] T value)
        {
            Write(value?.ToString());
        }

#endregion
        
        #region Append
        #region Append Value
        public override TextBuilder Append(byte value)
        {
            if (value.TryFormat(Available, out int charsWritten))
            {
                _length += charsWritten;
            }
            else
            {
                // We don't know how big ToString will be until we call it
                Write(value.ToString());
            }
            return this;
        }
        
        public override TextBuilder Append(sbyte value)
        {
            if (value.TryFormat(Available, out int charsWritten))
            {
                _length += charsWritten;
            }
            else
            {
                // We don't know how big ToString will be until we call it
                Write(value.ToString());
            }
            return this;
        }
        
        public override TextBuilder Append(short value)
        {
            if (value.TryFormat(Available, out int charsWritten))
            {
                _length += charsWritten;
            }
            else
            {
                // We don't know how big ToString will be until we call it
                Write(value.ToString());
            }
            return this;
        }
        
        public override TextBuilder Append(ushort value)
        {
            if (value.TryFormat(Available, out int charsWritten))
            {
                _length += charsWritten;
            }
            else
            {
                // We don't know how big ToString will be until we call it
                Write(value.ToString());
            }
            return this;
        }
        

        public override TextBuilder Append(int value)
        {
            if (value.TryFormat(Available, out int charsWritten))
            {
                _length += charsWritten;
            }
            else
            {
                // We don't know how big ToString will be until we call it
                Write(value.ToString());
            }
            return this;
        }
        

        public override TextBuilder Append(uint value)
        {
            if (value.TryFormat(Available, out int charsWritten))
            {
                _length += charsWritten;
            }
            else
            {
                // We don't know how big ToString will be until we call it
                Write(value.ToString());
            }
            return this;
        }
        

        public override TextBuilder Append(long value)
        {
            if (value.TryFormat(Available, out int charsWritten))
            {
                _length += charsWritten;
            }
            else
            {
                // We don't know how big ToString will be until we call it
                Write(value.ToString());
            }
            return this;
        }
        

        public override TextBuilder Append(ulong value)
        {
            if (value.TryFormat(Available, out int charsWritten))
            {
                _length += charsWritten;
            }
            else
            {
                // We don't know how big ToString will be until we call it
                Write(value.ToString());
            }
            return this;
        }
        

        public override TextBuilder Append(float value)
        {
            if (value.TryFormat(Available, out int charsWritten))
            {
                _length += charsWritten;
            }
            else
            {
                // We don't know how big ToString will be until we call it
                Write(value.ToString(CultureInfo.CurrentCulture));
            }
            return this;
        }
        

        public override TextBuilder Append(double value)
        {
            if (value.TryFormat(Available, out int charsWritten))
            {
                _length += charsWritten;
            }
            else
            {
                // We don't know how big ToString will be until we call it
                Write(value.ToString(CultureInfo.CurrentCulture));
            }
            return this;
        }
        

        public override TextBuilder Append(decimal value)
        {
            if (value.TryFormat(Available, out int charsWritten))
            {
                _length += charsWritten;
            }
            else
            {
                // We don't know how big ToString will be until we call it
                Write(value.ToString(CultureInfo.CurrentCulture));
            }
            return this;
        }
        

        public override TextBuilder Append(TimeSpan value)
        {
            if (value.TryFormat(Available, out int charsWritten))
            {
                _length += charsWritten;
            }
            else
            {
                // We don't know how big ToString will be until we call it
                Write(value.ToString());
            }
            return this;
        }
        

        public override TextBuilder Append(DateTime value)
        {
            if (value.TryFormat(Available, out int charsWritten))
            {
                _length += charsWritten;
            }
            else
            {
                // We don't know how big ToString will be until we call it
                Write(value.ToString(CultureInfo.CurrentCulture));
            }
            return this;
        }
        

        public override TextBuilder Append(DateTimeOffset value)
        {
            if (value.TryFormat(Available, out int charsWritten))
            {
                _length += charsWritten;
            }
            else
            {
                // We don't know how big ToString will be until we call it
                Write(value.ToString());
            }
            return this;
        }
        

        public override TextBuilder Append(Guid value)
        {
            if (value.TryFormat(Available, out int charsWritten))
            {
                _length += charsWritten;
            }
            else
            {
                // We don't know how big ToString will be until we call it
                Write(value.ToString());
            }
            return this;
        }
#endregion
        
        #region Append Line

        public override TextBuilder AppendNewLine()
        {
            Write(_newLine);
            return this;
        }
        #endregion

        public TextBuilder Terminate(ReadOnlySpan<char> text,
                                     StringComparison comparison = StringComparison.CurrentCulture)
        {
            if (text.Length <= _length)
            {
                if (_characters.Slice(_length - text.Length, text.Length).Equals(text, comparison))
                {
                    return this;
                }
            }
            Write(text);
            return this;
        }
        #endregion

     
        public override TextBuilder AppendRepeat(char character, int count)
        {
            if (count > 0)
            {
                var i = _length;
                var newLen = i + count;
                EnsureCapacity(newLen);
                for (; i < newLen; i++)
                {
                    _characters[i] = character;
                }
                _length = newLen;
            }
            return this;
        }
        
        public override TextBuilder AppendRepeat(ReadOnlySpan<char> text, int count)
        {
            if (count > 0)
            {
                var i = _length;
                var len = text.Length;
                var newLen = i + (count * len);
                EnsureCapacity(newLen);
                for (; i < newLen; i+=len)
                {
                    TextHelper.Copy(text, _characters.Slice(i, len));
                }
                _length = newLen;
            }
            return this;
        }

        /// <inheritdoc />
        public override TextBuilder AppendRepeat(char[] text, int count)
        {
            if (count > 0)
            {
                var i = _length;
                var len = text.Length;
                if (len > 0)
                {
                    var newLen = i + (count * len);
                    EnsureCapacity(newLen);
                    for (; i < newLen; i += len)
                    {
                        TextHelper.Copy(text, _characters.Slice(i, len));
                    }
                    _length = newLen;
                }
            }
            return this;
        }

        /// <inheritdoc />
        public override TextBuilder AppendRepeat(string? text, int count)
        {
            if (count > 0 && text != null)
            {
                var i = _length;
                var len = text.Length;
                if (len > 0)
                {
                    var newLen = i + (count * len);
                    EnsureCapacity(newLen);
                    for (; i < newLen; i += len)
                    {
                        TextHelper.Copy(text, _characters.Slice(i, len));
                    }
                    _length = newLen;
                }
            }
            return this;
        }


#region Trim
        public TextBuilder Trim()
        {
            return TrimStart().TrimEnd();
        }

        public TextBuilder TrimStart()
        {
            int i = 0;
            var chars = _characters;
            int len = _length;
            while (i < len && char.IsWhiteSpace(chars[i]))
            {
                i++;
            }
            TextHelper.Copy(Written.Slice(i, len - i), chars);
            _length = len - i;
            return this;
        }

        /// <inheritdoc />
        public TextBuilder TrimStart(char character)
        {
            int i = 0;
            var chars = _characters;
            int len = _length;
            while (i < len && chars[i] == character)
            {
                i++;
            }
            TextHelper.Copy(Written.Slice(i, len - i), chars);
            _length = len - i;
            return this;
        }

        /// <inheritdoc />
        public TextBuilder TrimStart(ReadOnlySpan<char> text, StringComparison comparison = StringComparison.CurrentCulture)
        {
            throw new NotImplementedException();
        }

        /// <inheritdoc />
        public TextBuilder TrimStart(Func<char, bool> isTrimChar)
        {
            int i = 0;
            var chars = _characters;
            int len = _length;
            while (i < len && isTrimChar(chars[i]))
            {
                i++;
            }
            TextHelper.Copy(Written.Slice(i, len - i), chars);
            _length = len - i;
            return this;
        }

        public TextBuilder TrimEnd()
        {
            int i = _length - 1;
            while (i >= 0 && char.IsWhiteSpace(_characters[i]))
            {
                i--;
            }
            _length = i + 1;
            return this;
        }
        
        public TextBuilder TrimEnd(char character)
        {
            int i = _length - 1;
            while (i >= 0 && _characters[i] == character)
            {
                i--;
            }
            _length = i + 1;
            return this;
        }
        
        public TextBuilder TrimEnd(ReadOnlySpan<char> text, StringComparison comparison = StringComparison.CurrentCulture)
        {
            if (_length >= text.Length)
            {
                var lastSlice = _characters[Range.StartAt(Index.FromEnd(text.Length))];
                if (TextHelper.Equals(lastSlice, text, comparison))
                {
                    _length -= text.Length;
                }
            }
            return this;
        }

        /// <inheritdoc />
        public TextBuilder TrimEnd(Func<char, bool> isTrimChar)
        {
            int i = _length - 1;
            while (i >= 0 && isTrimChar(_characters[i]))
            {
                i--;
            }
            _length = i + 1;
            return this;
        }

#endregion
        
        public TextBuilder Clear()
        {
            _length = 0;
            return this;
        }
        
        #region Transform

        public TextBuilder Transform(Func<char, char> transform)
        {
            if (_length > 0)
            {
                var chars = _characters;
                int i = 0;
                ref char c = ref chars[i];
                do
                {
                    c = transform(c);
                    i++;
                } while (i < _length);
            }
            return this;
        }

        public TextBuilder Transform(Func<char, int, char> transform)
        {
            if (_length > 0)
            {
                var chars = _characters;
                int i = 0;
                ref char c = ref chars[i];
                do
                {
                    c = transform(c, i);
                    i++;
                } while (i < _length);
            }
            return this;
        }

        /// <inheritdoc />
        public TextBuilder Transform(RefAction<char> transform)
        {
            if (_length > 0)
            {
                var chars = _characters;
                int i = 0;
                while (i < _length)
                {
                    transform(ref chars[i]);
                    i++;
                }
            }
            return this;
        }

#endregion
        
        public override void Dispose()
        {
            _charArrayPool.Return(_characters);
            _characters = Array.Empty<char>();
            _length = 0;
        }

        public bool Equals(TextBuilder? builder)
        {
            if (builder is null)
                return _length == 0;
            return Written.SequenceEqual(builder.Written);
        }
        
        public bool Equals(string? text)
        {
            return Written.SequenceEqual(text);
        }
        
        public bool Equals(params char[]? text)
        {
            return Written.SequenceEqual(text);
        }
        
        public override bool Equals(object? obj)
        {
            if (obj is string str)
                return Written.SequenceEqual(str);
            if (obj is char[] chars)
                return Written.SequenceEqual(chars);
            return false;
        }


        public override string ToString() => new string(_characters, 0, _length);
    }
}