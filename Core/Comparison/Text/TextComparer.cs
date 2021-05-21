using System;
using System.Collections;

namespace Jay.Comparison
{
    public abstract class TextComparer : ITextComparer
    {
        public abstract int Compare(ReadOnlySpan<char> x, ReadOnlySpan<char> y);

        public virtual int Compare(string? x, string? y) => Compare((ReadOnlySpan<char>)x, (ReadOnlySpan<char>)y);

        public virtual int Compare(char[]? x, char[]? y) => Compare((ReadOnlySpan<char>)x, (ReadOnlySpan<char>)y);

        int IComparer.Compare(object? x, object? y)
        {
            ReadOnlySpan<char> left = x switch
            {
                string str => str,
                char[] chars => chars,
                _ => x?.ToString()
            };
            ReadOnlySpan<char> right = y switch
            {
                string str => str,
                char[] chars => chars,
                _ => x?.ToString()
            };
            return Compare(left, right);
        }
    }
}