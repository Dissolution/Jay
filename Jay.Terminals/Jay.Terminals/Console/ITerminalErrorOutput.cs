namespace Jay.Terminals.Console;

public interface ITerminalErrorOutput
{
    TextWriter Writer { get; set; }
    bool IsRedirected { get; }

    Stream OpenStream();
    Stream OpenStream(int bufferSize);
}

internal partial class TerminalInstance : ITerminalErrorOutput
{
    public ITerminalErrorOutput Error => this;
    
    /// <summary>
    /// Gets or sets the <see cref="TextWriter"/> the Error outputs to.
    /// </summary>
    TextWriter ITerminalErrorOutput.Writer
    {
        get => ConsoleFunc(() => SystemConsole.Error);
        set => ConsoleAction(() => SystemConsole.SetError(value));
    }

    /// <summary>
    /// Has the error stream been redirected from standard?
    /// </summary>
    bool ITerminalErrorOutput.IsRedirected => ConsoleFunc(() => SystemConsole.IsErrorRedirected);

    /// <summary>
    /// Acquires the standard error <see cref="Stream"/>.
    /// </summary>
    /// <returns></returns>
    Stream ITerminalErrorOutput.OpenStream() => ConsoleFunc(() => SystemConsole.OpenStandardError());

    /// <summary>
    /// Acquires the standard error <see cref="Stream"/>, which is set to a specified buffer size.
    /// </summary>
    /// <param name="bufferSize"></param>
    /// <returns></returns>
    Stream ITerminalErrorOutput.OpenStream(int bufferSize) => ConsoleFunc(() => SystemConsole.OpenStandardError(bufferSize));
}