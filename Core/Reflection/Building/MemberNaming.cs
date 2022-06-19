﻿using System.Globalization;
using System.Reflection;
using Jay.Randomization;
using Jay.Text;

namespace Jay.Reflection.Building;

/// <summary>
/// Methods to assist with the naming of <see cref="dynamic"/> and Runtime members
/// </summary>
public static class MemberNaming
{
    /// <summary>
    /// Is the given <see cref="char"/> <paramref name="ch"/> valid as the first character in a <see cref="MemberInfo"/> name?
    /// </summary>
    /// <see cref="https://stackoverflow.com/questions/950616/what-characters-are-allowed-in-c-sharp-class-name"/>
    private static bool IsValidNameFirstChar(char ch)
    {
        var category = char.GetUnicodeCategory(ch);
        return ch == '_' ||
               category == UnicodeCategory.UppercaseLetter ||
               category == UnicodeCategory.LowercaseLetter ||
               category == UnicodeCategory.TitlecaseLetter ||
               category == UnicodeCategory.ModifierLetter ||
               category == UnicodeCategory.OtherLetter;
    }

    /// <summary>
    /// Is the given <see cref="char"/> <paramref name="ch"/> valid as any non-first character in a <see cref="MemberInfo"/> name?
    /// </summary>
    /// <see cref="https://stackoverflow.com/questions/950616/what-characters-are-allowed-in-c-sharp-class-name"/>
    private static bool IsValidNameChar(char ch)
    {
        var category = char.GetUnicodeCategory(ch);
        return category == UnicodeCategory.UppercaseLetter ||
               category == UnicodeCategory.LowercaseLetter ||
               category == UnicodeCategory.TitlecaseLetter ||
               category == UnicodeCategory.ModifierLetter ||
               category == UnicodeCategory.OtherLetter ||
               category == UnicodeCategory.NonSpacingMark ||
               category == UnicodeCategory.SpacingCombiningMark ||
               category == UnicodeCategory.DecimalDigitNumber ||
               category == UnicodeCategory.LetterNumber ||
               category == UnicodeCategory.Format ||
               category == UnicodeCategory.ConnectorPunctuation;
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
        if (!IsValidNameFirstChar(ch)) return false;
        for (var i = 1; i < len; i++)
        {
            if (!IsValidNameChar(ch)) return false;
        }
        return true;
    }

    /// <summary>
    /// Tries to write a <paramref name="name"/> to a <see cref="TextBuilder"/>, returning if it was validly written
    /// </summary>
    internal static bool TryWriteName([NotNullWhen(true)] string? name, TextBuilder text)
    {
        if (string.IsNullOrEmpty(name)) return false;

        int start = text.Length;
        char ch;
        int i;
        bool valid = false;
        // Get a valid first name char
        ch = name[0];
        if (IsValidNameFirstChar(ch))
        {
            text.Write(ch);
            // We used the first char
            i = 1;
            valid = true;
        }
        else
        {
            // Have to start with underscore
            text.Write('_');
            i = 0;
        }

        for (;i < name.Length; i++)
        {
            ch = name[i];
            if (IsValidNameChar(ch))
            {
                text.Write(ch);
                valid = true;
            }
        }

        if (!valid)
        {
            text.Length = start;
            return false;
        }

        return true;
    }

    public static string CreateMemberName(string? suggestedName = null)
    {
        using var text = TextBuilder.Borrow();
        if (!TryWriteName(suggestedName, text))
            text.Write(Randomizer.BitString);
        return text.ToString();
    }

    /// <summary>
    /// Gets a backing <see cref="FieldInfo"/> name from a <see cref="PropertyInfo"/> name
    /// </summary>
    public static string FieldName(string propertyName)
    {
        return string.Create(propertyName.Length + 1, propertyName, (span, name) =>
        {
            span[0] = '_';
            span[1] = char.ToLower(name[0]);
            for (var i = 1; i < name.Length; i++)
            {
                span[i + 1] = name[i];
            }
        });
    }

}