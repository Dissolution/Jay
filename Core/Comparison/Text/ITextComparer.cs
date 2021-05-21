using System;
using System.Collections;
using System.Collections.Generic;

namespace Jay.Comparison
{
    public interface ITextComparer : IComparer<string?>,
                                     IComparer<char[]?>,
                                     IComparer
    {
        int Compare(ReadOnlySpan<char> x, ReadOnlySpan<char> y);
    }
}