using System.Globalization;

namespace Jay.Parsing;

public interface INumberParsable<TSelf> : ISpanParsable<TSelf>, IParsable<TSelf>
    where TSelf : INumberParsable<TSelf>
{
    static abstract bool TryParse([NotNullWhen(true)] string? text, NumberStyles numberStyle, IFormatProvider? formatProvider, [MaybeNullWhen(false)] out TSelf value);
    static abstract bool TryParse(ReadOnlySpan<char> text, NumberStyles numberStyle, IFormatProvider? formatProvider, [MaybeNullWhen(false)] out TSelf value);
}