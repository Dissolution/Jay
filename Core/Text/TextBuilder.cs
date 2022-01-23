using System.Buffers;
using System.Collections;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using Jay.Exceptions;
using Jay.Validation;

namespace Jay.Text;

public class TextBuilder : IList<char>, IReadOnlyList<char>,
                           ICollection<char>, IReadOnlyCollection<char>,
                           IEnumerable<char>,
                           IDisposable
{
    internal const int MinLength = 1024;

    public static string Build(Action<TextBuilder>? buildText)
    {
        if (buildText is null) return string.Empty;
        using (var builder = new TextBuilder())
        {
            buildText(builder);
            return builder.ToString();
        }
    }

    public static string Build<TState>(TState? state, Action<TextBuilder, TState?>? buildText)
    {
        if (buildText is null) return string.Empty;
        using (var builder = new TextBuilder())
        {
            buildText(builder, state);
            return builder.ToString();
        }
    }


    protected char[]? _charArray;
    protected int _length;

    /// <summary>
    /// Gets the <see cref="Span{T}"/> of <see cref="char"/>s that have been written.
    /// </summary>
    public Span<char> Written
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        get => new Span<char>(_charArray, 0, _length);
    }

    /// <summary>
    /// Gets the <see cref="Span{T}"/> of <see cref="char"/>s available without growing.
    /// </summary>
    public Span<char> Available
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        get => _charArray.AsSpan(_length);
    }

    /// <summary>
    /// Gets a <see langword="ref"/> to the <see cref="char"/> at the given <paramref name="index"/>.
    /// </summary>
    /// <param name="index">The index of the <see cref="char"/> to reference.</param>
    /// <returns>A <see langword="ref"/> to the <see cref="char"/> at <paramref name="index"/>.</returns>
    /// <exception cref="IndexOutOfRangeException">
    /// Thrown if <paramref name="index"/> is not within the current bounds.
    /// </exception>
    public ref char this[int index]
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        get
        {
            Validate.Index(index, _length);
            // Length is >0 if _charArray is not null
            return ref _charArray![index];
        }
    }
    /// <inheritdoc cref="IList{T}"/>
    char IList<char>.this[int index]
    {
        get => this[index];
        set => this[index] = value;
    }
    /// <inheritdoc cref="IReadOnlyList{T}"/>
    char IReadOnlyList<char>.this[int index]
    {
        get => this[index];
    }

    public Span<char> this[Range range]
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        get => _charArray.AsSpan(range);
    }

    /// <summary>
    /// Gets the number of characters that have been written.
    /// </summary>
    public int Length
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        get => _length;
    }
    /// <inheritdoc cref="ICollection{T}"/>
    int ICollection<char>.Count => _length;
    /// <inheritdoc cref="IReadOnlyCollection{T}"/>
    int IReadOnlyCollection<char>.Count => _length;

    /// <inheritdoc cref="ICollection{T}"/>
    bool ICollection<char>.IsReadOnly => false;

    /// <summary>
    /// Construct a new <see cref="TextBuilder"/>.
    /// </summary>
    public TextBuilder()
    {
        _charArray = ArrayPool<char>.Shared.Rent(MinLength);
        _length = 0;
    }

    [MethodImpl(MethodImplOptions.NoInlining)]
    private void GrowThenCopy(char ch)
    {
        Grow(1);
        int index = _length;
        _charArray![index] = ch;
        _length = index + 1;
    }

    [MethodImpl(MethodImplOptions.NoInlining)]
    private void GrowThenCopy(ReadOnlySpan<char> text)
    {
        Grow(text.Length);
        TextHelper.CopyTo(text, Available);
        _length += text.Length;
    }

    [MethodImpl(MethodImplOptions.NoInlining)]
    private void GrowThenCopy(string text)
    {
        Grow(text.Length);
        TextHelper.CopyTo(text, Available);
        _length += text.Length;
    }

    /// <summary>Grows <see cref="_chars"/> to have the capacity to store at least <paramref name="additionalChars"/> beyond <see cref="_pos"/>.</summary>
    [MethodImpl(MethodImplOptions.NoInlining)] // keep consumers as streamlined as possible
    protected void Grow(int additionalChars)
    {
        // This method is called when the remaining space (_chars.Length - _pos) is
        // insufficient to store a specific number of additional characters.  Thus, we
        // need to grow to at least that new total. GrowCore will handle growing by more
        // than that if possible.
        Debug.Assert(additionalChars > _charArray!.Length - _length);
        GrowCore(_length + additionalChars);
    }

    /// <summary>Grow the size of <see cref="_chars"/> to at least the specified <paramref name="minCapacity"/>.</summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)] // but reuse this grow logic directly in both of the above grow routines
    private void GrowCore(int minCapacity)
    {
        // We want the max of how much space we actually required and doubling our capacity (without going beyond the max allowed length).
        // We also want to avoid asking for small arrays to reduce the number of times we need to grow.
        
        // string.MaxLength < array.MaxLength
        const int stringMaxLength = 0x3FFFFFDF;
        //const int arrayMaxLength =  0X7FFFFFC7;
        int newCapacity = Math.Clamp(Math.Max(minCapacity, _charArray!.Length * 2), 
                                     MinLength,
                                     stringMaxLength);

        // Get our new array, copy what we have written to it
        char[] newArray = ArrayPool<char>.Shared.Rent(newCapacity);
        TextHelper.CopyTo(Written, newArray);
        // Return the array and then point at the new one
        char[]? toReturn = _charArray;
        _charArray = newArray;
        // We may not have had anything to return (if we started with an initial buffer or some weird dispose)
        if (toReturn is not null)
        {
            ArrayPool<char>.Shared.Return(toReturn);
        }
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private Span<char> InsertSpan(int index, int length)
    {
        int len = _length;
        if (len + length > _charArray!.Length)
        {
            Grow(length);
        }
        TextHelper.CopyTo(Written[index..], this[(index + length)..]);
        _length += length;
        return _charArray.AsSpan(index, length);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private void RemoveSpan(int index, int length)
    {
        TextHelper.CopyTo(Written[(index+length)..], this[index..]);
        _length -= length;
    }

    /// <summary>
    /// Writes a single <see cref="char"/>.
    /// </summary>
    /// <param name="ch">The single <see cref="char"/> to write.</param>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void Write(char ch)
    {
        Span<char> chars = _charArray;
        int index = _length;
        if (index < chars.Length)
        {
            chars[index] = ch;
            _length = index + 1;
        }
        else
        {
            GrowThenCopy(ch);
        }
    }
    void ICollection<char>.Add(char item) => Write(item);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void Write(ReadOnlySpan<char> text)
    {
        if (TextHelper.TryCopyTo(text, Available))
        {
            _length += text.Length;
        }
        else
        {
            GrowThenCopy(text);
        }
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void Write(string? text)
    {
        if (text is null) return;
        if (TextHelper.TryCopyTo(text, Available))
        {
            _length += text.Length;
        }
        else
        {
            GrowThenCopy(text);
        }
    }

    /// <summary>
    /// Writes the text representation of a <paramref name="value"/> to this <see cref="TextBuilder"/>.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="value"></param>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void Write<T>(T? value)
    {
        string? strValue;
        if (value is IFormattable)
        {
            // If the value can format itself directly into our buffer, do so.
            if (value is ISpanFormattable)
            {
                int charsWritten;
                // constrained call avoiding boxing for value types
                while (!((ISpanFormattable)value).TryFormat(Available, out charsWritten, default, default))
                {
                    Grow(1);
                }

                _length += charsWritten;
                return;
            }

            // constrained call avoiding boxing for value types
            strValue = ((IFormattable)value).ToString(null, null);
        }
        else
        {
            strValue = value?.ToString();
        }

        if (strValue is not null)
        {
            if (TextHelper.TryCopyTo(strValue, Available))
            {
                _length += strValue.Length;
            }
            else
            {
                GrowThenCopy(strValue);
            }
        }
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void WriteFormat<T>(T? value, string? format = null, IFormatProvider? provider = null)
    {
        string? strValue;
        if (value is IFormattable)
        {
            // If the value can format itself directly into our buffer, do so.
            if (value is ISpanFormattable)
            {
                int charsWritten;
                // constrained call avoiding boxing for value types
                while (!((ISpanFormattable)value).TryFormat(Available, out charsWritten, format, provider))
                {
                    Grow(1);
                }

                _length += charsWritten;
                return;
            }

            // constrained call avoiding boxing for value types
            strValue = ((IFormattable)value).ToString(format, provider);
        }
        else
        {
            strValue = value?.ToString();
        }

        if (strValue is not null)
        {
            if (TextHelper.TryCopyTo(strValue, Available))
            {
                _length += strValue.Length;
            }
            else
            {
                GrowThenCopy(strValue);
            }
        }
    }


    public TextBuilder Append(char ch)
    {
        Write(ch);
        return this;
    }

    public TextBuilder Append(ReadOnlySpan<char> text)
    {
        Write(text);
        return this;
    }

    public TextBuilder Append(string? text)
    {
        Write(text);
        return this;
    }

    public TextBuilder Append<T>(T? value)
    {
        Write<T>(value);
        return this;
    }

    public TextBuilder AppendFormat<T>(T? value, string? format = null, IFormatProvider? provider = null)
    {
        WriteFormat<T>(value, format, provider);
        return this;
    }

    public TextBuilder AppendJoin<T>(params T[]? values)
    {
        if (values is not null)
        {
            for (var i = 0; i < values.Length; i++)
            {
                Write<T>(values[i]);
            }
        }
        return this;
    }

    public TextBuilder AppendJoin<T>(IEnumerable<T> values)
    {
        foreach (var value in values)
        {
            Write<T>(value);
        }
        return this;
    }

    public TextBuilder AppendDelimit<T>(ReadOnlySpan<char> delimiter, ReadOnlySpan<T> values)
    {
        if (values.Length >= 1)
        {
            Write<T>(values[0]);
            for (var i = 1; i < values.Length; i++)
            {
                Write(delimiter);
                Write<T>(values[i]);
            }
        }
        return this;
    }

    public TextBuilder AppendDelimit<T>(ReadOnlySpan<char> delimiter, params T[]? values)
    {
        if (values != null)
        {
            if (values.Length >= 1)
            {
                Write<T>(values[0]);
                for (var i = 1; i < values.Length; i++)
                {
                    Write(delimiter);
                    Write<T>(values[i]);
                }
            }
        }
        return this;
    }

    public TextBuilder AppendDelimit<T>(ReadOnlySpan<char> delimiter, IEnumerable<T> values)
    {
        using (var e = values.GetEnumerator())
        {
            if (e.MoveNext())
            {
                Write<T>(e.Current);
                while (e.MoveNext())
                {
                    Write(delimiter);
                    Write(e.Current);
                }
            }
        }
        return this;
    }

    public TextBuilder AppendDelimit<T>(ReadOnlySpan<char> delimiter, ReadOnlySpan<T> values, Action<TextBuilder, T> appendValue)
    {
        if (values.Length >= 1)
        {
            appendValue(this, values[0]);
            for (var i = 1; i < values.Length; i++)
            {
                Write(delimiter);
                appendValue(this, values[i]);
            }
        }
        return this;
    }

    public TextBuilder AppendDelimit<T>(ReadOnlySpan<char> delimiter, IEnumerable<T> values, Action<TextBuilder, T> appendValue)
    {
        using (var e = values.GetEnumerator())
        {
            if (e.MoveNext())
            {
                appendValue(this, e.Current);
                while (e.MoveNext())
                {
                    Write(delimiter);
                    appendValue(this, e.Current);
                }
            }
        }
        return this;
    }

    public TextBuilder AppendNewLine() => Append(Environment.NewLine);

    public TextBuilder AppendNewLines(int count)
    {
        ReadOnlySpan<char> nl = Environment.NewLine;
        for (var i = 0; i < count; i++)
        {
            Write(nl);
        }
        return this;
    }

    public TextBuilder Insert(int index, char ch)
    {
        Validate.Insert(index, _length);
        if (index == _length)
            return Append(ch);
        InsertSpan(index, 1)[0] = ch;
        return this;
    }
    void IList<char>.Insert(int index, char ch) => this.Insert(index, ch);

    public TextBuilder Insert(int index, ReadOnlySpan<char> text)
    {
        Validate.Insert(index, _length);
        if (index == _length)
            return Append(text);
        TextHelper.CopyTo(text, InsertSpan(index, text.Length));
        return this;
    }

    public TextBuilder Insert<T>(int index, T? value)
    {
        return Insert(index, tb => tb.Write<T>(value));
    }

    public TextBuilder Insert(int index, Action<TextBuilder> buildInsertText)
    {
        Validate.Insert(index, _length);
        using var temp = new TextBuilder();
        buildInsertText(temp);
        if (index == _length)
        {
            return Append(temp.Written);
        }

        TextHelper.CopyTo(temp.Written, InsertSpan(index, temp.Length));
        return this;
    }

    public int FirstIndexOf(char ch)
    {
        ReadOnlySpan<char> chars = Written;
        for (var i = 0; i < chars.Length; i++)
        {
            if (chars[i] == ch) return i;
        }
        return -1;
    }
    int IList<char>.IndexOf(char ch) => FirstIndexOf(ch);
    public int LastIndexOf(char ch)
    {
        ReadOnlySpan<char> chars = Written;
        for (var i = chars.Length-1; i >= 0; i--)
        {
            if (chars[i] == ch) return i;
        }
        return -1;
    }

    public int FirstIndexOf(ReadOnlySpan<char> text)
    {
        return MemoryExtensions.IndexOf(Written, text);
    }
    public int FirstIndexOf(ReadOnlySpan<char> text, StringComparison comparison)
    {
        return MemoryExtensions.IndexOf(Written, text, comparison);
    }
    public int LastIndexOf(ReadOnlySpan<char> text)
    {
        return MemoryExtensions.LastIndexOf(Written, text);
    }
    public int LastIndexOf(ReadOnlySpan<char> text, StringComparison comparison)
    {
        return MemoryExtensions.LastIndexOf(Written, text, comparison);
    }

    public bool Contains(char ch) => FirstIndexOf(ch) >= 0;

    public void RemoveAt(int index)
    {
        Validate.Index(index, _length);
        RemoveSpan(index, 1);
    }

    public void RemoveAt(Range range)
    {
        var (offset, length) = range.GetOffsetAndLength(_length);
        RemoveSpan(offset, length);
    }

    public int RemoveFirst(char ch)
    {
        ReadOnlySpan<char> chars = Written;
        for (var i = 0; i < chars.Length; i++)
        {
            if (chars[i] == ch)
            {
                RemoveSpan(i, 1);
                return i;
            }
        }
        return -1;
    }
    bool ICollection<char>.Remove(char ch) => RemoveFirst(ch) >= 0;

    public int RemoveLast(char ch)
    {
        ReadOnlySpan<char> chars = Written;
        for (var i = chars.Length - 1; i >= 0; i--)
        {
            if (chars[i] == ch)
            {
                RemoveSpan(i, 1);
                return i;
            }
        }
        return -1;
    }

    public int RemoveFirst(ReadOnlySpan<char> text)
    {
        var i = FirstIndexOf(text);
        if (i >= 0)
        {
            RemoveSpan(i, text.Length);
        }
        return i;
    }
    public int RemoveFirst(ReadOnlySpan<char> text, StringComparison comparison)
    {
        var i = FirstIndexOf(text, comparison);
        if (i >= 0)
        {
            RemoveSpan(i, text.Length);
        }
        return i;
    }

    public int RemoveLast(ReadOnlySpan<char> text)
    {
        var i = LastIndexOf(text);
        if (i >= 0)
        {
            RemoveSpan(i, text.Length);
        }
        return i;
    }
    public int RemoveLast(ReadOnlySpan<char> text, StringComparison comparison)
    {
        var i = LastIndexOf(text, comparison);
        if (i >= 0)
        {
            RemoveSpan(i, text.Length);
        }
        return i;
    }

    public TextBuilder Clear()
    {
        // We do not clear the contents of the array
        _length = 0;
        return this;
    }
    void ICollection<char>.Clear() => this.Clear();
    

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void CopyTo(char[] array, int arrayIndex = 0) => TextHelper.CopyTo(Written, array.AsSpan(arrayIndex));

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void CopyTo(Span<char> destination) => TextHelper.CopyTo(Written, destination);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public bool TryCopyTo(Span<char> destination) => TextHelper.TryCopyTo(Written, destination);

    /// <summary>Enumerates the elements of a <see cref="Span{T}"/>.</summary>
    public ref struct Enumerator
    {
        /// <summary>The span being enumerated.</summary>
        private readonly Span<char> _span;
        /// <summary>The next index to yield.</summary>
        private int _index;

        /// <summary>Gets the element at the current position of the enumerator.</summary>
        public ref char Current
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => ref _span[_index];
        }

        public int Index
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => _index;
        }

        /// <summary>Initialize the enumerator.</summary>
        /// <param name="span">The span to enumerate.</param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal Enumerator(Span<char> span)
        {
            _span = span;
            _index = -1;
        }

        /// <summary>Advances the enumerator to the next element of the span.</summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool MoveNext()
        {
            int index = _index + 1;
            if (index < _span.Length)
            {
                _index = index;
                return true;
            }

            return false;
        }
    }

    public Enumerator GetEnumerator()
    {
        return new Enumerator(Written);
    }

    IEnumerator<char> IEnumerable<char>.GetEnumerator()
    {
        for (var i = 0; i < Written.Length; i++)
        {
            yield return Written[i];
        }
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        for (var i = 0; i < Written.Length; i++)
        {
            yield return Written[i];
        }
    }

    public void Dispose()
    {
        char[]? toReturn = _charArray;
        _length = 0;
        _charArray = null;
        if (toReturn is not null)
        {
            // We do not clear the array
            ArrayPool<char>.Shared.Return(toReturn, false);
        }
    }

    public bool Equals(string? text)
    {
        return TextHelper.Equals(Written, text);
    }
    public bool Equals(ReadOnlySpan<char> text)
    {
        return TextHelper.Equals(Written, text);
    }

    public override bool Equals(object? obj)
    {
        return UnsuitableException.ThrowEquals(this);
    }

    public override int GetHashCode()
    {
        return UnsuitableException.ThrowGetHashCode(this);
    }

    public override string ToString()
    {
        return new string(_charArray!, 0, _length);
    }
}