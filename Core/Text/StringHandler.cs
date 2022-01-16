using System.Buffers;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace Jay.Text;

/*TODO:
[InterpolatedStringHandler]
public ref struct StringHandler
{
    private char[]? _charArray;
    private Span<char> _chars;
    private int _pos;

    public Span<char> Written => _chars.Slice(0, _pos);
    public Span<char> Available => _chars.Slice(_pos);

    public StringHandler()
    {
        _chars = _charArray = ArrayPool<char>.Shared.Rent(1024);
        _pos = 0;
    }

    [MethodImpl(MethodImplOptions.NoInlining)]
    private void GrowThenCopy(char ch)
    {
        Grow(1);
        _chars[_pos] = ch;
        _pos += 1;
    }

    [MethodImpl(MethodImplOptions.NoInlining)]
    private void GrowThenCopy(ReadOnlySpan<char> text)
    {
        Grow(text.Length);
        text.CopyTo(Available);
        _pos += text.Length;
    }

    /// <summary>Grows <see cref="_chars"/> to have the capacity to store at least <paramref name="additionalChars"/> beyond <see cref="_pos"/>.</summary>
    [MethodImpl(MethodImplOptions.NoInlining)] // keep consumers as streamlined as possible
    private void Grow(int additionalChars)
    {
        // This method is called when the remaining space (_chars.Length - _pos) is
        // insufficient to store a specific number of additional characters.  Thus, we
        // need to grow to at least that new total. GrowCore will handle growing by more
        // than that if possible.
        Debug.Assert(additionalChars > _chars.Length - _pos);
        GrowCore((uint)_pos + (uint)additionalChars);
    }

    /// <summary>Grows the size of <see cref="_chars"/>.</summary>
    [MethodImpl(MethodImplOptions.NoInlining)] // keep consumers as streamlined as possible
    private void Grow()
    {
        // This method is called when the remaining space in _chars isn't sufficient to continue
        // the operation.  Thus, we need at least one character beyond _chars.Length.  GrowCore
        // will handle growing by more than that if possible.
        GrowCore((uint)_chars.Length + 1);
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
        int arraySize = (int)Math.Clamp(newCapacity, 1024, int.MaxValue);

        char[] newArray = ArrayPool<char>.Shared.Rent(arraySize);
        _chars.Slice(0, _pos).CopyTo(newArray);

        char[]? toReturn = _charArray;
        _chars = _charArray = newArray;

        if (toReturn is not null)
        {
            ArrayPool<char>.Shared.Return(toReturn);
        }
    }

    private void AppendCore(ReadOnlySpan<char> text)
    {
        if (text.TryCopyTo(Available))
        {
            _pos += text.Length;
        }
        else
        {
            GrowThenCopy(text);
        }
    }

    /// <summary>Ensures <see cref="_chars"/> has the capacity to store <paramref name="additionalChars"/> beyond <see cref="_pos"/>.</summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void EnsureCapacity(int additionalChars)
    {
        if (_chars.Length - _pos < additionalChars)
        {
            Grow(additionalChars);
        }
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void Append(char ch)
    {
        Span<char> chars = _chars;
        int pos = _pos;
        if ((uint)pos < (uint)chars.Length)
        {
            chars[pos] = ch;
            _pos = pos + 1;

        }
        else
        {
            GrowThenCopy(ch);
        }
        return;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void Append(string? str) => Append((ReadOnlySpan<char>)str);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void Append(ReadOnlySpan<char> text)
    {
        if (text.Length == 0)
        {
            return;
        }
        if (text.Length == 1)
        {
            Span<char> chars = _chars;
            int pos = _pos;
            if ((uint)pos < (uint)chars.Length)
            {
                chars[pos] = text[0];
                _pos = pos + 1;

            }
            else
            {
                GrowThenCopy(text);
            }
            return;
        }

        if (text.Length == 2)
        {
            Span<char> chars = _chars;
            int pos = _pos;
            if ((uint)pos < chars.Length - 1)
            {
                Unsafe.WriteUnaligned(ref Unsafe.As<char, byte>(ref Unsafe.Add(ref MemoryMarshal.GetReference(chars), pos)),
                    Unsafe.ReadUnaligned<int>(ref Unsafe
                        .As<char,
                            byte>(ref Unsafe.AsRef<char>(in text
                            .GetPinnableReference()))));
                _pos = pos + 2;
            }
            else
            {
                GrowThenCopy(text);
            }
            return;
        }
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void Append<T>(T? value)
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

                _pos += charsWritten;
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
            AppendCore(s);
        }
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void AppendFormat<T>(T? value, string? format = null, IFormatProvider? provider = null)
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

                _pos += charsWritten;
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
            AppendCore(s);
        }
    }

    public void Clear()
    {
        _pos = 0;
    }

    public void Dispose()
    {
        char[]? toReturn = _charArray;
        this = default; // Defensive clear
        if (toReturn is not null)
        {
            ArrayPool<char>.Shared.Return(toReturn);
        }
    }

    public override bool Equals(object? obj)
    {
        if (obj is string str)
            return MemoryExtensions.SequenceEqual(Written, str);
        if (obj is char[] chars)
            return MemoryExtensions.SequenceEqual(Written, chars);
        return false;
    }

    public override int GetHashCode()
    {
        throw new InvalidOperationException();
    }

    public override string ToString()
    {
        return new string(Written);
    }
}*/

[InterpolatedStringHandler]
public ref struct TextHandler
{

}
