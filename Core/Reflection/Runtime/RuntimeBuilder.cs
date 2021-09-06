using System;
using System.Globalization;
using System.Reflection;
using Jay.Reflection.Delegates;
using Jay.Text;

namespace Jay.Reflection.Runtime
{
    public static class RuntimeBuilder
    {
        internal static Module Module { get; } = typeof(RuntimeBuilder).Module;

        public static RuntimeTypeBuilder TypeBuilder { get; } = new RuntimeTypeBuilder();

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

        internal static bool IsValidIdentifier(string? name)
        {
            if (name is null) return false;
            var len = name.Length;
            if (len == 0) return false;
            for (var i = 0; i < len; i++)
            {
                if (!IsValidIdentifierChar(name[i], i == 0))
                    return false;
            }
            return true;
        }
        
        internal static char FixChar(char c, int index)
        {
            if (IsValidIdentifierChar(c, index == 0))
                return c;
            return '_';
        }

        internal static string FixName(string? name)
        {
            if (string.IsNullOrWhiteSpace(name))
            {
                return Guid.NewGuid().ToString("N");
            }
            return TextBuilder.Build(text =>
            {
                for (var i = 0; i < name.Length; i++)
                {
                    text.Write(FixChar(name[i], i));
                }
            });
        }

        internal static string ToFieldName(string name)
        {
            return TextBuilder.Build(name, (text, n) =>
            {
                text.Write('_');
                var firstChar = n[0];
                if (!char.IsLower(firstChar))
                {
                    text.Append(char.ToLower(firstChar));
                }
                else
                {
                    text.Append(firstChar);
                }

                for (var i = 1; i < n.Length; i++)
                {
                    text.Append(n[i]);
                }
            });
        }
    }
}