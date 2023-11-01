namespace Jay.Terminals.Console;

public interface ITerminalBeep
{
    /// <summary>
    /// Plays a beep through the console speaker
    /// </summary>
    void Beep();

    /// <summary>
    /// Plays a beep of <paramref name="frequency"/> and <paramref name="duration"/> through the console speaker
    /// </summary>
    /// <param name="frequency">The frequency of the beep, ranging from 37 to 32767 hertz.</param>
    /// <param name="duration">The duration of the beep measured in milliseconds.</param>
    /// <exception cref="ArgumentOutOfRangeException"><br/>
    /// <paramref name="frequency"/> is less than 37 or more than 32767<br/>
    /// -or-<br/>
    /// <paramref name="duration"/> is less than or equal to zero
    /// </exception>
    void Beep(int frequency, int duration);

    /// <summary>
    /// Plays a beep of <paramref name="frequency"/> and <paramref name="duration"/> through the console speaker
    /// </summary>
    /// <param name="frequency">The frequency of the beep, ranging from 37 to 32767 hertz.</param>
    /// <param name="duration">The duration of the beep.</param>
    /// <exception cref="ArgumentOutOfRangeException"><br/>
    /// <paramref name="frequency"/> is less than 37 or more than 32767<br/>
    /// -or-<br/>
    /// <paramref name="duration"/> is less than or equal to zero
    /// </exception>
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