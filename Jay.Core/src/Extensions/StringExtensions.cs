namespace Jay.Extensions;

public static class StringExtensions
{
    public static bool IsNullOrEmpty([NotNullWhen(false)] this string? text)
    {
        return string.IsNullOrEmpty(text);
    }

    public static bool IsNullOrWhiteSpace([NotNullWhen(false)] this string? text)
    {
        return string.IsNullOrWhiteSpace(text);
    }

    public static bool IsNonWhiteSpace([NotNullWhen(true)] this string? text)
    {
        return !string.IsNullOrWhiteSpace(text);
    }

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

    [return: NotNullIfNotNull(nameof(ifInvalid))]
    public static string? IfNull(this string? str, string? ifInvalid)
    {
        return str ?? ifInvalid;
    }

    [return: NotNullIfNotNull(nameof(ifInvalid))]
    public static string? IfNullOrEmpty(this string? str, string? ifInvalid)
    {
        if (string.IsNullOrEmpty(str))
            return ifInvalid;
        return str;
    }

    [return: NotNullIfNotNull(nameof(ifInvalid))]
    public static string? IfNullOrWhiteSpace(this string? str, string? ifInvalid)
    {
        if (string.IsNullOrWhiteSpace(str))
            return ifInvalid;
        return str;
    }
}