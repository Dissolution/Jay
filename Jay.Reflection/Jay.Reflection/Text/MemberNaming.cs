using System.Globalization;
using Jay.Text.Building;
using Jay.Text.Utilities;

namespace Jay.Reflection.Text;

/// <summary>
/// Methods to assist with the naming of <c>dynamic</c> and Runtime members
/// </summary>
public static class MemberNaming
{
    private const char BAD_CHAR = 'X';
    
    /// <summary>
    /// Is the given <paramref name="ch"/> a valid <see cref="MemberInfo"/> Name <see cref="char"/>?
    /// </summary>
    /// <param name="ch">The <see cref="char"/> to validate.</param>
    /// <param name="firstChar">Whether or not you're validating the first character in a Member Name.</param>
    /// <returns><c>true</c> if <paramref name="ch"/> is valid; otherwise <c>false</c></returns>
    /// <see href="https://stackoverflow.com/questions/950616/what-characters-are-allowed-in-c-sharp-class-name"/>
    public static bool IsValidNameCharacter(char ch, bool firstChar = false)
    {
        // Fast allow underscore always
        // '_'.UnicodeCategory = ConnectorPunctuation
        if (ch == '_') return true;

        var category = char.GetUnicodeCategory(ch);

        // Always allowed
        if (category is UnicodeCategory.UppercaseLetter
            or UnicodeCategory.LowercaseLetter
            or UnicodeCategory.TitlecaseLetter
            or UnicodeCategory.ModifierLetter
            or UnicodeCategory.OtherLetter)
        {
            return true;
        }

        // No further characters are valid for a first char
        if (firstChar) return false;

        // Remaining allowed characters
        return category is UnicodeCategory.NonSpacingMark
            or UnicodeCategory.SpacingCombiningMark
            or UnicodeCategory.DecimalDigitNumber
            or UnicodeCategory.LetterNumber
            or UnicodeCategory.Format
            or UnicodeCategory.ConnectorPunctuation;
    }

    /// <summary>
    /// Is the given <paramref name="name"/> a valid <see cref="MemberInfo"/> name?
    /// </summary>
    public static bool IsValidMemberName([NotNullWhen(true)] string? name)
    {
        if (name is null) return false;
        var len = name.Length;
        if (len == 0) return false;
        char ch = name[0];
        if (!IsValidNameCharacter(ch, true)) return false;
        for (var i = 1; i < len; i++)
        {
            if (!IsValidNameCharacter(ch, false)) return false;
        }
        return true;
    }

    public static string CreateMemberName(MemberTypes memberType, string? suggestedName = null)
    {
        ReadOnlySpan<char> name = suggestedName.AsSpan();
        if (name.IsWhiteSpace())
        {
            // No good name passed, return semi-random name
            return $"{memberType}_{Guid.NewGuid():N}";
        }

        var builder = new InterpolatedTextWriter();
        char ch = name[0];
        bool appendedBadChar = false;
        if (!IsValidNameCharacter(ch, true))
        {
            builder.AppendFormatted('_');
        }
        builder.AppendFormatted(ch);
        for (var i = 1; i < name.Length; i++)
        {
            ch = name[i];
            if (IsValidNameCharacter(ch))
            {
                builder.AppendFormatted(ch);
                appendedBadChar = false;
            }
            else if (!appendedBadChar)
            {
                builder.AppendFormatted(BAD_CHAR);
                appendedBadChar = true;
            }
        }
        return builder.ToStringAndDispose();
    }
    
    /// <summary>
    /// Creates a backing <see cref="FieldInfo"/> name for a <see cref="PropertyInfo"/>
    /// </summary>
    public static string CreateBackingFieldName(PropertyInfo property)
    {
        var propertyName = property.Name.AsSpan();
        Span<char> buffer = stackalloc char[propertyName.Length + 1];
        buffer[0] = '_';
        TextHelper.CopyTo(propertyName, buffer[1..]);
        return buffer.ToString();
    }

    public static string CreateInterfaceImplementationName(Type interfaceType)
    {
        ReadOnlySpan<char> interfaceName = interfaceType.Name.AsSpan();
        int offset = interfaceName.StartsWith('I') ? 1 : 0;
        Span<char> buffer = stackalloc char[(interfaceName.Length - offset) + 4];
        TextHelper.CopyTo(interfaceName[offset..], buffer);
        TextHelper.CopyTo("Impl", buffer[^4..]);
        return buffer.ToString();
    }
}