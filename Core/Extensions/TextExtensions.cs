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
    }
}