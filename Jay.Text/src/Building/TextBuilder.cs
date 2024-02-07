using System.Diagnostics;

namespace Jay.Text.Building;

public sealed class TextBuilder : FluentTextBuilder<TextBuilder>
{
    public static TextBuilder New => new();

    public ref char this[int index] => ref _textBuffer[index];
    public Span<char> this[Range range] => _textBuffer[range];
    
    public TextBuilder()
    {
    }

    public TextBuilder(int minCapacity) : base(minCapacity)
    {
    }

    public TextBuilder(int literalLength, int formattedCount) : base(literalLength, formattedCount)
    {
    }
}

public abstract class FluentTextBuilder<TBuilder> : IBuildingText
    where TBuilder : FluentTextBuilder<TBuilder>
{
    protected readonly TextBuffer _textBuffer;
    protected readonly TBuilder _self;

    public int Length => _textBuffer.Count;
    
    protected FluentTextBuilder()
    {
        _textBuffer = new();
        _self = (TBuilder)this;
    }

    protected FluentTextBuilder(int minCapacity)
    {
        _textBuffer = new(minCapacity);
        _self = (TBuilder)this;
    }

    protected FluentTextBuilder(int literalLength, int formattedCount)
    {
        _textBuffer = new(literalLength, formattedCount);
        _self = (TBuilder)this;
    }

    public virtual TBuilder NewLine() => Append(NewLineAndIndentManager.DefaultNewLine);

    public TBuilder NewLines(int count)
    {
        for (var i = 0; i < count; i++)
        {
            this.NewLine();
        }
        return _self;
    }

#region Append
    public virtual TBuilder Append(char ch)
    {
        _textBuffer.Add(ch);
        return _self;
    }

    public TBuilder AppendLine(char ch) => this.Append(ch).NewLine();

    public virtual TBuilder Append(scoped ReadOnlySpan<char> text)
    {
        _textBuffer.AddMany(text);
        return _self;
    }

    public TBuilder AppendLine(scoped ReadOnlySpan<char> text) => this.Append(text).NewLine();

    public virtual TBuilder Append(params char[]? characters)
    {
        _textBuffer.AddMany(characters);
        return _self;
    }

    public TBuilder AppendLine(params char[]? characters) => this.Append(characters).NewLine();

    public virtual TBuilder Append(string? str)
    {
        _textBuffer.Add(str);
        return _self;
    }

    public TBuilder AppendLine(string? str) => this.Append(str).NewLine();

    public TBuilder Append([InterpolatedStringHandlerArgument("")] ref InterpolatedFluentTextBuilder<TBuilder> interpolatedText)
    {
        // It has already written to us!
        return _self;
    }

    public TBuilder AppendLine([InterpolatedStringHandlerArgument("")] ref InterpolatedFluentTextBuilder<TBuilder> interpolatedText)
        => this.Append(ref interpolatedText).NewLine();

    public virtual TBuilder Append<T>(T? value)
    {
        string? str;
        if (value is IFormattable)
        {
#if NET6_0_OR_GREATER
            if (value is ISpanFormattable)
            {
                int charsWritten;
                if (!((ISpanFormattable)value).TryFormat(
                    _textBuffer.UnwrittenSpan(),
                    out charsWritten,
                    default, default))
                {
                    _textBuffer.IncreaseCapacity();
                }
                _textBuffer.Count += charsWritten;
                return _self;
            }
#endif
            str = ((IFormattable)value).ToString(default, default);
        }
        else
        {
            str = value?.ToString();
        }
        _textBuffer.Add(str);
        return _self;
    }

    public TBuilder AppendLine<T>(T? value) => this.Append(value).NewLine();

    public virtual TBuilder Append<T>(T? value, string? format, IFormatProvider? provider = null)
    {
        string? str;
        if (value is IFormattable)
        {
#if NET6_0_OR_GREATER
            if (value is ISpanFormattable)
            {
                int charsWritten;
                if (!((ISpanFormattable)value).TryFormat(
                    _textBuffer.UnwrittenSpan(),
                    out charsWritten,
                    format, provider))
                {
                    _textBuffer.IncreaseCapacity();
                }
                _textBuffer.Count += charsWritten;
                return _self;
            }
#endif
            str = ((IFormattable)value).ToString(format, provider);
        }
        else
        {
            str = value?.ToString();
        }
        _textBuffer.Add(str);
        return _self;
    }

    public TBuilder AppendLine<T>(T? value, string? format, IFormatProvider? provider = null)
        => this.Append(value, format, provider).NewLine();

    public virtual TBuilder Append<T>(T? value, scoped ReadOnlySpan<char> format, IFormatProvider? provider = null)
    {
        string? str;
        if (value is IFormattable)
        {
#if NET6_0_OR_GREATER
            if (value is ISpanFormattable)
            {
                int charsWritten;
                if (!((ISpanFormattable)value).TryFormat(
                    _textBuffer.UnwrittenSpan(),
                    out charsWritten,
                    format, provider))
                {
                    _textBuffer.IncreaseCapacity();
                }
                _textBuffer.Count += charsWritten;
                return _self;
            }
#endif
            str = ((IFormattable)value).ToString(format.ToString(), provider);
        }
        else
        {
            str = value?.ToString();
        }
        _textBuffer.Add(str);
        return _self;
    }

    public TBuilder AppendLine<T>(T? value, scoped ReadOnlySpan<char> format, IFormatProvider? provider = null)
        => this.Append(value, format, provider).NewLine();
#endregion


    public TBuilder Case<T>(T? value, Casing casing)
        => this.Append(value?.ToString().ToCase(casing));


#region Align
    public TBuilder Align(char ch, int width, Alignment alignment)
    {
        if (width < 1)
            throw new ArgumentOutOfRangeException(nameof(width), width, "Width must be 1 or greater");

        var appendSpan = _textBuffer.Allocate(width);
        if (alignment == Alignment.Left)
        {
            appendSpan[0] = ch;
            appendSpan[1..]
                .Fill(' ');
        }
        else if (alignment == Alignment.Right)
        {
            appendSpan[..^1]
                .Fill(' ');
            appendSpan[^1] = ch;
        }
        else // Center
        {
            int padding;
            // Odd width?
            if (width % 2 == 1)
            {
                padding = width / 2;
            }
            else // even
            {
                if (alignment.HasFlag(Alignment.Right)) // Right|Center?
                {
                    padding = width / 2;
                }
                else // Left|Center / Default|Center
                {
                    padding = width / 2 - 1;
                }
            }
            appendSpan[..padding]
                .Fill(' ');
            appendSpan[padding] = ch;
            appendSpan[(padding + 1)..]
                .Fill(' ');
        }
        return _self;
    }

    public TBuilder Align(string? str, int width, Alignment alignment) => Align(str.AsSpan(), width, alignment);

    public TBuilder Align(scoped ReadOnlySpan<char> text, int width, Alignment alignment)
    {
        int textLen = text.Length;
        if (textLen == 0)
        {
            _textBuffer.Allocate(width)
                .Fill(' ');
            return _self;
        }
        int spaces = width - textLen;
        if (spaces < 0)
            throw new ArgumentOutOfRangeException(nameof(width), width, $"Width must be {textLen} or greater");

        if (spaces == 0)
        {
            _textBuffer.AddMany(text);
            return _self;
        }
        var appendSpan = _textBuffer.Allocate(width);
        if (alignment == Alignment.Left)
        {
            TextHelper.Unsafe.CopyBlock(text, appendSpan, textLen);
            appendSpan[textLen..]
                .Fill(' ');
        }
        else if (alignment == Alignment.Right)
        {
            appendSpan[..spaces]
                .Fill(' ');
            TextHelper.Unsafe.CopyBlock(text, appendSpan[spaces..], textLen);
        }
        else // Center
        {
            int frontPadding;
            // Even spacing is easy split
            if (spaces % 2 == 0)
            {
                frontPadding = spaces / 2;
            }
            else // Odd spacing we have to align
            {
                if (alignment.HasFlag(Alignment.Right)) // Right|Center
                {
                    frontPadding = (int)Math.Ceiling(spaces / 2d);
                }
                else // Center or Left|Center 
                {
                    frontPadding = (int)Math.Floor(spaces / 2d);
                }
            }
            appendSpan[..frontPadding]
                .Fill(' ');
            TextHelper.Unsafe.CopyBlock(text, appendSpan[frontPadding..], textLen);
            appendSpan[(frontPadding + textLen)..]
                .Fill(' ');
        }
        return _self;
    }
#endregion

#region Format
    protected void WriteFormatLine(ReadOnlySpan<char> format, object?[] args)
    {
        // Undocumented exclusive limits on the range for Argument Hole Index
        const int INDEX_LIMIT = 1_000_000; // Note:            0 <= ArgIndex < IndexLimit

        // Repeatedly find the next hole and process it.
        int pos = 0;
        char ch;
        while (true)
        {
            // Skip until either the end of the input or the first unescaped opening brace, whichever comes first.
            // Along the way we need to also unescape escaped closing braces.
            while (true)
            {
                // Find the next brace.  If there isn't one, the remainder of the input is text to be appended, and we're done.
                if (pos >= format.Length)
                {
                    return;
                }

                ReadOnlySpan<char> remainder = format.Slice(pos);
                int countUntilNextBrace = remainder.IndexOfAny('{', '}');
                if (countUntilNextBrace < 0)
                {
                    _textBuffer.AddMany(remainder);
                    return;
                }

                // Append the text until the brace.
                _textBuffer.AddMany(remainder.Slice(0, countUntilNextBrace));
                pos += countUntilNextBrace;

                // Get the brace.
                // It must be followed by another character, either a copy of itself in the case of being escaped,
                // or an arbitrary character that's part of the hole in the case of an opening brace.
                char brace = format[pos];
                ch = moveNext(format, ref pos);
                if (brace == ch)
                {
                    _textBuffer.Add(ch);
                    pos++;
                    continue;
                }

                // This wasn't an escape, so it must be an opening brace.
                if (brace != '{')
                {
                    throw createFormatException(format, pos, "Missing opening brace");
                }

                // Proceed to parse the hole.
                break;
            }

            // We're now positioned just after the opening brace of an argument hole, which consists of
            // an opening brace, an index, and an optional format
            // preceded by a colon, with arbitrary amounts of spaces throughout.
            ReadOnlySpan<char> itemFormatSpan = default; // used if itemFormat is null

            // First up is the index parameter, which is of the form:
            //     at least on digit
            //     optional any number of spaces
            // We've already read the first digit into ch.
            Debug.Assert(format[pos - 1] == '{');
            Debug.Assert(ch != '{');
            int index = ch - '0';
            // Has to be between 0 and 9
            if ((uint)index >= 10u)
            {
                throw createFormatException(format, pos, "Invalid character in index");
            }

            // Common case is a single digit index followed by a closing brace.  If it's not a closing brace,
            // proceed to finish parsing the full hole format.
            ch = moveNext(format, ref pos);
            if (ch != '}')
            {
                // Continue consuming optional additional digits.
                while (ch.IsAsciiDigit() && index < INDEX_LIMIT)
                {
                    // Shift by power of 10
                    index = index * 10 + (ch - '0');
                    ch = moveNext(format, ref pos);
                }

                // Consume optional whitespace.
                while (ch == ' ')
                {
                    ch = moveNext(format, ref pos);
                }

                // We do not support alignment
                if (ch == ',')
                {
                    throw createFormatException(format, pos, "Alignment is not supported");
                }

                // The next character needs to either be a closing brace for the end of the hole,
                // or a colon indicating the start of the format.
                if (ch != '}')
                {
                    if (ch != ':')
                    {
                        // Unexpected character
                        throw createFormatException(format, pos, "Unexpected character");
                    }

                    // Search for the closing brace; everything in between is the format,
                    // but opening braces aren't allowed.
                    int startingPos = pos;
                    while (true)
                    {
                        ch = moveNext(format, ref pos);

                        if (ch == '}')
                        {
                            // Argument hole closed
                            break;
                        }

                        if (ch == '{')
                        {
                            // Braces inside the argument hole are not supported
                            throw createFormatException(format, pos, "Braces inside the argument hole are not supported");
                        }
                    }

                    startingPos++;
                    itemFormatSpan = format.Slice(startingPos, pos - startingPos);
                }
            }

            // Construct the output for this arg hole.
            Debug.Assert(format[pos] == '}');
            pos++;

            if ((uint)index >= (uint)args.Length)
            {
                throw createFormatException(format, pos, $"Invalid Format: Argument '{index}' does not exist");
            }

            string? itemFormat = null;
            if (itemFormatSpan.Length > 0)
                itemFormat = itemFormatSpan.ToString();

            object? arg = args[index];

            // Append this arg, allows for overridden behavior
            this.Append<object?>(arg, itemFormat);

            // Continue parsing the rest of the format string.
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        static char moveNext(ReadOnlySpan<char> format, ref int pos)
        {
            pos++;
            if (pos < format.Length)
                return format[pos];

            throw createFormatException(format, pos, "Attempted to move past final character");
        }

        [MethodImpl(MethodImplOptions.NoInlining)]
        static FormatException createFormatException(ReadOnlySpan<char> format, int pos, string? details = null)
        {
            using var message = new InterpolatedText();
            message.Write("Invalid Format at position ");
            message.Write(pos);
            message.Write(Environment.NewLine);
            int start = pos - 16;
            if (start < 0)
                start = 0;
            int end = pos + 16;
            if (end > format.Length)
                end = format.Length - 1;
            message.Write(format[new Range(start, end)]);
            if (details is not null)
            {
                message.Write(Environment.NewLine);
                message.Write("Details: ");
                message.Write(details);
            }
            return new FormatException(message.ToString());
        }
    }

    public TBuilder Format(NonFormattableString format, params object?[] args)
    {
        WriteFormatLine(format.Text, args);
        return _self;
    }

    public TBuilder Format(FormattableString formattableString)
    {
        WriteFormatLine(formattableString.Format.AsSpan(), formattableString.GetArguments());
        return _self;
    }

    public TBuilder Format([InterpolatedStringHandlerArgument("")] ref InterpolatedTextBuilder interpolatedText)
    {
        // Exactly like Write, the work is already done
        return _self;
    }
#endregion


#region Enumerate
    // public TBuilder Enumerate(
    //     TextSplitEnumerable splitEnumerable,
    //     TextBuilderTextAction<TBuilder> perSplitSection)
    // {
    //     foreach (var splitSection in splitEnumerable)
    //     {
    //         perSplitSection(_builder, splitSection);
    //     }
    //     return _builder;
    // }

    public TBuilder Enumerate<T>(ReadOnlySpan<T> values, Action<TBuilder, T> perValue)
    {
        for (var i = 0; i < values.Length; i++)
        {
            perValue(_self, values[i]);
        }
        return _self;
    }

    public TBuilder Enumerate<T>(IEnumerable<T> values, Action<TBuilder, T> perValue)
    {
        foreach (var value in values)
        {
            perValue(_self, value);
        }
        return _self;
    }

    public TBuilder Iterate<T>(ReadOnlySpan<T> values, Action<TBuilder, T, int> perValueIndex)
    {
        for (var i = 0; i < values.Length; i++)
        {
            perValueIndex(_self, values[i], i);
        }
        return _self;
    }

    public TBuilder Iterate<T>(IEnumerable<T> values, Action<TBuilder, T, int> perValueIndex)
    {
        if (values is IList<T> list)
        {
            for (var i = 0; i < list.Count; i++)
            {
                perValueIndex(_self, list[i], i);
            }
        }
        else
        {
            using var e = values.GetEnumerator();
            if (!e.MoveNext())
                return _self;

            int i = 0;
            perValueIndex(_self, e.Current, i);
            while (e.MoveNext())
            {
                i++;
                perValueIndex(_self, e.Current, i);
            }
        }
        return _self;
    }
#endregion

#region Delimit
    // public TBuilder Delimit(
    //     ReadOnlySpan<char> delimiter,
    //     TextSplitEnumerable splitEnumerable,
    //     TextBuilderTextAction<TBuilder> perSplitSection)
    // {
    //     var splitEnumerator = splitEnumerable.GetEnumerator();
    //     if (!splitEnumerator.MoveNext()) return _builder;
    //     perSplitSection(_builder, splitEnumerator.Current);
    //     while (splitEnumerator.MoveNext())
    //     {
    //         Append(delimiter);
    //         perSplitSection(_builder, splitEnumerator.Current);
    //     }
    //     return _builder;
    // }

    // public TBuilder Delimit(
    //     TextBuilderAction<TBuilder> delimit,
    //     TextSplitEnumerable splitEnumerable,
    //     TextBuilderTextAction<TBuilder> perSplitSection)
    // {
    //     var splitEnumerator = splitEnumerable.GetEnumerator();
    //     if (!splitEnumerator.MoveNext()) return _builder;
    //     perSplitSection(_builder, splitEnumerator.Current);
    //     while (splitEnumerator.MoveNext())
    //     {
    //         delimit(_builder);
    //         perSplitSection(_builder, splitEnumerator.Current);
    //     }
    //     return _builder;
    // }

    public TBuilder Delimit<T>(Action<TBuilder> delimit, ReadOnlySpan<T> values, Action<TBuilder, T> perValue)
    {
        var count = values.Length;
        if (count == 0)
            return _self;

        perValue(_self, values[0]);
        for (var i = 1; i < count; i++)
        {
            delimit(_self);
            perValue(_self, values[i]);
        }
        return _self;
    }

    public TBuilder Delimit<T>(Action<TBuilder> delimit, IEnumerable<T> values, Action<TBuilder, T> perValue)
    {
        if (values is IList<T> list)
        {
            var count = list.Count;
            if (count == 0)
                return _self;

            perValue(_self, list[0]);
            for (var i = 1; i < count; i++)
            {
                delimit(_self);
                perValue(_self, list[i]);
            }
        }
        else
        {
            using var e = values.GetEnumerator();
            if (!e.MoveNext())
                return _self;

            perValue(_self, e.Current);
            while (e.MoveNext())
            {
                delimit(_self);
                perValue(_self, e.Current);
            }
        }

        return _self;
    }
#endregion


    public TBuilder If(
        bool predicateResult,
        Action<TBuilder>? ifTrue,
        Action<TBuilder>? ifFalse = null)
    {
        if (predicateResult)
        {
            ifTrue?.Invoke(_self);
        }
        else
        {
            ifFalse?.Invoke(_self);
        }
        return _self;
    }

    public TBuilder IfAppend<T>(T? value, Action<TBuilder>? ifAppended, Action<TBuilder>? ifNotAppended = null)
    {
        if (this.Wrote(b => b.Append<T>(value)))
            return Invoke(ifAppended);

        return _self;
    }

    public bool Wrote(Action<TBuilder>? build)
    {
        int pos = _textBuffer.Count;
        build?.Invoke(_self);
        return _textBuffer.Count > pos;
    }


    public TBuilder GetWritten(Action<TBuilder> build, out Span<char> written)
    {
        int start = _textBuffer.Count;
        build(_self);
        int end = _textBuffer.Count;
        written = _textBuffer[new Range(start: start, end: end)];
        return _self;
    }

    public TBuilder Invoke(Action<TBuilder>? tba)
    {
        tba?.Invoke(_self);
        return _self;
    }


    public void Clear() => _textBuffer.Clear();

    public Span<char> AsSpan() => _textBuffer.AsSpan();
    

    public void Dispose()
    {
        _textBuffer.Dispose();
    }

    public string ToStringAndDispose()
    {
        return _textBuffer.ToStringAndDispose();
    }

    public override string ToString()
    {
        return _textBuffer.ToString();
    }
}