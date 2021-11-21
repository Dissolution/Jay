using System;
using System.Collections;

namespace Jay.Comparison
{
    public abstract class TextComparer<TSelf> : ITextEqualityComparer,
                                                ITextComparer
        where TSelf : TextComparer<TSelf>, new()
    {
        public static TSelf Instance { get; } = new TSelf();

        protected TextComparer()
        {
            
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
            return Equals(left, right);
        }
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
        
        public abstract bool Equals(ReadOnlySpan<char> x, ReadOnlySpan<char> y);
        public abstract int GetHashCode(ReadOnlySpan<char> text);
        public abstract int Compare(ReadOnlySpan<char> x, ReadOnlySpan<char> y);
        
        public bool Equals(string? x, string? y) => Equals((ReadOnlySpan<char>) x, (ReadOnlySpan<char>) y);
        public bool Equals(char[]? x, char[]? y) => Equals((ReadOnlySpan<char>) x, (ReadOnlySpan<char>) y);
        public int GetHashCode(string? str) => GetHashCode((ReadOnlySpan<char>) str);
        public int GetHashCode(char[]? chars) => GetHashCode((ReadOnlySpan<char>) chars);
        public virtual int Compare(string? x, string? y) => Compare((ReadOnlySpan<char>)x, (ReadOnlySpan<char>)y);
        public virtual int Compare(char[]? x, char[]? y) => Compare((ReadOnlySpan<char>)x, (ReadOnlySpan<char>)y);
    }
}