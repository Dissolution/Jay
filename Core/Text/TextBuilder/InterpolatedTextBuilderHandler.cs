﻿using System.Runtime.CompilerServices;
using Jay.Exceptions;

namespace Jay.Text;

[InterpolatedStringHandler]
public ref struct InterpolatedTextBuilderHandler
{
    private readonly TextBuilder _textBuilder;
#if DEBUG
    private readonly int _handlerTextStart;
    private int _handlerTextEnd;
#endif

    public InterpolatedTextBuilderHandler(int literalLength, int formattedCount,
                                          TextBuilder textBuilder)
    {
        ArgumentNullException.ThrowIfNull(textBuilder);
        _textBuilder = textBuilder;
#if DEBUG
        _handlerTextEnd = _handlerTextStart = _textBuilder.Length;
#endif
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void AppendLiteral(string? text)
    {
        _textBuilder.Write(text);
#if DEBUG
        _handlerTextEnd = _textBuilder.Length;
#endif
    }

     [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void AppendFormatted<T>(T value, string? format = null)
    {
        _textBuilder.WriteFormatted<T>(value, format);
#if DEBUG
        _handlerTextEnd = _textBuilder.Length;
#endif
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void AppendFormatted<T>(T value, int alignment, string? format = null)
    {
        if (alignment != 0)
        {
            using var temp = new TextBuilder();
            temp.WriteFormatted<T>(value, format);
            _textBuilder.WriteAligned(temp.Written, alignment > 0 ? Alignment.Right : Alignment.Left, alignment);
        }
        else
        {
            _textBuilder.WriteFormatted<T>(value, format);
        }

#if DEBUG
        _handlerTextEnd = _textBuilder.Length;
#endif
    }


    public override bool Equals(object? obj) =>
        UnsuitableException.ThrowEquals(typeof(InterpolatedTextBuilderHandler));

    public override int GetHashCode() =>
        UnsuitableException.ThrowGetHashCode(typeof(InterpolatedTextBuilderHandler));

    public override string ToString()
    {
#if !DEBUG
        return _textBuilder.ToString();
#else
        return _textBuilder.ToString(new Range(_handlerTextStart, _handlerTextEnd));
#endif
    }
}