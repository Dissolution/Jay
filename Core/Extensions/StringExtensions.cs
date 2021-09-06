using System;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;

namespace Jay
{
    public static class StringExtensions
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool IsNullOrEmpty([NotNullWhen(false)] this string? text)
        {
            return (text == null || 0u >= (uint)text.Length) ? true : false;
        }
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool IsNullOrWhiteSpace([NotNullWhen(false)] this string? text)
        {
            if (text == null) return true;
            for (int i = 0; i < text.Length; i++)
            {
                if (!char.IsWhiteSpace(text[i])) return false;
            }
            return true;
        }

        public static ReadOnlySpan<char> Slice(this string? text, int start)
        {
            ReadOnlySpan<char> span = text;
            return span.Slice(start);
        }
        
        public static ReadOnlySpan<char> Slice(this string? text, int start, int length)
        {
            ReadOnlySpan<char> span = text;
            return span.Slice(start, length);
        }
        
        public static ReadOnlySpan<char> Slice(this string? text, Range range)
        {
            ReadOnlySpan<char> span = text;
            return span[range];
        }
    }
}