using Jay.Text.Utilities;

namespace Jay.Reflection.Text;

public static class NamingExtensions
{
    [return: NotNullIfNotNull(nameof(text))]
    public static string? WithNaming(this string? text, Naming naming)
    {
        if (text is null) return null;
        switch (naming)
        {
            case Naming.Lower:
                return text.ToLower();
            case Naming.Upper:
                return text.ToUpper();
            case Naming.Camel:
            {
                int len = text.Length;

                if (len == 0) return "";
                if (char.IsLower(text[0])) return text;

                Span<char> nameBuffer = stackalloc char[len];
                nameBuffer[0] = char.ToLower(text[0]);
                TextHelper.CopyTo(text.AsSpan(1), nameBuffer.Slice(1));
                return nameBuffer.ToString();
            }
            case Naming.Pascal:
            {
                int len = text.Length;

                if (len == 0) return "";
                if (char.IsUpper(text[0])) return text;

                Span<char> nameBuffer = stackalloc char[len];
                nameBuffer[0] = char.ToUpper(text[0]);
                TextHelper.CopyTo(text.AsSpan(1), nameBuffer.Slice(1));
                return nameBuffer.ToString();
            }
            // case Naming.Snake:
            // {
            //     int textLen = text.Length;
            //     if (textLen < 2) return text;
            //
            //     Span<char> nameBuffer = stackalloc char[textLen * 2]; // way large
            //     int n = 0;
            //
            //     bool prevWasCaps = false;
            //     int textIndex = 0;
            //
            //     // First char is just lower
            //     char ch = text[textIndex];
            //     prevWasCaps = char.IsUpper(ch);
            //     nameBuffer[n++] = char.ToLower(ch);
            //
            //     // Process everything else using the usual rules
            //     for (; textIndex < textLen; textIndex++)
            //     {
            //         ch = text[textIndex];
            //         if (char.IsUpper(ch))
            //         {
            //             // start a new segment so long as we're not part of a possible acronym
            //             if (!prevWasCaps)
            //             {
            //                 nameBuffer[n++] = '_';
            //             }
            //             nameBuffer[n++] = char.ToLower(ch);
            //             prevWasCaps = true;
            //         }
            //         else
            //         {
            //             nameBuffer[n++] = ch;
            //             prevWasCaps = false;
            //         }
            //     }
            //
            //     return nameBuffer.Slice(0, n).ToString();
            // }
            case Naming.Field:
            {
                int len = text.Length;
                if (len == 0) return "";
                Span<char> nameBuffer = stackalloc char[len + 1];
                nameBuffer[0] = '_';
                nameBuffer[1] = char.ToLower(text[0]);
                TextHelper.CopyTo(text.AsSpan(1), nameBuffer.Slice(2));
                return nameBuffer.ToString();
            }
            case Naming.Default:
            default:
                return text;
        }
    }
}