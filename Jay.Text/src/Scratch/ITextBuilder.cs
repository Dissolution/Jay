using Jay.Text.Building;

namespace Jay.Text.Scratch;

public interface ITextBuilder : IBuildingText
{
    ref char this[int index] { get; }
    Span<char> this[Range range] { get; }
    int Length { get; }
    Span<char> WrittenText { get; }

    ref char Allocate();
    ref char AllocateAt(int index);
    Span<char> Allocate(int count);
    Span<char> AllocateAt(int index, int count);

    void RemoveAt(int index);
    void Remove(int index, int count);
    void Remove(Range range);

    void Replace(char oldChar, char newChar);

    void Replace(
        scoped ReadOnlySpan<char> oldText,
        scoped ReadOnlySpan<char> newText,
        StringComparison comparison = StringComparison.Ordinal);

    void Replace(
        string oldStr,
        string newStr,
        StringComparison comparision = StringComparison.Ordinal);

    void Clear();
}

internal class TextBuilder : BuildingText, ITextBuilder, ITextWriter
{
    public Span<char> WrittenText => _charArray.AsSpan(0, _position);
    public Span<char> AvailableText => _charArray.AsSpan(_position);

    public TextBuilder()
    {
    }

    public TextBuilder(int minCapacity) 
        : base(minCapacity)
    {
    }

    protected void GrowBy(int count)
    {
        int pos = _position;
        var oldArray = _charArray;
        var newArray = TextPool.RentGrowBy(pos, count);
        TextHelper.Unsafe.CopyBlock(oldArray, newArray, pos);
        TextPool.Return(oldArray);
        _charArray = newArray;
    }
    
    public ref char Allocate()
    {
        int pos = _position;
        var chars = _charArray;
        if (pos >= chars.Length)
        {
            GrowBy(1);
        }
        _position = pos + 1;
        return ref chars[pos];
    }

    public ref char AllocateAt(int index)
    {
        int pos = _position;
        Validate.InsertIndex(pos, index);
        var chars = _charArray.AsSpan();
        if (pos >= chars.Length)
        {
            GrowBy(1);
        }
        chars.ExpandAt(index);
        _position = pos + 1;
        return ref chars[index];
    }

    public Span<char> Allocate(int count)
    {
        int pos = _position;
        int newPos = pos + count;
        var chars = _charArray;
        if (newPos >= chars.Length)
        {
            GrowBy(count);
        }
        _position = newPos;
        return chars.AsSpan(pos, count);
    }

    public Span<char> AllocateAt(int index, int count)
    {
        int pos = _position;
        Validate.InsertIndex(pos, index);
        var chars = _charArray.AsSpan();
        var range = new Range(start: index, end: index + count);
        var newPos = pos + count;
        if (newPos >= chars.Length)
        {
            GrowBy(count);
        }
        chars.ExpandAt(range);
        _position = pos + count;
        return chars[range];
    }

    public virtual void Write(char ch)
    {
        Allocate() = ch;
    }

    public virtual void Write(scoped ReadOnlySpan<char> text)
    {
        int len = text.Length;
        TextHelper.Unsafe.CopyBlock(text, Allocate(len), len);
    }

    public virtual void Write(string? str)
    {
        if (str is not null)
        {
            int len = str.Length;
            TextHelper.Unsafe.CopyBlock(str, Allocate(len), len);
        }
    }

    public virtual void Write(params char[]? characters)
    {
        if (characters is not null)
        {
            int len = characters.Length;
            TextHelper.Unsafe.CopyBlock(characters, Allocate(len), len);
        }
    }

    public virtual void Write([InterpolatedStringHandlerArgument("")] ref InterpolatedTextWriter interpolatedText)
    {
        // Will have written to me using me
    }

    public virtual void Write<T>(T? value)
    {
        Write(value?.ToString());
    }

    public virtual void Write<T>(T? value, scoped ReadOnlySpan<char> format, IFormatProvider? provider = null)
    {
        string? str;
        if (value is IFormattable)
        {
            #if NET6_0_OR_GREATER
            if (value is ISpanFormattable)
            {
                int charsWritten;
                while (!((ISpanFormattable)value).TryFormat(AvailableText, out charsWritten, format, provider))
                {
                    GrowBy(1);
                }
                _position += charsWritten;
                return;
            }
#endif
            str = ((IFormattable)value).ToString(format.ToString(), provider);
        }
        else
        {
            str = value?.ToString();
        }
        Write(str);
    }

    public void Write<T>(T? value, string? format, IFormatProvider? provider = null)
    {
        string? str;
        if (value is IFormattable)
        {
#if NET6_0_OR_GREATER
            if (value is ISpanFormattable)
            {
                int charsWritten;
                while (!((ISpanFormattable)value).TryFormat(AvailableText, out charsWritten, format, provider))
                {
                    GrowBy(1);
                }
                _position += charsWritten;
                return;
            }
#endif
            str = ((IFormattable)value).ToString(format, provider);
        }
        else
        {
            str = value?.ToString();
        }
        Write(str);
    }

    public void RemoveAt(int index)
    {
        int pos = _position;
        Validate.Index(pos, index);
        WrittenText.RemoveAt(index);
        _position = pos - 1;
    }

    public void Remove(int index, int count)
    {
        int pos = _position;
        Validate.Range(pos, index, count);
        WrittenText.RemoveRange(index, count);
        _position = pos - count;
    }

    public void Remove(Range range)
    {
        int pos = _position;
        (int offset, int length) = Validate.RangeResolveOffsetLength(pos, range);
        WrittenText.RemoveRange(offset, length);
        _position = pos - length;
    }

    public void Replace(char oldChar, char newChar)
    {
        throw new NotImplementedException();
    }

    public void Replace(scoped ReadOnlySpan<char> oldText, scoped ReadOnlySpan<char> newText, StringComparison comparison = StringComparison.Ordinal)
    {
        throw new NotImplementedException();
    }

    public void Replace(string oldStr, string newStr, StringComparison comparision = StringComparison.Ordinal)
    {
        throw new NotImplementedException();
    }

    public void Clear()
    {
        _position = 0;
    }
}