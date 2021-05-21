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
    }
}