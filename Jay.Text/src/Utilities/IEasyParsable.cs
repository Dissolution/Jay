#if NET7_0_OR_GREATER
namespace Jay.Text.Utilities;

public interface IEasyParsable<TSelf> : ISpanParsable<TSelf>, IParsable<TSelf>
    where TSelf : IEasyParsable<TSelf>
{
    static TSelf ISpanParsable<TSelf>.Parse(ReadOnlySpan<char> text, IFormatProvider? _)
    {
        if (TSelf.TryParse(text, out var value))
            return value;
        throw new ArgumentException($"Cannot parse '{text}' to a {typeof(TSelf)}", nameof(text));
    }
    static TSelf IParsable<TSelf>.Parse([AllowNull, NotNull] string? str, IFormatProvider? _)
    {
        if (TSelf.TryParse(str, out var value))
            return value;
        throw new ArgumentException($"Cannot parse '{str}' to a {typeof(TSelf)}", nameof(str));
    }

    static abstract bool TryParse(ReadOnlySpan<char> text, [MaybeNullWhen(false)] out TSelf result);
    static virtual bool TryParse([AllowNull, NotNullWhen(true)] string? str, [MaybeNullWhen(false)] out TSelf result)
        => TSelf.TryParse((ReadOnlySpan<char>)str, out result);
    
    static bool ISpanParsable<TSelf>.TryParse(ReadOnlySpan<char> text, IFormatProvider? _, [MaybeNullWhen(false)] out TSelf result) 
        => TSelf.TryParse(text, out result);
    
    static bool IParsable<TSelf>.TryParse([AllowNull, NotNullWhen(true)] string? str, IFormatProvider? _, [MaybeNullWhen(false)] out TSelf result)
        => TSelf.TryParse(str, out result);}
#endif