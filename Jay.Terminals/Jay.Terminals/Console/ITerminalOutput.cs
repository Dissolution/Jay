using System.Runtime.CompilerServices;
using System.Text;
using Jay.Terminals.Native;
using Jay.Utilities;

namespace Jay.Terminals.Console;

public interface ITerminalOutput
{
    TerminalColor DefaultForeColor { get; }
    
    TerminalColor DefaultBackColor { get; }
    
    /// <summary>
    /// Gets or sets the <see cref="TextWriter"/> used for output.
    /// </summary>
    TextWriter Writer { get; set; }
    
    /// <summary>
    /// Gets or sets the <see cref="System.Text.Encoding"/> the <see cref="Terminal"/> uses to output.
    /// </summary>
    Encoding Encoding { get; set; }
    
    /// <summary>
    /// Has the output stream been redirected from standard?
    /// </summary>
    bool IsRedirected { get; }

    /// <summary>
    /// Gets or sets the foreground <see cref="System.Drawing.Color"/> of the <see cref="Terminal"/>.
    /// </summary>
    TerminalColor ForegroundColor { get; set; }

    /// <summary>
    /// Gets or sets the background <see cref="System.Drawing.Color"/> of the <see cref="Terminal"/>.
    /// </summary>
    TerminalColor BackgroundColor { get; set; }

    // /// <summary>
    // /// Gets or sets the <see cref="IPalette"/> in use for this <see cref="Terminal"/>.
    // /// </summary>
    // IPalette Palette { get; set; }

    /// <summary>
    /// Gets an <see cref="IDisposable"/> that, when disposed, resets the <see cref="Terminal"/>'s colors back to what they were when the <see cref="ColorReset"/> was taken.
    /// </summary>
    IDisposable ColorReset { get; }
    
    /// <summary>
    /// Acquires the standard output <see cref="Stream"/>.
    /// </summary>
    /// <returns></returns>
    Stream OpenStream();
    
    /// <summary>
    /// Acquires the standard output <see cref="Stream"/>, which is set to a specified buffer size.
    /// </summary>
    /// <param name="bufferSize"></param>
    /// <returns></returns>
    Stream OpenStream(int bufferSize);

    /// <summary>
    /// Sets the foreground <see cref="System.Drawing.Color"/> for the <see cref="Terminal"/>.
    /// </summary>
    /// <param name="foreColor"></param>
    /// <returns></returns>
    void SetForeColor(TerminalColor foreColor);

    /// <summary>
    /// Sets the background <see cref="System.Drawing.Color"/> for the <see cref="Terminal"/>.
    /// </summary>
    /// <param name="backColor"></param>
    /// <returns></returns>
    void SetBackColor(TerminalColor backColor);
    
    /// <summary>
    /// Sets the foreground and background <see cref="System.Drawing.Color"/>s for the <see cref="Terminal"/>.
    /// </summary>
    /// <param name="foreColor"></param>
    /// <param name="backColor"></param>
    /// <returns></returns>
    void SetColors(TerminalColor? foreColor = null, TerminalColor? backColor = null);


    void Write(char ch);

    void Write(string? str) => Write(str.AsSpan());

    void Write(params char[]? chars) => Write(chars.AsSpan());

    /// <summary>
    /// Writes the specified array of Unicode characters to the standard output stream.
    /// </summary>
    /// <param name="text"></param>
    /// <returns></returns>
    void Write(scoped ReadOnlySpan<char> text);

    /// <summary>
    /// Writes the specified <see cref="FormattableString"/> value to the standard output stream, using embedded <see cref="System.Drawing.Color"/> values to change the foreground color during the writing process.
    /// </summary>
    /// <param name="interpolatedText"></param>
    /// <returns></returns>
    void Write(ref DefaultInterpolatedStringHandler interpolatedText);
    
    /// <summary>
    /// Writes the text representation of the given <paramref name="value"/> to the standard <see cref="Output"/> stream.
    /// </summary>
    void Write<T>(T? value);
    
    
    /// <summary>
    /// Writes the current line terminator (<see cref="Environment.NewLine"/>) to the standard output stream.
    /// </summary>
    /// <returns></returns>
    void WriteLine();

    void WriteLine(char ch)
    {
        Write(ch);
        WriteLine();
    }
    void WriteLine(string? str) => WriteLine(str.AsSpan());
    void WriteLine(params char[]? chars) => WriteLine(chars.AsSpan());

