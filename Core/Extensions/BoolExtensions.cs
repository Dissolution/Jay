namespace Jay;

/// <summary>
/// Extensions for <see cref="bool"/>s and <see cref="Nullable"/>&lt;<see cref="bool"/>&gt;s.
/// </summary>
public static class BooleanExtensions
{
    /// <summary>
    /// Convert this <see cref="bool"/> to a <see cref="char"/> representation.
    /// </summary>
    /// <param name="b"></param>
    /// <param name="trueCharacter">The character to return if the value is true.</param>
    /// <param name="falseCharacter">The character to return if the value is false.</param>
    /// <returns></returns>
    public static char ToChar(this bool b, char trueCharacter = 'T', char falseCharacter = 'F')
    {
        return b ? trueCharacter : falseCharacter;
    }

    /// <summary>
    /// Convert this <see cref="Nullable"/>&lt;<see cref="bool"/>&gt; to a <see cref="char"/> representation.
    /// </summary>
    /// <param name="b"></param>
    /// <param name="trueCharacter">The character to return if the value is true.</param>
    /// <param name="falseCharacter">The character to return if the value is false.</param>
    /// <param name="nullCharacter"></param>
    /// <returns></returns>
    public static char ToChar(this bool? b, char trueCharacter = 'T', char falseCharacter = 'F', char nullCharacter = '?')
    {
        return b?.ToChar(trueCharacter, falseCharacter) ?? nullCharacter;
    }

    /// <summary>
    /// Convert this <see cref="bool"/> to a <see cref="string"/> representation.
    /// </summary>
    /// <param name="b"></param>
    /// <param name="trueString">The string to return if the value is true.</param>
    /// <param name="falseString">The string to return if the value is false.</param>
    /// <returns></returns>
    public static string ToString(this bool b, string trueString = "true", string falseString = "false")
    {
        return b ? trueString : falseString;
    }

    /// <summary>
    /// Convert this <see cref="Nullable"/>&lt;<see cref="bool"/>&gt; to a <see cref="string"/> representation.
    /// </summary>
    /// <param name="b"></param>
    /// <param name="trueString">The string to return if the value is true.</param>
    /// <param name="falseString">The string to return if the value is false.</param>
    /// <param name="nullString">The string to return if the value is null.</param>
    /// <returns></returns>
    public static string ToString(this bool? b, string trueString = "true", string falseString = "false", string nullString = "null")
    {
        return b?.ToString(trueString, falseString) ?? nullString;
    }
}