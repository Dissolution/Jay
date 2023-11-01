using Jay.Terminals.Native;

namespace Jay.Terminals.Console;

public interface ITerminalDisplay
{
    /// <summary>
    /// Gets or sets the leftmost position of the <see cref="Terminal"/> display window area relative to the screen buffer.
    /// </summary>
    short Left { get; set; }
    
    /// <summary>
    /// Gets or sets the top position of the <see cref="Terminal"/> display window area relative to the screen buffer.
    /// </summary>
    short Top { get; set; }
    
    /// <summary>
    /// Gets or sets the position of the <see cref="Terminal"/> display window area relative to the screen buffer.
    /// </summary>
    PointI16 Position { get; set; }
    
    /// <summary>
    /// Gets or sets the width of the <see cref="Terminal"/> display window.
    /// </summary>
    short Width { get; set; }
    
    /// <summary>
    /// Gets or sets the height of the <see cref="Terminal"/> display window.
    /// </summary>
    short Height { get; set; }
    
    /// <summary>
    /// Gets or sets the width and height of the <see cref="Terminal"/> display window.
    /// </summary>
    SizeI16 Size { get; set; }
    
    /// <summary>
    /// Gets the largest possible number of <see cref="Terminal"/> display window rows, based on the current font, screen resolution, and window size.
    /// </summary>
    int LargestHeight { get; }
    
    /// <summary>
    /// Gets the largest possible number of <see cref="Terminal"/> display window columns, based on the current font, screen resolution, and window size.
    /// </summary>
    int LargestWidth { get; }
    
    /// <summary>
    /// Clears the <see cref="ITerminalDisplay"/>
    /// </summary>
    void Clear();
}

internal partial class TerminalInstance : ITerminalDisplay
{
    public ITerminalDisplay Display => this;
    
    short ITerminalDisplay.Left
    {
        get => ConsoleFunc(() => (short)SystemConsole.WindowLeft);
        set => ConsoleAction(() => SystemConsole.WindowLeft = value);
    }
    
    short ITerminalDisplay.Top
    {
        get => ConsoleFunc(() => (short)SystemConsole.WindowTop);
        set => ConsoleAction(() => SystemConsole.WindowTop = value);
    }
    
    PointI16 ITerminalDisplay.Position
    {
        get => ConsoleFunc(() => PointI16.Create(SystemConsole.WindowLeft, SystemConsole.WindowTop));
        set => ConsoleAction(() => SystemConsole.SetWindowPosition(value.X, value.Y));
    }
    
    short ITerminalDisplay.Width
    {
        get => ConsoleFunc(() => (short)SystemConsole.WindowWidth);
        set => ConsoleAction(() => SystemConsole.WindowWidth = value);
    }
    
    short ITerminalDisplay.Height
    {
        get => ConsoleFunc(() => (short)SystemConsole.WindowHeight);
        set => ConsoleAction(() => SystemConsole.WindowHeight = value);
    }
    
    SizeI16 ITerminalDisplay.Size
    {
        get => ConsoleFunc(() => SizeI16.Create(SystemConsole.WindowWidth, SystemConsole.WindowHeight));
        set => ConsoleAction(() => SystemConsole.SetWindowSize(value.Width, value.Height));
    }
    
    int ITerminalDisplay.LargestHeight => ConsoleFunc(() => SystemConsole.LargestWindowHeight);
    
    int ITerminalDisplay.LargestWidth => ConsoleFunc(() => SystemConsole.LargestWindowWidth);

    void ITerminalDisplay.Clear() => ConsoleAction(() => SystemConsole.Clear());
    
}