    /// <summary>
    /// Writes the specified array of Unicode characters, followed by the current line terminator, to the standard output stream.
    /// </summary>
    /// <param name="text"></param>
    /// <returns></returns>
    void WriteLine(scoped ReadOnlySpan<char> text)
    {
        Write(text);
        WriteLine();
    }

    void WriteLine(ref DefaultInterpolatedStringHandler interpolatedText)
    {
        Write(ref interpolatedText);
        WriteLine();
    }
    
    /// <summary>
    /// Writes the text representation of the given <paramref name="value"/> followed by the current line terminator, to the standard <see cref="Output"/> stream.
    /// </summary>
    void WriteLine<T>(T? value)
    {
        Write<T>(value);
        WriteLine();
    }

}

internal partial class TerminalInstance : ITerminalOutput
{
    public ITerminalOutput Output => this;
    
    TextWriter ITerminalOutput.Writer
    {
        get => ConsoleFunc(() => SystemConsole.Out);
        set => ConsoleAction(() => SystemConsole.SetOut(value));
    }
    
    Encoding ITerminalOutput.Encoding
    {
        get => ConsoleFunc(() => SystemConsole.OutputEncoding);
        set => ConsoleAction(() => SystemConsole.OutputEncoding = value);
    }
    
    bool ITerminalOutput.IsRedirected => ConsoleFunc(() => SystemConsole.IsOutputRedirected);
    
    Stream ITerminalOutput.OpenStream() => ConsoleFunc(() => SystemConsole.OpenStandardOutput());
    
    Stream ITerminalOutput.OpenStream(int bufferSize) => ConsoleFunc(() => SystemConsole.OpenStandardOutput(bufferSize));

    TerminalColor ITerminalOutput.DefaultForeColor => TerminalColors.Default.Foreground;

    TerminalColor ITerminalOutput.DefaultBackColor => TerminalColors.Default.Background;

    TerminalColor ITerminalOutput.ForegroundColor
    {
        get => ConsoleFunc(() => (TerminalColor)SystemConsole.ForegroundColor);
        set => ConsoleAction(() => SystemConsole.ForegroundColor = (ConsoleColor)value);
    }

    TerminalColor ITerminalOutput.BackgroundColor
    {
        get => ConsoleFunc(() => (TerminalColor)SystemConsole.BackgroundColor);
        set => ConsoleAction(() => SystemConsole.BackgroundColor = (ConsoleColor)value);
    }

    // IPalette ITerminalOutput.Palette
    // {
    //     get => throw new NotImplementedException();
    //     set => throw new NotImplementedException();
    // }

    IDisposable ITerminalOutput.ColorReset
    {
        get
        {
            var (fore, back) = (SystemConsole.ForegroundColor, SystemConsole.BackgroundColor);
            return new ActionDisposable(() => ConsoleAction(() =>
            {
                SystemConsole.ForegroundColor = fore;
                SystemConsole.BackgroundColor = back;
            }));
        }
    }

    void ITerminalOutput.SetForeColor(TerminalColor foreColor)
    {
        this.Output.ForegroundColor = foreColor;
    }
    void ITerminalOutput.SetBackColor(TerminalColor backColor)
    {
        this.Output.BackgroundColor = backColor;
    }
    void ITerminalOutput.SetColors(TerminalColor? foreColor, TerminalColor? backColor)
    {
        if (foreColor.HasValue)
            this.Output.ForegroundColor = foreColor.Value;
        if (backColor.HasValue)
            this.Output.BackgroundColor = backColor.Value;
    }

    void ITerminalOutput.Write(char ch) 
        => ConsoleAction(() => SystemConsole.Write(ch));

    void ITerminalOutput.Write(scoped ReadOnlySpan<char> text)
    {
        _slimLock.TryEnterWriteLock(-1);
        try
        {
            SystemConsole.Out.Write(text);
        }
        finally
        {
            _slimLock.ExitWriteLock();
        }
    }

    void ITerminalOutput.Write(ref DefaultInterpolatedStringHandler interpolatedText)
    {
        string str = interpolatedText.ToStringAndClear();
        ConsoleAction(() => SystemConsole.Write(str));
    }

    void ITerminalOutput.Write<T>(T? value) where T : default
    {
        ConsoleAction(() => SystemConsole.Write(value));
    }

    void ITerminalOutput.WriteLine()
    {
        ConsoleAction(() => SystemConsole.WriteLine());
    }
}