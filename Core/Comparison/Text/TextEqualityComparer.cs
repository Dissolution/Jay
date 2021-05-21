using System;
using System.Collections;

namespace Jay.Comparison
{
    public abstract class TextEqualityComparer : ITextEqualityComparer
    {
        public abstract bool Equals(ReadOnlySpan<char> x, ReadOnlySpan<char> y);
        public abstract int GetHashCode(ReadOnlySpan<char> text);
        
        public bool Equals(string? x, string? y) => Equals((ReadOnlySpan<char>) x, (ReadOnlySpan<char>) y);

        public bool Equals(char[]? x, char[]? y) => Equals((ReadOnlySpan<char>) x, (ReadOnlySpan<char>) y);

        bool IEqualityComparer.Equals(object? x, object? y)
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
            return Equals(left, right);
        }

        public int GetHashCode(string? str) => GetHashCode((ReadOnlySpan<char>) str);
        
        public int GetHashCode(char[]? chars) => GetHashCode((ReadOnlySpan<char>) chars);

        int IEqualityComparer.GetHashCode(object? obj)
        {
            ReadOnlySpan<char> span = obj switch
            {
                string str => str,
                char[] chars => chars,
                _ => obj?.ToString()
            };
            return GetHashCode(span);
        }
    }
}