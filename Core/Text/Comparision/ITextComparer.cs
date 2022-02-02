using System.Collections;

namespace Jay.Text.Comparision;

public interface ITextComparer : IComparer<string?>,
                                 IComparer<char[]?>,
                                 IComparer<char>,
                                 IComparer
{
    int IComparer<string?>.Compare(string? x, string? y)
    {
        return Compare(x.AsSpan(), y.AsSpan());
    }

    int IComparer<char[]?>.Compare(char[]? x, char[]? y)
    {
        return Compare(x.AsSpan(), y.AsSpan());
    }

    int IComparer<char>.Compare(char x, char y)
    {
        return Compare(x.AsReadOnlySpan(), y.AsReadOnlySpan());
    }
    
    int IComparer.Compare(object? x, object? y)
    {
        return Compare(x.ToReadOnlyText(), y.ToReadOnlyText());
    }

    int Compare(ReadOnlySpan<char> x, ReadOnlySpan<char> y);

}