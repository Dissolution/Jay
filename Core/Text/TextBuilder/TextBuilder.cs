using InlineIL;
using Jay.Debugging;
using System;
using System.Buffers;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Runtime.CompilerServices;
using JetBrains.Annotations;

namespace Jay.Text
{
    /// <summary>
    /// A shared builder of text (<see cref="char"/>, <see cref="string"/>, <see cref="ReadOnlySpan{char}"/>) similar to
    /// <see cref="System.Text.StringBuilder"/>
    /// </summary>
    public sealed partial class TextBuilder : IDisposable
    {
        private char[] _characters;
        private int _length;

        internal Span<char> Written
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
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
            internal set => _length = value;
        }

        internal int Capacity
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => _characters.Length;
        }
        
        private TextBuilder()
        {
            _characters = CharArrayPool.Rent(DefaultCapacity);
            _length = 0;
        }

        [MethodImpl(MethodImplOptions.NoInlining)]
        private void ResizeTo(int minCapacity)
        {
            var newArray = CharArrayPool.Rent(minCapacity);
            TextHelper.Copy(Written, newArray);
            CharArrayPool.Return(_characters);
            _characters = newArray;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal void EnsureCapacity(int minCapacity)
        {
            if (minCapacity > _characters.Length)
            {
                // Since we're a shared pool, if we start getting big needs we want to keep big arrays around
                ResizeTo(minCapacity * 2);
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal int Adding(int charCount)
        {
            var len = _length;
            var newLen = len + charCount;
            if (newLen > _characters.Length)
            {
                ResizeTo(newLen * 2);
            }
            _length = newLen;
            return len;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal void Write(char c)
        {
            _characters[Adding(1)] = c;
        }
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal void Write(ReadOnlySpan<char> text)
        {
            int textLen = text.Length;
            if (textLen > 0)
            {
                int len = _length;
                int newLen = len + textLen;
                if (newLen > _characters.Length)
                {
                    ResizeTo(newLen * 2);
                }
                TextHelper.Copy(text, _characters.Slice(len, textLen));
                _length = newLen;
            }
        }
        
        #region Append
        #region Append Value
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public TextBuilder Append(char c)
        {
            Write(c);
            return this;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public TextBuilder Append(params char[]? chars)
        {
            Write(chars);
            return this;
        }
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public TextBuilder Append(string? text)
        {
            Write(text);
            return this;
        }
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public TextBuilder Append(ReadOnlySpan<char> text)
        {
            Write(text);
            return this;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public TextBuilder Append(bool boolean)
        {
            Write(boolean ? bool.TrueString : bool.FalseString);
            return this;
        }
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public TextBuilder Append(byte value)
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
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public TextBuilder Append(sbyte value)
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
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public TextBuilder Append(short value)
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
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public TextBuilder Append(ushort value)
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
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public TextBuilder Append(int value)
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
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public TextBuilder Append(uint value)
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
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public TextBuilder Append(long value)
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
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public TextBuilder Append(ulong value)
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
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public TextBuilder Append(float value)
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
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public TextBuilder Append(double value)
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
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public TextBuilder Append(decimal value)
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
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public TextBuilder Append(TimeSpan value)
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
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public TextBuilder Append(DateTime value)
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
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public TextBuilder Append(DateTimeOffset value)
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
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public TextBuilder Append(Guid value)
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
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public TextBuilder Append(object? value)
        {
            Write(value?.ToString());
            return this;
        }
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public TextBuilder Append<T>(T? value)
        {
            Write(value?.ToString());
            return this;
        }
        #endregion

        #region Append(Delegate)
        public TextBuilder Append(BuildText buildText)
        {
            buildText.Invoke(this);
            return this;
        }
        
        public TextBuilder Append<TState>(TState state, StateBuildText<TState> buildText)
        {
            buildText.Invoke(this, state);
            return this;
        }
        
        public TextBuilder Append<T>(ReadOnlySpan<T> readOnlySpan, SpanBuildText<T> buildText)
        {
            buildText.Invoke(this, readOnlySpan);
            return this;
        }
        
        public TextBuilder Append(ReadOnlySpan<char> text, TextBuildText buildText)
        {
            buildText.Invoke(this, text);
            return this;
        }
        #endregion
        
        #region Append Line

        public TextBuilder AppendLine()
        {
            Write(Environment.NewLine);
            return this;
        }

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

        public TextBuilder AppendIf(bool check,
                                    BuildText ifTrue,
                                    BuildText ifFalse)
        {
            if (check)
            {
                ifTrue(this);
            }
            else
            {
                ifFalse(this);
            }
            return this;
        }

        public TextBuilder AppendJoin<T>(params T[]? values)
        {
            if (values != null)
            {
                for (var i = 0; i < values.Length; i++)
                {
                    Write(values[i]?.ToString());
                }
            }
            return this;
        }
        
        public TextBuilder AppendJoin<T>(IEnumerable<T?>? values)
        {
            if (values != null)
            {
                foreach (var value in values)
                {
                    Write(value?.ToString());
                }
            }
            return this;
        }

        public TextBuilder AppendDelimit(char delimiter, params string?[]? values)
        {
            if (values != null)
            {
                int len = values.Length;
                if (len > 0)
                {
                    Write(values[0]);
                }
                for (var i = 1; i < len; i++)
                {
                    Write(delimiter);
                    Write(values[i]);
                }
            }
            return this;
        }
        
        public TextBuilder AppendDelimit<T>(char delimiter, params T[]? values)
        {
            if (values != null)
            {
                int len = values.Length;
                if (len > 0)
                {
                    Write(values[0]?.ToString());
                }
                for (var i = 1; i < len; i++)
                {
                    Write(delimiter);
                    Write(values[i]?.ToString());
                }
            }
            return this;
        }
        
        public TextBuilder AppendDelimit<T>(ReadOnlySpan<char> delimiter, params T[]? values)
        {
            if (values != null)
            {
                int len = values.Length;
                if (len > 0)
                {
                    Write(values[0]?.ToString());
                }
                for (var i = 1; i < len; i++)
                {
                    Write(delimiter);
                    Write(values[i]?.ToString());
                }
            }
            return this;
        }
        
        public TextBuilder AppendDelimit<T>(char delimiter, ReadOnlySpan<T> values)
        {
            int len = values.Length;
            if (len > 0)
            {
                Write(values[0]?.ToString());
            }
            for (var i = 1; i < len; i++)
            {
                Write(delimiter);
                Write(values[i]?.ToString());
            }
            return this;
        }
        
        public TextBuilder AppendDelimit<T>(ReadOnlySpan<char> delimiter, ReadOnlySpan<T> values)
        {
            int len = values.Length;
            if (len > 0)
            {
                Write(values[0]?.ToString());
            }
            for (var i = 1; i < len; i++)
            {
                Write(delimiter);
                Write(values[i]?.ToString());
            }
            return this;
        }
        
        public TextBuilder AppendDelimit(char delimiter, IEnumerable<string?>? values)
        {
            if (values != null)
            {
                using (var e = values.GetEnumerator())
                {
                    if (!e.MoveNext())
                        return this;
                    Write(e.Current);
                    while (e.MoveNext())
                    {
                        Write(delimiter);
                        Write(e.Current);
                    }
                }
            }
            return this;
        }
        
        public TextBuilder AppendDelimit<T>(char delimiter, IEnumerable<T?>? values)
        {
            if (values != null)
            {
                using (var e = values.GetEnumerator())
                {
                    if (!e.MoveNext())
                        return this;
                    Write(e.Current?.ToString());
                    while (e.MoveNext())
                    {
                        Write(delimiter);
                        Write(e.Current?.ToString());
                    }
                }
            }
            return this;
        }
        
        public TextBuilder AppendDelimit<T>(ReadOnlySpan<char> delimiter, IEnumerable<T?>? values)
        {
            if (values != null)
            {
                using (var e = values.GetEnumerator())
                {
                    if (!e.MoveNext())
                        return this;
                    Write(e.Current?.ToString());
                    while (e.MoveNext())
                    {
                        Write(delimiter);
                        Write(e.Current?.ToString());
                    }
                }
            }
            return this;
        }
        
        public TextBuilder AppendDelimit<T>(char delimiter, ReadOnlySpan<T> values, Action<TextBuilder, T?>? action)
        {
            if (action != null)
            {
                int len = values.Length;
                if (len > 0)
                {
                    action(this, values[0]);
                }
                for (var i = 1; i < len; i++)
                {
                    Write(delimiter);
                    action(this, values[i]);
                }
            }
            return this;
        }
        
        public TextBuilder AppendDelimit<T>(ReadOnlySpan<char> delimiter, ReadOnlySpan<T> values, Action<TextBuilder, T>? action)
        {
            if (action != null)
            {
                int len = values.Length;
                if (len > 0)
                {
                    action(this, values[0]);
                }
                for (var i = 1; i < len; i++)
                {
                    Write(delimiter);
                    action(this, values[i]);
                }
            }
            return this;
        }
        
        public TextBuilder AppendDelimit<T>(char delimiter, IEnumerable<T>? values, StateBuildText<T>? action)
        {
            if (values != null)
            {
                using (var e = values.GetEnumerator())
                {
                    if (!e.MoveNext())
                        return this;
                    action?.Invoke(this, e.Current);
                    while (e.MoveNext())
                    {
                        Write(delimiter);
                        action?.Invoke(this, e.Current);
                    }
                }
            }
            return this;
        }
        
        public TextBuilder AppendDelimit<T>(ReadOnlySpan<char> delimiter, IEnumerable<T>? values, StateBuildText<T>? action)
        {
            if (values != null)
            {
                using (var e = values.GetEnumerator())
                {
                    if (!e.MoveNext())
                        return this;
                    action?.Invoke(this, e.Current);
                    while (e.MoveNext())
                    {
                        Write(delimiter);
                        action?.Invoke(this, e.Current);
                    }
                }
            }
            return this;
        }
        #endregion

        public TextBuilder AppendAlign(ReadOnlySpan<char> text, 
                                       Alignment alignment, 
                                       int width, 
                                       char trimIndicationChar = '…')
        {
            if (width > 0)
            {
                var len = text.Length;
                if (width == len)
                {
                    Write(text);
                }
                else if (width > len)
                {
                    int gap = width - len;
                    if (alignment == Alignment.Left)
                    {
                        Write(text);
                        for (var i = 0; i < gap; i++)
                            Write(' ');
                    }
                    else if (alignment == Alignment.Right)
                    {
                        for (var i = 0; i < gap; i++)
                            Write(' ');
                        Write(text);
                    }
                    else // alignment.HasFlag(Alignment.Center)
                    {
                        int leftSpaces;
                        int rightSpaces;
                        if (alignment.HasFlag<Alignment>(Alignment.Right))
                        {
                            leftSpaces = Bits.HalfRoundUp(gap);
                            rightSpaces = Bits.HalfRoundDown(gap);
                        }
                        else // alignment.HasFlag(Alignment.Left) || alignment == Alignment.Center
                        {
                            leftSpaces = Bits.HalfRoundDown(gap);
                            rightSpaces = Bits.HalfRoundUp(gap);
                        }
                        
                        for (var i = 0; i < leftSpaces; i++)
                            Write(' ');
                        Write(text);
                        for (var i = 0; i < rightSpaces; i++)
                            Write(' ');
                    }
                }
                else // width < len
                {
                    if (alignment.HasFlag<Alignment>(Alignment.Right))
                    {
                        Write(trimIndicationChar);
                        Write(text[^(width-1)..]);
                    }
                    else
                    {
                        Write(text[..(width-1)]);
                        Write(trimIndicationChar);
                    }
                }
            }
            return this;
        }

        public TextBuilder AppendRepeat(char character, int count)
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
        
        public TextBuilder AppendRepeat(ReadOnlySpan<char> text, int count)
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
        
        
        #region Trim
        public TextBuilder Trim()
        {
            return TrimStart().TrimEnd();
        }

        public TextBuilder TrimStart()
        {
            int i = 0;
            int len = _length;
            while (i < len && char.IsWhiteSpace(_characters[i]))
            {
                i++;
            }
            TextHelper.Copy(Written.Slice(i, len - i), _characters);
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
        
        #endregion
        
        public void Dispose()
        {
            CharArrayPool.Return(_characters);
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

        public override int GetHashCode() => GetHashCodeException.Throw(typeof(TextBuilder));

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public override string ToString() => new string(_characters, 0, _length);
    }
}