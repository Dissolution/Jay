using System.Collections;

namespace Jay.Text.Comparision;

public interface ITextEqualityComparer : IEqualityComparer<string?>,
                                         IEqualityComparer<char[]>,
                                         IEqualityComparer<char>,
                                         IEqualityComparer
{
    bool IEqualityComparer<string?>.Equals(string? x, string? y) => Equals(x.AsSpan(), y.AsSpan());

    bool IEqualityComparer<char[]>.Equals(char[]? x, char[]? y) => Equals(x.AsSpan(), y.AsSpan());

    bool IEqualityComparer<char>.Equals(char x, char y) => Equals(x.AsReadOnlySpan(), y.AsReadOnlySpan());

    bool IEqualityComparer.Equals(object? x, object? y) => Equals(x.ToReadOnlyText(), y.ToReadOnlyText());

    bool Equals(ReadOnlySpan<char> x, ReadOnlySpan<char> y);


    int IEqualityComparer<string?>.GetHashCode(string? str) => GetHashCode(str.AsSpan());

    int IEqualityComparer<char[]>.GetHashCode(char[]? charArray) => GetHashCode(charArray.AsSpan());

    int IEqualityComparer<char>.GetHashCode(char ch) => GetHashCode(ch.AsReadOnlySpan());

    int IEqualityComparer.GetHashCode(object? obj) => GetHashCode(obj.ToReadOnlyText());

    int GetHashCode(ReadOnlySpan<char> span);
}