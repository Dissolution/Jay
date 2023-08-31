using System.Collections;

namespace Jay.Text.Comparision;

/// <summary>
/// A <see cref="IComparer{T}"/> that works on all text types
/// </summary>
public interface ITextComparer :
    IComparer<string?>,
    IComparer<char[]?>,
    IComparer<char>,
    IComparer
{
    /// <inheritdoc cref="IComparer{T}"/>
    int Compare(ReadOnlySpan<char> x, ReadOnlySpan<char> y);
}
