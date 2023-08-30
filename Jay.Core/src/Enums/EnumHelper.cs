namespace Jay.Enums;

public static class EnumHelper
{
    private static readonly char[] _splitChars = new char[3]
    {
        ' ',
        ',',
        '|',
    };
    
    public static bool TryParse<TEnum>(
        [AllowNull, NotNullWhen(true)] string? str,
        out TEnum @enum)
    where TEnum : struct, Enum
    {
        @enum = default;
        if (string.IsNullOrEmpty(str))
            return false;
        if (Enum.TryParse<TEnum>(
            str,
            true,
            out @enum))
            return true;

        var split = str!.Split(_splitChars, StringSplitOptions.RemoveEmptyEntries);
        foreach (var segment in split)
        {
            if (Enum.TryParse<TEnum>(
                segment,
                true,
                out var flag))
            {
                @enum.AddFlag(flag);
            }
            else
            {
                // could not parse this segment, invalid
                @enum = default;
                return false;
            }
        }
        return true;
    }
}