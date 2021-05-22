using InlineIL;
using Jay.Debugging;
using System;
using System.Buffers;
using System.Collections.Generic;
using System.Globalization;
using System.Runtime.CompilerServices;

namespace Jay.Text
{
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
            _characters = _charArrayPool.Rent(DefaultCapacity);
            _length = 0;
        }

        [MethodImpl(MethodImplOptions.NoInlining)]
        private void ResizeTo(int minCapacity)
        {
            var newArray = _charArrayPool.Rent(minCapacity);
            Written.CopyTo(newArray);
            _charArrayPool.Return(_characters);
            _characters = newArray;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal void EnsureCapacity(int minCapacity)
        {
            if (minCapacity >= _characters.Length)
                ResizeTo(minCapacity * 2);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal void Write(char c)
        {
            int len = _length;
            if (len >= _characters.Length)
                ResizeTo(len + 1);
            _length = len + 1;
            _characters[len] = c;
        }
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal void Write(ReadOnlySpan<char> text)
        {
            int textLen = text.Length;
            if (textLen > 0)
            {
                int newLen = _length + textLen;
                if (newLen > _characters.Length)
                    ResizeTo(newLen);
                TextHelper.Copy(text, _characters.Slice(_length, textLen));
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
            if (Formatter<byte>.TryFormat(value, Available, out int charsWritten))
            {
                _length += charsWritten;
            }
            else
            {
                Write(value.ToString());
            }
            return this;
        }
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public TextBuilder Append(sbyte value)
        {
            if (Formatter<sbyte>.TryFormat(value, Available, out int charsWritten))
            {
                _length += charsWritten;
            }
            else
            {
                Write(value.ToString());
            }
            return this;
        }
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public TextBuilder Append(short value)
        {
            if (Formatter<short>.TryFormat(value, Available, out int charsWritten))
            {
                _length += charsWritten;
            }
            else
            {
                Write(value.ToString());
            }
            return this;
        }
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public TextBuilder Append(ushort value)
        {
            if (Formatter<ushort>.TryFormat(value, Available, out int charsWritten))
            {
                _length += charsWritten;
            }
            else
            {
                Write(value.ToString());
            }
            return this;
        }
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public TextBuilder Append(int value)
        {
            if (Formatter<int>.TryFormat(value, Available, out int charsWritten))
            {
                _length += charsWritten;
            }
            else
            {
                Write(value.ToString());
            }
            return this;
        }
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public TextBuilder Append(uint value)
        {
            if (Formatter<uint>.TryFormat(value, Available, out int charsWritten))
            {
                _length += charsWritten;
            }
            else
            {
                Write(value.ToString());
            }
            return this;
        }
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public TextBuilder Append(long value)
        {
            if (Formatter<long>.TryFormat(value, Available, out int charsWritten))
            {
                _length += charsWritten;
            }
            else
            {
                Write(value.ToString());
            }
            return this;
        }
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public TextBuilder Append(ulong value)
        {
            if (Formatter<ulong>.TryFormat(value, Available, out int charsWritten))
            {
                _length += charsWritten;
            }
            else
            {
                Write(value.ToString());
            }
            return this;
        }
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public TextBuilder Append(float value)
        {
            if (Formatter<float>.TryFormat(value, Available, out int charsWritten))
            {
                _length += charsWritten;
            }
            else
            {
                Write(value.ToString(CultureInfo.CurrentCulture));
            }
            return this;
        }
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public TextBuilder Append(double value)
        {
            if (Formatter<double>.TryFormat(value, Available, out int charsWritten))
            {
                _length += charsWritten;
            }
            else
            {
                Write(value.ToString(CultureInfo.CurrentCulture));
            }
            return this;
        }
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public TextBuilder Append(decimal value)
        {
            if (Formatter<decimal>.TryFormat(value, Available, out int charsWritten))
            {
                _length += charsWritten;
            }
            else
            {
                Write(value.ToString(CultureInfo.CurrentCulture));
            }
            return this;
        }
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public TextBuilder Append(TimeSpan value)
        {
            if (Formatter<TimeSpan>.TryFormat(value, Available, out int charsWritten))
            {
                _length += charsWritten;
            }
            else
            {
                Write(value.ToString());
            }
            return this;
        }
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public TextBuilder Append(DateTime value)
        {
            if (Formatter<DateTime>.TryFormat(value, Available, out int charsWritten))
            {
                _length += charsWritten;
            }
            else
            {
                Write(value.ToString(CultureInfo.CurrentCulture));
            }
            return this;
        }
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public TextBuilder Append(DateTimeOffset value)
        {
            if (Formatter<DateTimeOffset>.TryFormat(value, Available, out int charsWritten))
            {
                _length += charsWritten;
            }
            else
            {
                Write(value.ToString());
            }
            return this;
        }
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public TextBuilder Append(Guid value)
        {
            if (Formatter<Guid>.TryFormat(value, Available, out int charsWritten))
            {
                _length += charsWritten;
            }
            else
            {
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
            if (Formatter<T?>.TryFormat(value, Available, out int charsWritten))
            {
                _length += charsWritten;
            }
            else
            {
                Write(value?.ToString());
            }
            return this;
        }
        #endregion

        public TextBuilder Append(BuildText? buildText)
        {
            buildText?.Invoke(this);
            return this;
        }
        
        #region Append Line

        public TextBuilder AppendLine()
        {
            Write(Environment.NewLine);
            return this;
        }
        #endregion

        public TextBuilder AppendIf(bool check,
                                    Action<TextBuilder> ifTrue,
                                    Action<TextBuilder> ifFalse)
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
        
        public TextBuilder AppendIf(Func<bool> predicate,
                                    Action<TextBuilder> ifTrue,
                                    Action<TextBuilder> ifFalse)
        {
            if (predicate())
            {
                ifTrue(this);
            }
            else
            {
                ifFalse(this);
            }
            return this;
        }
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public TextBuilder AppendFormat<T>(T? value,
                                           string? format = null,
                                           IFormatProvider? provider = null)
        {
            if (value is not null)
            {
                if (Formatter<T>.TryFormat(value, 
                                           this.Available, 
                                           out int charsWritten,
                                           format, 
                                           provider))
                {
                    _length += charsWritten;
                }
                else if (value is IFormattable formattable)
                {
                    Write(formattable.ToString(format, provider));
                }
                else
                {
                    Write(value.ToString());
                }
            }
            return this;
        }

        public TextBuilder AppendJoin<T>(params T?[]? values)
        {
            if (values != null)
            {
                for (var i = 0; i < values.Length; i++)
                {
                    Append<T>(values[i]);
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
                    Append<T>(value);
                }
            }
            return this;
        }

        public TextBuilder AppendDelimit<T>(char delimiter, params T?[]? values)
        {
            if (values != null)
            {
                int len = values.Length;
                if (len > 0)
                {
                    Append<T>(values[0]);
                }
                for (var i = 1; i < len; i++)
                {
                    Write(delimiter);
                    Append<T>(values[i]);
                }
            }
            return this;
        }
        
        public TextBuilder AppendDelimit<T>(ReadOnlySpan<char> delimiter, params T?[]? values)
        {
            if (values != null)
            {
                int len = values.Length;
                if (len > 0)
                {
                    Append<T>(values[0]);
                }
                for (var i = 1; i < len; i++)
                {
                    Write(delimiter);
                    Append<T>(values[i]);
                }
            }
            return this;
        }
        
        public TextBuilder AppendDelimit<T>(char delimiter, ReadOnlySpan<T> values)
        {
            int len = values.Length;
            if (len > 0)
            {
                Append<T>(values[0]);
            }
            for (var i = 1; i < len; i++)
            {
                Write(delimiter);
                Append<T>(values[i]);
            }
            return this;
        }
        
        public TextBuilder AppendDelimit<T>(ReadOnlySpan<char> delimiter, ReadOnlySpan<T> values)
        {
            int len = values.Length;
            if (len > 0)
            {
                Append<T>(values[0]);
            }
            for (var i = 1; i < len; i++)
            {
                Write(delimiter);
                Append<T>(values[i]);
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
                    Append<T>(e.Current);
                    while (e.MoveNext())
                    {
                        Write(delimiter);
                        Append<T>(e.Current);
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
                    Append<T>(e.Current);
                    while (e.MoveNext())
                    {
                        Write(delimiter);
                        Append<T>(e.Current);
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
        
        public TextBuilder AppendDelimit<T>(char delimiter, IEnumerable<T?>? values, BuildText<T>? action)
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
        
        public TextBuilder AppendDelimit<T>(ReadOnlySpan<char> delimiter, IEnumerable<T>? values, BuildText<T>? action)
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
                ResizeTo(newLen);
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
                ResizeTo(newLen);
                for (; i < newLen; i+=len)
                {
                    TextHelper.Copy(text, _characters.Slice(i, len));
                }
                _length = newLen;
            }
            return this;
        }
        
        public void Dispose()
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

        public override int GetHashCode() => GetHashCodeException.Throw(typeof(TextBuilder));

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public override string ToString() => new string(_characters, 0, _length);
    }
}