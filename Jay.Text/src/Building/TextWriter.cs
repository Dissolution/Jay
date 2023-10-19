// ReSharper disable MergeCastWithTypeCheck

using System.Diagnostics;

namespace Jay.Text.Building;

public class TextWriter : ITextWriter
{
    protected char[] _chars;
    protected int _length;

    public TextWriter()
    {
        _chars = TextPool.Rent();
        _length = 0;
    }

    public TextWriter(int minCapacity)
    {
        _chars = TextPool.Rent(minCapacity);
        _length = 0;
    }

    public TextWriter(int literalLength, int formattedCount)
    {
        _chars = TextPool.Rent(literalLength, formattedCount);
        _length = 0;
    }

#region Grow
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    protected void GrowBy(int growCount)
    {
        Debug.Assert(growCount > 0);
        char[] newArray = TextPool.RentGrowBy(_chars.Length, growCount);
        TextHelper.CopyTo(_chars, newArray);

        char[] toReturn = _chars;
        _chars = newArray;
        TextPool.Return(toReturn);
    }

    [MethodImpl(MethodImplOptions.NoInlining)]
    protected void GrowThenCopy(char ch)
    {
        int index = _length;
        GrowBy(1);
        TextHelper.Unsafe.CopyBlock(in ch, ref _chars[index], 1);
        _length = index + 1;
    }

    [MethodImpl(MethodImplOptions.NoInlining)]
    protected void GrowThenCopy(scoped ReadOnlySpan<char> text)
    {
        int index = _length;
        int len = text.Length;
        GrowBy(len);
        TextHelper.Unsafe.CopyBlock(text, _chars.AsSpan(index), len);
        _length = index + len;
    }

    [MethodImpl(MethodImplOptions.NoInlining)]
    private void GrowThenCopy(string text)
    {
        int index = _length;
        int len = text.Length;
        GrowBy(len);
        TextHelper.Unsafe.CopyBlock(text, _chars.AsSpan(index), len);
        _length = index + len;
    }
#endregion

    public virtual void Write(char ch)
    {
        int pos = _length;
        Span<char> chars = _chars;
        if (pos < chars.Length)
        {
            chars[pos] = ch;
            _length = pos + 1;
        }
        else
        {
            GrowThenCopy(ch);
        }
    }

    public virtual void Write(scoped ReadOnlySpan<char> text)
    {
        int textLen = text.Length;
        if (textLen > 0)
        {
            int pos = _length;
            Span<char> chars = _chars;
            if (pos + textLen <= chars.Length)
            {
                TextHelper.Unsafe.CopyBlock(text, chars[pos..], textLen);
                _length = pos + textLen;
            }
            else
            {
                GrowThenCopy(text);
            }
        }
    }

    public virtual void Write(params char[]? characters)
    {
        if (characters is not null)
        {
            int pos = _length;
            Span<char> chars = _chars;
            int textLen = characters.Length;
            if (pos + textLen <= chars.Length)
            {
                TextHelper.Unsafe.CopyBlock(characters, ref chars[pos], textLen);
                _length = pos + textLen;
            }
            else
            {
                GrowThenCopy(characters);
            }
        }
    }

    public virtual void Write(string? str)
    {
        if (str is not null)
        {
            int pos = _length;
            Span<char> chars = _chars;
            int textLen = str.Length;
            if (pos + textLen <= chars.Length)
            {
                TextHelper.Unsafe.CopyBlock(str, ref chars[pos], textLen);
                _length = pos + textLen;
            }
            else
            {
                GrowThenCopy(str);
            }
        }
    }

    public virtual void Write([InterpolatedStringHandlerArgument("")] ref InterpolatedTextWriter interpolatedText)
    {
        // by the time we get into the method, the interpolated text writer
        // has already written to us
        // and since we were passed in to its constructor
        // we do not want / need to dispose it
    }

    public virtual void Write<T>(T? value)
    {
        string? str;
        if (value is IFormattable)
        {
#if NET6_0_OR_GREATER
            // If the value can format itself directly into our buffer, do so.
            if (value is ISpanFormattable)
            {
                int charsWritten;
                // constrained call avoiding boxing for value types
                while (!((ISpanFormattable)value).TryFormat(_chars.AsSpan(_length), out charsWritten, default, default))
                {
                    GrowBy(1);
                }
                _length += charsWritten;
                return;
            }
#endif

            // constrained call avoiding boxing for value types
            str = ((IFormattable)value).ToString(default, default);
        }
        else
        {
            str = value?.ToString();
        }

        this.Write(str);
    }

    public virtual void Write<T>(T? value, string? format, IFormatProvider? provider = null)
    {
        string? str;
        if (value is IFormattable)
        {
#if NET6_0_OR_GREATER
            // If the value can format itself directly into our buffer, do so.
            if (value is ISpanFormattable)
            {
                int charsWritten;
                // constrained call avoiding boxing for value types
                while (!((ISpanFormattable)value).TryFormat(_chars.AsSpan(_length), out charsWritten, format, provider))
                {
                    GrowBy(1);
                }
                _length += charsWritten;
                return;
            }
#endif

            // constrained call avoiding boxing for value types
            str = ((IFormattable)value).ToString(format, provider);
        }
        else
        {
            str = value?.ToString();
        }

        Write(str);
    }

    public virtual void Write<T>(T? value, scoped ReadOnlySpan<char> format, IFormatProvider? provider = null)
    {
        string? str;
        if (value is IFormattable)
        {
#if NET6_0_OR_GREATER
            // If the value can format itself directly into our buffer, do so.
            if (value is ISpanFormattable)
            {
                int charsWritten;
                // constrained call avoiding boxing for value types
                while (!((ISpanFormattable)value).TryFormat(_chars.AsSpan(_length), out charsWritten, format, provider))
                {
                    GrowBy(1);
                }
                _length += charsWritten;
                return;
            }
#endif

            // constrained call avoiding boxing for value types
            str = ((IFormattable)value).ToString(format.ToString(), provider);
        }
        else
        {
            str = value?.ToString();
        }

        Write(str);
    }

    public virtual void Dispose()
    {
        char[] toReturn = _chars;
        _chars = null!;
        TextPool.Return(toReturn);
    }

    public string ToStringAndDispose()
    {
        string str = this.ToString();
        this.Dispose();
        return str;
    }

    public override string ToString() => new string(_chars, 0, _length);
    public sealed override bool Equals(object? obj) => throw new NotSupportedException();
    public sealed override int GetHashCode() => throw new NotSupportedException();
}