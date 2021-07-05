using System.Globalization;
using System.Reflection;
using Jay.Reflection.Delegates;

namespace Jay.Reflection.Runtime
{
    public static class RuntimeBuilder
    {
        internal static Module Module { get; } = typeof(RuntimeBuilder).Module;

        internal static bool IsValidIdentifierChar(char c, bool isFirstChar)
        {
            switch (char.GetUnicodeCategory(c))
            {
                case UnicodeCategory.UppercaseLetter:
                case UnicodeCategory.LowercaseLetter:
                case UnicodeCategory.TitlecaseLetter:
                case UnicodeCategory.ModifierLetter:
                case UnicodeCategory.OtherLetter:
                    // Always allowed in C# identifiers
                    return true;
                case UnicodeCategory.LetterNumber:
                case UnicodeCategory.NonSpacingMark:
                case UnicodeCategory.SpacingCombiningMark:
                case UnicodeCategory.DecimalDigitNumber:
                case UnicodeCategory.ConnectorPunctuation:
                case UnicodeCategory.Format:
                    // Only allowed after first char
                    return !isFirstChar;
                default:
                    return false;
            }
        }

        internal static char FixChar(char c, int index)
        {
            if (IsValidIdentifierChar(c, index == 0))
                return c;
            return '_';
        }
    }
}