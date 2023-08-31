using Jay.Text.Building;

namespace Jay.Text.Scratch;

public interface ITextWriter : IDisposable
{
    /// <summary>
    /// Writes a single <see cref="char"/> to this <see cref="ITextWriter"/>
    /// </summary>
    void Write(char ch);

    /// <summary>
    /// Writes a <see cref="Array">char[]</see> to this <see cref="ITextWriter"/>
    /// </summary>
    void Write(params char[]? chars);
    
    /// <summary>
    /// Writes a <see cref="ReadOnlySpan{T}">ReadOnlySpan&lt;char&gt;</see> to this <see cref="ITextWriter"/>
    /// </summary>
    void Write(scoped ReadOnlySpan<char> text);

    /// <summary>
    /// Writes a <see cref="string"/> to this <see cref="ITextWriter"/>
    /// </summary>
    void Write(string? str);

    /// <summary>
    /// Writes an interpolated string to this <see cref="ITextWriter"/>
    /// </summary>
    /// <param name="interpolatedText"></param>
    void Write([InterpolatedStringHandlerArgument("")] ref InterpolatedTextWriter interpolatedText);

    void WriteLine();
    
    /// <summary>
    /// Formats a <typeparamref name="T"/> <paramref name="value"/> into this <see cref="ITextWriter"/>
    /// </summary>
    /// <param name="value"></param>
    /// <param name="format"></param>
    /// <param name="provider"></param>
    /// <typeparam name="T"></typeparam>
    void Format<T>(T? value, scoped ReadOnlySpan<char> format = default, IFormatProvider? provider = null);
    
    /// <summary>
    /// Formats a <typeparamref name="T"/> <paramref name="value"/> into this <see cref="ITextWriter"/>
    /// </summary>
    /// <param name="value"></param>
    /// <param name="format"></param>
    /// <param name="provider"></param>
    /// <typeparam name="T"></typeparam>
    void Format<T>(T? value, string? format = null, IFormatProvider? provider = null);
}