using System;
using System.Collections;
using System.Collections.Generic;

namespace Jay.Comparison
{
    public interface ITextEqualityComparer : IEqualityComparer<string?>, 
                                             IEqualityComparer<char[]?>,
                                             IEqualityComparer
    {
        bool Equals(ReadOnlySpan<char> x, ReadOnlySpan<char> y);

        int GetHashCode(ReadOnlySpan<char> text);
    }
}