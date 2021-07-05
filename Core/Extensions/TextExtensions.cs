using System;

namespace Jay
{
    public static class TextExtensions
    {
        public static void ConvertToUpper(this Span<char> text)
        {
            for (var i = 0; i < text.Length; i++)
            {
                text[i] = char.ToUpper(text[i]);
            }
        }

        public static void Convert(this Span<char> text, Func<char, char> transform)
        {
            for (var i = 0; i < text.Length; i++)
            {
                text[i] = transform(text[i]);
            }
        }
        
        public static void Convert(this Span<char> text, Func<char, int, char> transform)
        {
            for (var i = 0; i < text.Length; i++)
            {
                text[i] = transform(text[i], i);
            }
        }

        public static bool Equals(this Span<char> text, Span<char> otherText, StringComparison stringComparison)
        {
            return MemoryExtensions.Equals(text, otherText, stringComparison);
        }
        
        public static bool Equals(this Span<char> text, ReadOnlySpan<char> otherText, StringComparison stringComparison)
        {
            return MemoryExtensions.Equals(text, otherText, stringComparison);
        }
    }
}