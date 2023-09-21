namespace Jay.Terminals.Console;

public interface ITerminalBeep
{
    /// <summary>
    /// Plays a beep through the console speaker.
    /// </summary>
    void Beep();

    /// <summary>
    /// Plays a beep of specified frequency and duration through the console speaker.
    /// </summary>
    /// <param name="frequency"></param>
    /// <param name="duration">The duration, in milliseconds</param>
    /// <returns></returns>
    void Beep(int frequency, int duration);

    /// <summary>
    /// Plays a beep of specified frequency and duration through the console speaker.
    /// </summary>
    /// <param name="frequency"></param>
    /// <param name="duration"></param>
    /// <returns></returns>
    void Beep(int frequency, TimeSpan duration);
}

internal partial class TerminalInstance : ITerminalBeep
{
    public ITerminalBeep Sound => this;

    void ITerminalBeep.Beep() 
        => ConsoleAction(() => SystemConsole.Beep());

    void ITerminalBeep.Beep(int frequency, int duration) 
        => ConsoleAction(() => SystemConsole.Beep(frequency, duration));

    void ITerminalBeep.Beep(int frequency, TimeSpan duration) 
        => ConsoleAction(() => SystemConsole.Beep(frequency, (int)duration.TotalMilliseconds));
}