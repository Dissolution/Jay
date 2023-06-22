using System.Diagnostics;
using Jay.Utilities;

namespace Jay.Text.Linq;

[DebuggerDisplay("{DebugDisplay,nq}")]
public readonly ref struct TextSlice
{
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static implicit operator text(TextSlice textSlice) => textSlice.AsText();

    private readonly text _text;
    private readonly int _sliceStart;
    private readonly int _sliceLength;

    public text SourceText => _text;
    /// <summary>
    /// inclusive
    /// </summary>
    public int SliceStart => _sliceStart;
    /// <summary>
    /// inclusive
    /// </summary>
    public int SliceEnd => (_sliceStart + _sliceLength) - 1;
    public int SliceLength => _sliceLength;
    
    internal TextSlice(text text, int start, int length)
    {
        _text = text;
        _sliceStart = start;
        _sliceLength = length;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public text AsText()
    {
        return _text.Slice(_sliceStart, _sliceLength);
    }

    private string DebugDisplay
    {
        get
        {
            var builder = new CharSpanBuilder();
            // up to 5 characters before
            int start = Math.Max(_sliceStart - 5, 0);
            if (start > 0)
            {
                builder.Write(_text.Slice(new Range(start, _sliceStart)));
            }
            builder.Write('[');
            builder.Write(_text.Slice(_sliceStart, _sliceLength));
            builder.Write(']');
            // up to 5 characters after
            var endRange = new Range(start: SliceEnd + 1, end: SliceEnd + 6);
            builder.Write(_text.SafeSlice(endRange));
        }
    }

    public override string ToString()
    {
        return new string(AsText());
    }
}

public static class TestThoughts
{
    public static void WritePreChars(this ref CharSpanBuilder builder,
        text text,
        int index,
        int count)
    {
        if (index <= 0)
        {
            // nothing before
            return;
        }

        int start = index - count;

        if (start == 0)
        {
            
        }
        else if (start < 0)
        {
            count += start;
            start = 0;
        }
        else
        {
            builder.Write('…');
            start += 1;
            count -= 1;
        }
        
        builder.Write(text.Slice(start, count));
    }
    
    public static void WritePostChars(this ref CharSpanBuilder builder,
        text text,
        int index,
        int count)
    {
        int textLen = text.Length;
        
        if (index >= textLen)
        {
            // nothing after
            return;
        }

        int end = index + count;
        bool trailing = false;

        if (end == textLen)
        {
            
        }
        else if (end > textLen)
        {
            count += (textLen - end);
            end = textLen;
        }
        else
        {
            //builder.Write('…');
            trailing = true;
            end -= 1;
            count -= 1;
        }
        
        builder.Write(text.Slice(start, count));
    }
}