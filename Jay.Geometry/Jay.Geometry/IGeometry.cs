namespace Jay.Geometry;

public interface IGeometry :
#if NET7_0_OR_GREATER
    ISpanFormattable,
#endif
    IFormattable,
    ICloneable
{
    bool IsEmpty { get; }
}

public interface IGeometry<Self> : IGeometry,
#if NET7_0_OR_GREATER
    IEqualityOperators<Self, Self, bool>,
    IAdditionOperators<Self, Self, Self>,
    ISubtractionOperators<Self, Self, Self>,
    ISpanParsable<Self>, IParsable<Self>,
#endif
    IEquatable<Self>
    where Self : struct, IGeometry<Self>
{
#if NET6_0_OR_GREATER
    static readonly Self Empty = default;

#if NET7_0_OR_GREATER
    static Self IParsable<Self>.Parse([AllowNull, NotNull] string? str, IFormatProvider? provider)
    {
        ArgumentNullException.ThrowIfNull(str);
        if (Self.TryParse(str, provider, out Self self))
            return self;

        throw new ArgumentException($"Could not parse '{str}' as a {typeof(Self)} value", nameof(str));
    }

    static Self ISpanParsable<Self>.Parse(ReadOnlySpan<char> text, IFormatProvider? provider)
    {
        if (Self.TryParse(text, provider, out Self self))
            return self;

        throw new ArgumentException($"Could not parse '{text}' as a {typeof(Self)} value", nameof(text));
    }
#endif

    object ICloneable.Clone() => Clone();
#endif

    new Self Clone();
}