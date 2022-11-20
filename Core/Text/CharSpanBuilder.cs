// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Buffers;
using System.Diagnostics;

using Jay.Exceptions;
using Jay.Validation;

namespace Jay.Text;

/// <summary>
/// A fast and simple way to write text to an expandable pooled <c>ReadOnlySpan&lt;char&gt;</c>
/// </summary>
/// <remarks>
/// A copy of <see cref="DefaultInterpolatedStringHandler"/> without:<br/>
/// - custom <see cref="IFormatProvider"/> support<br/>
/// - alignment (in format strings)
/// </remarks>
public ref struct CharSpanWriter
{
    /// <summary>Minimum size array to rent from the pool.</summary>
    /// <remarks>Same as stack-allocation size used today by string.Format.</remarks>
    private const int MinCapacity = 0x100;

    // string.MaxLength (0x3FFFFFDF) < Array.MaxLength (0x7FFFFFC7)
    private const int MaxCapacity = 0x3FFFFFDF;

    /// <summary>Array rented from the array pool and used to back <see cref="_chars"/>.</summary>
    private char[]? _arrayToReturnToPool;

    /// <summary>The span to write into.</summary>
    private Span<char> _chars;

    /// <summary>Position at which to write the next character.</summary>
    private int _pos;

    public ref char this[int index]
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        get
        {
            Validate.Index(_pos, index);
            return ref _chars[index];
        }
    }

    public int Index
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        get => _pos;
    }

    public int Capacity
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        get => _chars.Length;
    }

    public Span<char> Written
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        get => _chars[.._pos];
    }

    public Span<char> Available
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        get => _chars[_pos..];
    }

    public CharSpanWriter()
    {
        _chars = _arrayToReturnToPool = ArrayPool<char>.Shared.Rent(MinCapacity);
        _pos = 0;
    }

    public CharSpanWriter(Span<char> initialBuffer)
    {
        _chars = initialBuffer;
        _arrayToReturnToPool = null;
        _pos = 0;
    }

    #region Grow

    /// <summary>Grow the size of <see cref="_chars"/> to at least the specified <paramref name="requiredMinCapacity"/>.</summary>
    [MethodImpl(MethodImplOptions
        .AggressiveInlining)] // but reuse this grow logic directly in both of the above grow routines
    private void GrowCore(int newCapacity)
    {
        Debug.Assert(newCapacity > MinCapacity);
        Debug.Assert(newCapacity < MaxCapacity);
        Debug.Assert(newCapacity > Capacity);

        char[] newArray = ArrayPool<char>.Shared.Rent(newCapacity);
        Written.CopyTo(newArray);

        char[]? toReturn = _arrayToReturnToPool;
        _chars = _arrayToReturnToPool = newArray;

        if (toReturn is not null)
        {
            ArrayPool<char>.Shared.Return(toReturn);
        }
    }

    /// <summary>Grows <see cref="_chars"/> to have the capacity to store at least <paramref name="additionalChars"/> beyond <see cref="_pos"/>.</summary>
    [MethodImpl(MethodImplOptions.NoInlining)] // keep consumers as streamlined as possible
    private void Grow(int additionalChars)
    {
        int newCapacity = Math.Min((Capacity + additionalChars) * 2, MaxCapacity);
        GrowCore(newCapacity);
    }

    /// <summary>Fallback for fast path in <see cref="WriteCore"/> when there's not enough space in the destination.</summary>
    /// <param name="value">The string to write.</param>
    [MethodImpl(MethodImplOptions.NoInlining)]
    private void GrowThenCopyString(string value)
    {
        Grow(value.Length);
        value.CopyTo(Available);
        _pos += value.Length;
    }

    /// <summary>Fallback for <see cref="AppendFormatted(ReadOnlySpan{char})"/> for when not enough space exists in the current buffer.</summary>
    /// <param name="value">The span to write.</param>
    [MethodImpl(MethodImplOptions.NoInlining)]
    private void GrowThenCopySpan(scoped ReadOnlySpan<char> value)
    {
        Grow(value.Length);
        value.CopyTo(Available);
        _pos += value.Length;
    }

    #endregion

    #region Write

    /// <summary>Writes the specified string to the handler.</summary>
    /// <param name="value">The string to write.</param>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private void WriteCore(string value)
    {
        if (value.TryCopyTo(Available))
        {
            _pos += value.Length;
        }
        else
        {
            GrowThenCopyString(value);
        }
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void Write(char ch)
    {
        int pos = _pos;
        Span<char> chars = _chars;
        if ((uint)pos < chars.Length)
        {
            chars[pos] = ch;
            _pos = pos + 1;
        }
        else
        {
            GrowThenCopySpan(new ReadOnlySpan<char>(in ch));
        }
    }

    /// <summary>Writes the specified character span to the handler.</summary>
    /// <param name="value">The span to write.</param>
    public void Write(scoped ReadOnlySpan<char> text)
    {
        // Fast path for when the value fits in the current buffer
        if (text.TryCopyTo(Available))
        {
            _pos += text.Length;
        }
        else
        {
            GrowThenCopySpan(text);
        }
    }

    /// <summary>Writes the specified string to the handler.</summary>
    /// <param name="text">The string to write.</param>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void Write(string? text)
    {
        if (text is null) return;

        if (text.Length == 1)
        {
            Write(text[0]);
        }
        else if (text.Length == 2)
        {
            int pos = _pos;
            Span<char> chars = _chars;
            if ((uint)pos < chars.Length - 1)
            {
                chars[pos] = text[0];
                chars[pos + 1] = text[1];
                _pos = pos + 2;
            }
            else
            {
                GrowThenCopyString(text);
            }
        }
        else
        {
            WriteCore(text);
        }
    }

    /// <summary>Writes the specified value to the handler.</summary>
    /// <param name="value">The value to write.</param>
    /// <typeparam name="T">The type of the value to write.</typeparam>
    public void Write<T>(T? value)
    {
        string? str;
        if (value is IFormattable)
        {
            // If the value can format itself directly into our buffer, do so.
            if (value is ISpanFormattable)
            {
                int charsWritten;
                // constrained call avoiding boxing for value types
                while (!((ISpanFormattable)value).TryFormat(_chars.Slice(_pos), out charsWritten, default, default))
                {
                    Grow(MinCapacity);
                }

                _pos += charsWritten;
                return;
            }

            // constrained call avoiding boxing for value types
            str = ((IFormattable)value).ToString(default, default);
        }
        else
        {
            str = value?.ToString();
        }

        if (str is not null)
        {
            WriteCore(str);
        }
    }

    /// <summary>Writes the specified value to the handler.</summary>
    /// <param name="value">The value to write.</param>
    /// <param name="format">The format string.</param>
    /// <typeparam name="T">The type of the value to write.</typeparam>
    public void Write<T>(T? value, string? format)
    {
        string? str;
        if (value is IFormattable)
        {
            // If the value can format itself directly into our buffer, do so.
            if (value is ISpanFormattable)
            {
                int charsWritten;
                // constrained call avoiding boxing for value types
                while (!((ISpanFormattable)value).TryFormat(_chars.Slice(_pos), out charsWritten, format, default))
                {
                    Grow(MinCapacity);
                }

                _pos += charsWritten;
                return;
            }

            // constrained call avoiding boxing for value types
            str = ((IFormattable)value).ToString(format, default);
        }
        else
        {
            str = value?.ToString();
        }

        if (str is not null)
        {
            WriteCore(str);
        }
    }


    /// <summary>Writes the specified value to the handler.</summary>
    /// <param name="value">The value to write.</param>
    /// <param name="format">The format string.</param>
    public void Write(object? value, string? format = null) => Write<object?>(value, format);

    #endregion

    public void WriteLine() => Write(Environment.NewLine);


    /// <summary>Clears the handler, returning any rented array to the pool.</summary>
    public void Dispose()
    {
        char[]? toReturn = _arrayToReturnToPool;
        this = default; // defensive clear
        if (toReturn is not null)
        {
            ArrayPool<char>.Shared.Return(toReturn);
        }
    }

    public override bool Equals(object? obj) => UnsupportedException.ThrowForEquals(typeof(CharSpanWriter));
    public override int GetHashCode() => UnsupportedException.ThrowForGetHashCode(typeof(CharSpanWriter));

    /// <summary>Gets the built <see cref="string"/> and clears the handler.</summary>
    /// <returns>The built string.</returns>
    /// <remarks>
    /// This releases any resources used by the handler. The method should be invoked only
    /// once and as the last thing performed on the handler. Subsequent use is erroneous, ill-defined,
    /// and may destabilize the process, as may using any other copies of the handler after ToStringAndClear
    /// is called on any one of them.
    /// </remarks>
    public string ToStringAndDispose()
    {
        string result = new string(Written);
        Dispose();
        return result;
    }

    /// <summary>Gets the built <see cref="string"/>.</summary>
    /// <returns>The built string.</returns>
    public override string ToString() => new string(Written);
}