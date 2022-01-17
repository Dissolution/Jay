
using System.Runtime.InteropServices;
using Jay.Text.Extensions;

namespace Jay.Text;

public abstract class TextComparer : IEqualityComparer<char>, IComparer<char>,
                                     IEqualityComparer<string>, IComparer<string>, 
                                     IEqualityComparer<char[]>, IComparer<char[]>
{
    public static implicit operator TextComparer(StringComparison stringComparison)
    {
        switch (stringComparison)
        {
            case StringComparison.CurrentCulture:
                return CurrentCulture;
            case StringComparison.CurrentCultureIgnoreCase:
                return CurrentCultureIgnoreCase;
            case StringComparison.InvariantCulture:
                return Invariant;
            case StringComparison.InvariantCultureIgnoreCase:
                return InvariantIgnoreCase;
            case StringComparison.Ordinal:
                return Ordinal;
            case StringComparison.OrdinalIgnoreCase:
                return OrdinalIgnoreCase;
            default:
                return Default;
        }
    }

    public static TextComparer CurrentCulture { get; } = new StringComparisonTextComparer(StringComparison.CurrentCulture);
    public static TextComparer CurrentCultureIgnoreCase { get; } = new StringComparisonTextComparer(StringComparison.CurrentCultureIgnoreCase);
    public static TextComparer Ordinal { get; } = new StringComparisonTextComparer(StringComparison.Ordinal);
    public static TextComparer OrdinalIgnoreCase { get; } = new StringComparisonTextComparer(StringComparison.OrdinalIgnoreCase);
    public static TextComparer Invariant { get; } = new StringComparisonTextComparer(StringComparison.InvariantCulture);
    public static TextComparer InvariantIgnoreCase { get; } = new StringComparisonTextComparer(StringComparison.InvariantCultureIgnoreCase);

    public static TextComparer Default { get; } = new FastTextComparer();

    public virtual int Compare(char x, char y)
    {
        return Compare(x.ToReadOnlySpan(), y.ToReadOnlySpan());
    }

    public virtual int Compare(string? x, string? y)
    {
        return Compare((text)x, (text)y);
    }

    public virtual int Compare(char[]? x, char[]? y)
    {
        return Compare((text)x, (text)y);
    }

    public abstract int Compare(text x, text y);

    public virtual bool Equals(char x, char y)
    {
        return Equals(x.ToReadOnlySpan(), y.ToReadOnlySpan());
    }

    public virtual bool Equals(string? x, string? y)
    {
        return Equals((text)x, (text)y);
    }

    public virtual bool Equals(char[]? x, char[]? y)
    {
        return Equals((text)x, (text)y);
    }

    public abstract bool Equals(text x, text y);

    public int GetHashCode(char ch)
    {
        return GetHashCode(ch.ToReadOnlySpan());
    }

    public virtual int GetHashCode(string? text)
    {
        return GetHashCode((text)text);
    }
        
    public int GetHashCode(char[] charArray)
    {
        return GetHashCode((text)charArray);
    }

    public abstract int GetHashCode(text text);
}

internal sealed class FastTextComparer : TextComparer
{
    public override int Compare(char x, char y)
    {
        return ((ushort)x).CompareTo((ushort)y);
    }

    public override int Compare(string? x, string? y)
    {
        return string.Compare(x, y);
    }

    public override int Compare(char[]? x, char[]? y)
    {
        return base.Compare(x, y);
    }

    public override int Compare(ReadOnlySpan<char> x, ReadOnlySpan<char> y)
    {
        return x.SequenceCompareTo(y);
    }

    public override bool Equals(char x, char y)
    {
        return x == y;
    }

    public override bool Equals(string? x, string? y)
    {
        return TextHelper.Equals(x, y);
    }

    public override bool Equals(char[]? x, char[]? y)
    {
        return TextHelper.Equals(x, y);
    }

    public override bool Equals(ReadOnlySpan<char> x, ReadOnlySpan<char> y)
    {
        return TextHelper.Equals(x, y);
    }

    public override int GetHashCode(string? text)
    {
        return string.GetHashCode(text);
    }

    public override int GetHashCode(ReadOnlySpan<char> text)
    {
        return string.GetHashCode(text);
    }
}

internal sealed class StringComparisonTextComparer : TextComparer
{
    private readonly StringComparison _stringComparison;

    public StringComparisonTextComparer(StringComparison stringComparison)
    {
        _stringComparison = stringComparison;
    }

    public override int Compare(string? x, string? y)
    {
        return string.Compare(x, y, _stringComparison);
    }

    public override int Compare(ReadOnlySpan<char> x, ReadOnlySpan<char> y)
    {
        return x.CompareTo(y, _stringComparison);
    }

    public override bool Equals(string? x, string? y)
    {
        return string.Equals(x, y, _stringComparison);
    }

    public override bool Equals(ReadOnlySpan<char> x, ReadOnlySpan<char> y)
    {
        return x.Equals(y, _stringComparison);
    }

    public override int GetHashCode(string? text)
    {
        return base.GetHashCode();
    }

    public override int GetHashCode(ReadOnlySpan<char> text)
    {
        return string.GetHashCode(text, _stringComparison);
    }
}