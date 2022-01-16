using System.Buffers;
using System.Collections;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using Jay.Exceptions;

namespace Jay.Text;

public class TextBuilder : IList<char>, IReadOnlyList<char>,
                           ICollection<char>, IReadOnlyCollection<char>,
                           IEnumerable<char>,
                           IDisposable
{
    internal const int MinLength = 1024;

    public static string Build(Action<TextBuilder> buildText)
    {
        if (buildText is null) return string.Empty;
        using (var builder = new TextBuilder())
        {
            buildText(builder);
            return builder.ToString();
        }
    }


    protected char[]? _charArray;
    protected int _length;

    public Span<char> Written
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        get => _charArray.AsSpan(0, _length);
    }

    public Span<char> Available
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        get => _charArray.AsSpan(_length);
    }
    
    public ref char this[int index]
    {
        get
        {
            if ((uint)index >= (uint)_length)
                throw new IndexOutOfRangeException();
            return ref _charArray![index];
        }
    }
    char IList<char>.this[int index]
    {
        get
        {
            if ((uint)index >= (uint)_length)
                throw new IndexOutOfRangeException();
            return _charArray![index];
        }
        set
        {
            if ((uint)index >= (uint)_length)
                throw new IndexOutOfRangeException();
            _charArray![index] = value;
        }
    }
    char IReadOnlyList<char>.this[int index]
    {
        get
        {
            if ((uint)index >= (uint)_length)
                throw new IndexOutOfRangeException();
            return _charArray![index];
        }
    }

    int ICollection<char>.Count => _length;

    bool ICollection<char>.IsReadOnly => false;

    int IReadOnlyCollection<char>.Count => _length;

    public int Length
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        get => _length;
    }

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

    /// <summary>Grows <see cref="_chars"/> to have the capacity to store at least <paramref name="additionalChars"/> beyond <see cref="_pos"/>.</summary>
    [MethodImpl(MethodImplOptions.NoInlining)] // keep consumers as streamlined as possible
    private void Grow(int additionalChars)
    {
        // This method is called when the remaining space (_chars.Length - _pos) is
        // insufficient to store a specific number of additional characters.  Thus, we
        // need to grow to at least that new total. GrowCore will handle growing by more
        // than that if possible.
        Debug.Assert(additionalChars > _charArray!.Length - _length);
        GrowCore((uint)_length + (uint)additionalChars);
    }

    /// <summary>Grows the size of <see cref="_chars"/>.</summary>
    [MethodImpl(MethodImplOptions.NoInlining)] // keep consumers as streamlined as possible
    private void Grow()
    {
        // This method is called when the remaining space in _chars isn't sufficient to continue
        // the operation.  Thus, we need at least one character beyond _chars.Length.  GrowCore
        // will handle growing by more than that if possible.
        GrowCore((uint)_charArray!.Length + 1);
    }

    /// <summary>Grow the size of <see cref="_chars"/> to at least the specified <paramref name="requiredMinCapacity"/>.</summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)] // but reuse this grow logic directly in both of the above grow routines
    private void GrowCore(uint requiredMinCapacity)
    {
        // We want the max of how much space we actually required and doubling our capacity (without going beyond the max allowed length). We
        // also want to avoid asking for small arrays, to reduce the number of times we need to grow, and since we're working with unsigned
        // ints that could technically overflow if someone tried to, for example, append a huge string to a huge string, we also clamp to int.MaxValue.
        // Even if the array creation fails in such a case, we may later fail in ToStringAndClear.

        uint newCapacity = Math.Max(requiredMinCapacity, Math.Min((uint)_charArray!.Length * 2, 0x3FFFFFDF));
        int arraySize = (int)Math.Clamp(newCapacity, 1024, int.MaxValue);

        char[] newArray = ArrayPool<char>.Shared.Rent(arraySize);
        Written.CopyTo(newArray);

        char[]? toReturn = _charArray;
        _charArray = newArray;

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
        int right = len - index;
        _charArray.AsSpan(index, right)
                  .CopyTo(_charArray.AsSpan(index + length, right));
        _length = len + length;
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
                    Grow();
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
                    Grow();
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