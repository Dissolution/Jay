#if NET7_0_OR_GREATER
using System.Numerics;
#endif

namespace Jay.Debugging;

/// <summary>
/// Unique Instance Identifier
/// </summary>
public sealed class UIID : IEquatable<UIID>
    , IFormattable
#if NET6_0_OR_GREATER
    , ISpanFormattable
#if NET7_0_OR_GREATER
    , ISpanParsable<UIID>
    , IParsable<UIID>
    , IEqualityOperators<UIID, UIID, bool>
#endif
#endif
{
    public static implicit operator UIID(long id) => new(id);

    public static bool operator ==(UIID? left, UIID? right)
        => Easy.FastEqual(left?._id, right?._id);

    public static bool operator !=(UIID? left, UIID? right)
        => !Easy.FastEqual(left?._id, right?._id);

    public static UIID Zero { get; } = new UIID(0L);

#if NET7_0_OR_GREATER
    static bool IParsable<UIID>.TryParse(string? str, IFormatProvider? _, [NotNullWhen(true)] out UIID? uiid)
    {
        return TryParse(str.AsSpan(), out uiid);
    }

    static bool ISpanParsable<UIID>.TryParse(ReadOnlySpan<char> text, IFormatProvider? _, [NotNullWhen(true)] out UIID? uiid)
    {
        return TryParse(text, out uiid);
    }

    public static bool TryParse(ReadOnlySpan<char> text, [NotNullWhen(true)] out UIID? uiid)
    {
        if (long.TryParse(text, out var id))
        {
            uiid = new(id);
            return true;
        }
        uiid = null;
        return false;
    }

    public static UIID Parse(ReadOnlySpan<char> text, IFormatProvider? _ = default)
    {
        if (TryParse(text, out var uiid))
            return uiid;

        throw new ArgumentException($"Could not parse '{text.ToString()}' to a UIID");
    }
#else
    public static bool TryParse(string? text, [NotNullWhen(true)] out UIID? uiid)
    {
        if (long.TryParse(text, out var id))
        {
            uiid = new(id);
            return true;
        }
        uiid = null;
        return false;
    }
#endif
    public static UIID Parse(string? str, IFormatProvider? _ = default)
    {
        if (TryParse(str, out var uiid))
            return uiid;

        throw new ArgumentException($"Could not parse '{str}' to a UIID");
    }

    private readonly long _id;

    public UIID(long id)
    {
        _id = id;
    }

    public bool Equals(UIID? other)
    {
        return other is not null && other._id == this._id;
    }

    public bool Equals(long id)
    {
        return id == this._id;
    }

    public override bool Equals(object? obj) => obj switch
    {
        UIID uiid => Equals(uiid),
        long id => Equals(id),
        _ => false,
    };

    public override int GetHashCode()
    {
        return _id.GetHashCode();
    }

#if NET6_0_OR_GREATER
    public bool TryFormat(
        Span<char> destination, out int charsWritten,
        ReadOnlySpan<char> format = default,
        IFormatProvider? _ = default)
    {
        return _id.TryFormat(destination, out charsWritten, format);
    }
#endif

    public string ToString(string? format, IFormatProvider? _ = default)
    {
        return _id.ToString(format);
    }

    public override string ToString()
    {
        return _id.ToString("X");
    }
}