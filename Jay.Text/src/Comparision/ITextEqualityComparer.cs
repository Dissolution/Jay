using System.Collections;

namespace Jay.Text.Comparision;

/// <summary>
/// An <see cref="IEqualityComparer{T}"/> that works on all text types
/// </summary>
public interface ITextEqualityComparer : 
    IEqualityComparer<string?>,
    IEqualityComparer<char[]>,
    IEqualityComparer<char>,
    IEqualityComparer
{
    /// <inheritdoc cref="IEqualityComparer{T}"/>
    bool Equals(ReadOnlySpan<char> x, ReadOnlySpan<char> y);
    
    /// <inheritdoc cref="IEqualityComparer{T}"/>
    int GetHashCode(ReadOnlySpan<char> span);
}