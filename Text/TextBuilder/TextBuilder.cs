using System.Buffers;
using System.Collections;
using System.Runtime.CompilerServices;
using Jay.Exceptions;

namespace Jay.Text.TextBuilder
{
    public partial class TextBuilder
    {
        public static string Build(Action<TextBuilder> buildText)
        {
            using var builder = new TextBuilder();
            buildText(builder);
            return builder.ToString();
        }
    }

    public partial class TextBuilder : IList<char>, IReadOnlyList<char>,
                                       ICollection<char>, IReadOnlyCollection<char>,
                                       IEnumerable<char>, IEnumerable,
                                       IDisposable 
    {
        protected const int MinimumArrayPoolLength = 256;

        protected char[] _chars;
        protected int _length;

        internal Span<char> Text => new Span<char>(_chars);
        internal Span<char> Written => _chars.AsSpan(0, _length);
        internal Span<char> Available => _chars.AsSpan(_length);

        char IList<char>.this[int index]
        {
            get => this[index];
            set => this[index] = value;
        }
        char IReadOnlyList<char>.this[int index]
        {
            get => this[index];
        }
        bool ICollection<char>.IsReadOnly => false;
        int IReadOnlyCollection<char>.Count => _length;
        int ICollection<char>.Count => _length;

        public ref char this[int index]
        {
            get
            {
                if ((uint)index < (uint)_length)
                {
                    return ref _chars[index];
                }

                throw new IndexOutOfRangeException();
            }
        }
        public int Length => _length;

        private TextBuilder()
        {
            _chars = ArrayPool<char>.Shared.Rent(MinimumArrayPoolLength);
            _length = 0;
        }

        /// <summary>Grow the size of <see cref="_chars"/> to at least the specified <paramref name="requiredMinCapacity"/>.</summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)] // but reuse this grow logic directly in both of the above grow routines
        private void GrowCore(uint requiredMinCapacity)
        {
            // We want the max of how much space we actually required and doubling our capacity (without going beyond the max allowed length). We
            // also want to avoid asking for small arrays, to reduce the number of times we need to grow, and since we're working with unsigned
            // ints that could technically overflow if someone tried to, for example, append a huge string to a huge string, we also clamp to int.MaxValue.
            // Even if the array creation fails in such a case, we may later fail in ToStringAndClear.

            uint newCapacity = Math.Max(requiredMinCapacity, Math.Min((uint)_chars.Length * 2, 0x3FFFFFDF));
            int arraySize = (int)Math.Clamp(newCapacity, MinimumArrayPoolLength, int.MaxValue);

            char[] newArray = ArrayPool<char>.Shared.Rent(arraySize);
            _chars.AsSpan(0, _length).CopyTo(newArray);

            char[] toReturn = _chars;
            _chars = newArray;
            ArrayPool<char>.Shared.Return(toReturn);
        }

        private void RemoveCore(int index, int count)
        {
            TextHelper.CopyTo(_chars.AsSpan(index + count), 
                              _chars.AsSpan(index));
            _length -= count;
        }

        private void InsertCore(int index, int count)
        {
            //TODO
        }

        [MethodImpl(MethodImplOptions.NoInlining)]
        private void Grow()
        {
            GrowCore((uint)_chars.Length + 1);
        }
        [MethodImpl(MethodImplOptions.NoInlining)]
        private void Grow(int additionalChars)
        {
            GrowCore((uint)_chars.Length + (uint)additionalChars);
        }

        [MethodImpl(MethodImplOptions.NoInlining)]
        protected void GrowThenCopyString(string value)
        {
            Grow(value.Length);
            value.CopyTo(Available);
            _length += value.Length;
        }

        [MethodImpl(MethodImplOptions.NoInlining)]
        protected void GrowThenCopySpan(text value)
        {
            Grow(value.Length);
            value.CopyTo(Available);
            _length += value.Length;
        }

        void ICollection<char>.Add(char ch) => Append(ch);

        void IList<char>.Insert(int index, char ch) => Insert(index, ch);

        void ICollection<char>.Clear() => Clear();

        void ICollection<char>.CopyTo(char[] array, int arrayIndex) => CopyTo(array.AsSpan(arrayIndex));

        bool ICollection<char>.Remove(char ch)
        {
            for (var i = 0; i < _length; i++)
            {
                if (_chars[i] == ch)
                {
                    RemoveCore(i, 1);
                    return true;
                }
            }
            return false;
        }

        int IList<char>.IndexOf(char ch)
        {
            for (var i = 0; i < _length; i++)
            {
                if (_chars[i] == ch)
                {
                    return i;
                }
            }
            return -1;
        }
        
        public TextBuilder Append(char ch)
        {
            var chars = _chars;
            int index = _length;
            if (index >= chars.Length)
            {
                Grow(1);
            }
           
            chars[index] = ch;
            _length = index + 1;
            return this;
        }

        public TextBuilder Append(string? value)
        {
            if (value is null) return this;
            var len = value.Length;
            if (len == 0)
            {

            }
            else if (len == 1)
            {
                var chars = _chars;
                int index = _length;
                if (index < chars.Length)
                {
                    chars[index] = value[0];
                    _length = index + 1;
                }
                else
                {
                    GrowThenCopyString(value);
                }
            }
            else if (value.Length == 2)
            {
                var chars = _chars;
                int index = _length;
                if (index < chars.Length - 1)
                {
                    UnsafeText.WriteTwoChars(value, Available);
                    _length = index + 2;
                }
                else
                {
                    GrowThenCopyString(value);
                }
            }
            else if (value.TryCopyTo(Available))
            {
                _length += value.Length;
            }
            else
            {
                GrowThenCopyString(value);
            }
            return this;
        }

        public TextBuilder Append<T>(T value)
        {
            string? s;
            if (value is IFormattable)
            {
                if (value is ISpanFormattable spanFormattable)
                {
                    int charsWritten;
                    while (!spanFormattable.TryFormat(Available, out charsWritten, default, default))
                    {
                        GrowCore((uint)_length + 1);
                    }
                    _length += charsWritten;
                    return this;
                }

                s = ((IFormattable)value).ToString(null, null);
            }
            else
            {
                s = value?.ToString();
            }

            return Append(s);
        }

        public TextBuilder Append(text value)
        {
            if (value.TryCopyTo(Available))
            {
                _length += value.Length;
            }
            else
            {
                GrowThenCopySpan(value);
            }
            return this;
        }


        public TextBuilder AppendFormat<T>(T value, string? format, IFormatProvider? provider = null)
        {
            string? s;
            if (value is IFormattable)
            {
                if (value is ISpanFormattable spanFormattable)
                {
                    int charsWritten;
                    while (!spanFormattable.TryFormat(Available, out charsWritten, format, provider))
                    {
                        Grow();
                    }
                    _length += charsWritten;
                    return this;
                }

                s = ((IFormattable)value).ToString(format, provider);
            }
            else
            {
                s = value?.ToString();
            }

            return Append(s);
        }

        public TextBuilder Insert(int index, char ch)
        {

        }

      
        public bool Contains(char ch)
        {
            for (var i = 0; i < _length; i++)
            {
                if (_chars[i] == ch) return true;
            }
            return false;
        }

        public void CopyTo(Span<char> destination)
        {
            TextHelper.CopyTo(Text, destination);
        }

        public bool TryCopyTo(Span<char> destination)
        {
            return TextHelper.TryCopyTo(Text, destination);
        }

        public void RemoveAt(int index)
        {
            if ((uint)index < (uint)_length)
            {
                RemoveCore(index, 1);
            }
        }


        public TextBuilder Clear()
        {
            // We do not clean up the array's contents
            _length = 0;
            return this;
        }

        public IEnumerator<char> GetEnumerator()
        {
            //TODO: Custom Enumerator
            for (var i = 0; i < _length; i++)
            {
                yield return _chars[i];
            }
        }
        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

        public void Dispose()
        {
            ArrayPool<char>.Shared.Return(_chars);
        }

        public override bool Equals(object? obj)
        {
            if (obj is string str) return TextHelper.Equals(str, Text);
            if (obj is TextBuilder tb) return TextHelper.Equals(tb.Text, Text);
            return false;
        }

        public override int GetHashCode()
        {
            return UnsuitableException.ThrowGetHashCode(this);
        }

        public override string ToString()
        {
            return new string(_chars, 0, _length);
        }
    }
}
