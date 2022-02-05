using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;

namespace Jay;

public static class StringExtensions
{
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool IsNullOrEmpty([NotNullWhen(false)] this string? text)
    {
        return text == null || (uint)text.Length <= 0u;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool IsNullOrWhiteSpace([NotNullWhen(false)] this string? text)
    {
        if (text == null) return true;
        for (int i = text.Length - 1; i >= 0; i--)
        {
            if (!char.IsWhiteSpace(text[i])) return false;
        }
        return true;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool TryGetChar(this string? text, int index, out char ch)
    {
        if (text is not null && (uint)index < (uint)text.Length)
        {
            ch = text[index];
            return true;
        }
        ch = default;
        return false;
    }
}