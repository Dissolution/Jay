using System;
using System.Buffers;
using System.Collections;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using Jay.Reflection;
using Jay.Exceptions;


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
    /// Gets the <see cref="Span{T}"/> of <see cref="char"/>acters that have been written.
    /// </summary>
    public Span<char> Written
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        get => _charArray.AsSpan(0, _length);
    }

    /// <summary>
    /// Gets the <see cref="Span{T}"/> of <see cref="char"/>acters available without growing.
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
            if ((uint)index >= (uint)_length)
                throw new IndexOutOfRangeException($"Index '{index}' is not within [0..{_length - 1}]");
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
        text.CopyTo(Available);
        _length += text.Length;
    }

    [MethodImpl(MethodImplOptions.NoInlining)]
    private void GrowThenCopy(string text)
    {
        Grow(text.Length);
        text.CopyTo(Available);
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
        Written.CopyTo(newArray);
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
        Written[index..].CopyTo(this[(index + length)..]);
        _length += length;
        return _charArray.AsSpan(index, length);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private void RemoveSpan(int index, int length)
    {
        int rIndex = index + length;
        int rLength = _length - rIndex;
        _charArray.AsSpan(rIndex, rLength)
                  .CopyTo(_charArray.AsSpan(index, rLength));
        _length -= length;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void Write(char ch)
    {
        Span<char> chars = _charArray;
        int index = _length;
        if ((uint)index < (uint)chars.Length)
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
        if (text.TryCopyTo(Available))
        {
            _length += text.Length;
        }
        else
        {
            GrowThenCopy(text);
        }
    }

    public void Write<T>(T? value)
    {
        string? s;
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
            s = ((IFormattable)value).ToString(null, null);
        }
        else
        {
            s = value?.ToString();
        }

        if (s is not null)
        {
            if (s.TryCopyTo(Available))
            {
                _length += s.Length;
            }
            else
            {
                GrowThenCopy(s);
            }
        }
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void WriteFormat<T>(T? value, string? format = null, IFormatProvider? provider = null)
    {
        string? s;
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
            s = ((IFormattable)value).ToString(format, provider);
        }
        else
        {
            s = value?.ToString();
        }

        if (s is not null)
        {
            Write(s);
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

    public TextBuilder AppendJoin<T>(params T?[]? values)
    {
        if (values != null)
        {
            for (var i = 0; i < values.Length; i++)
            {
                Write<T>(values[i]);
            }
        }
        return this;
    }

    public TextBuilder AppendJoin<T>(IEnumerable<T?> values)
    {
        foreach (var value in values)
        {
            Write<T>(value);
        }
        return this;
    }

    public TextBuilder AppendDelimit<T>(ReadOnlySpan<char> delimiter, params T?[]? values)
    {
        if (values != null)
        {
            if (values.Length >= 1)
            {
                Write<T>(values[0]);
                for (var i = 1; i < values.Length; i++)
                {
                    Write(delimiter);
                    Write(values[i]);
                }
            }
        }
        return this;
    }

    public TextBuilder AppendDelimit<T>(ReadOnlySpan<char> delimiter, IEnumerable<T?> values)
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

    public TextBuilder Insert(int index, char ch)
    {
        if ((uint)index > (uint)_length)
            throw new IndexOutOfRangeException();
        if (index == _length)
            return Append(ch);
        InsertSpan(index, 1)[0] = ch;
        return this;
    }
    void IList<char>.Insert(int index, char ch) => this.Insert(index, ch);

    public TextBuilder Insert(int index, ReadOnlySpan<char> text)
    {
        if ((uint)index > (uint)_length)
            throw new IndexOutOfRangeException();
        if (index == _length)
            return Append(text);
        text.CopyTo(InsertSpan(index, text.Length));
        return this;
    }

    public TextBuilder Insert<T>(int index, T? value)
    {
        if ((uint)index > (uint)_length)
            throw new IndexOutOfRangeException();
        if (index == _length)
            return Append<T>(value);
        using var temp = new TextBuilder();
        temp.Write<T>(value);
        temp.CopyTo(InsertSpan(index, temp.Length));
        return this;
    }

    public int IndexOf(char ch)
    {
        for (var i = 0; i < _length; i++)
        {
            if (_charArray![i] == ch)
                return i;
        }
        return -1;
    }

    public bool Contains(char ch)
    {
        for (var i = 0; i < _length; i++)
        {
            if (_charArray![i] == ch)
                return true;
        }
        return false;
    }


    public void RemoveAt(int index)
    {
        if ((uint)index >= (uint)_length)
            throw new ArgumentOutOfRangeException(nameof(index), index, $"Index must be between 0 and {_length - 1}");
        RemoveSpan(index, 1);
    }

    public bool Remove(char ch)
    {
        int i = IndexOf(ch);
        if (i >= 0)
        {
            RemoveAt(i);
            return true;
        }
        return false;
    }

    public TextBuilder Clear()
    {
        // We do not clear the contents of the array
        _length = 0;
        return this;
    }
    void ICollection<char>.Clear() => this.Clear();
    

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void CopyTo(char[] array, int arrayIndex = 0)
    {
        Written.CopyTo(array.AsSpan(arrayIndex));
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void CopyTo(Span<char> destination)
    {
        Written.CopyTo(destination);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public bool TryCopyTo(Span<char> destination)
    {
        return Written.TryCopyTo(destination);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public Span<char> AsSpan() => new Span<char>(_charArray);


    public IEnumerator<char> GetEnumerator()
    {
        for (var i = 0; i < _length; i++)
        {
            yield return _charArray![i];
        }
    }
    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

    public void Dispose()
    {
        char[]? toReturn = _charArray;
        _length = 0;
        _charArray = null;
        if (toReturn is not null)
        {
            ArrayPool<char>.Shared.Return(toReturn, true);
        }
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