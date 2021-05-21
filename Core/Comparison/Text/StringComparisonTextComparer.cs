using System;
using System.Collections;
using System.Runtime.CompilerServices;

namespace Jay.Comparison
{
    internal sealed class StringComparisonTextComparer : ITextEqualityComparer,
                                                         ITextComparer
    {
        private readonly StringComparison _comparison;

        public StringComparisonTextComparer(StringComparison comparison)
        {
            _comparison = comparison;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool Equals(ReadOnlySpan<char> x, ReadOnlySpan<char> y)
        {
            return MemoryExtensions.Equals(x, y, _comparison);
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public int GetHashCode(ReadOnlySpan<char> text)
        {
            return string.GetHashCode(text, _comparison);
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public int Compare(ReadOnlySpan<char> x, ReadOnlySpan<char> y)
        {
            return MemoryExtensions.CompareTo(x, y, _comparison);
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool Equals(string? x, string? y)
        {
            return MemoryExtensions.Equals(x, y, _comparison);
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public int GetHashCode(string? str)
        {
            return string.GetHashCode(str, _comparison);
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public int Compare(string? x, string? y)
        {
            return MemoryExtensions.CompareTo(x, y, _comparison);
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool Equals(char[]? x, char[]? y)
        {
            return MemoryExtensions.Equals(x, y, _comparison);
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public int GetHashCode(char[]? chars)
        {
            return string.GetHashCode(chars, _comparison);
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public int Compare(char[]? x, char[]? y)
        {
            return MemoryExtensions.CompareTo(x, y, _comparison);
        }

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
            return MemoryExtensions.Equals(left, right, _comparison);
        }

        int IEqualityComparer.GetHashCode(object obj)
        {
            ReadOnlySpan<char> span = obj switch
            {
                string str => str,
                char[] chars => chars,
                _ => obj?.ToString()
            };
            return string.GetHashCode(span, _comparison);
        }

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
            return MemoryExtensions.CompareTo(left, right, _comparison);
        }

      
    }
}