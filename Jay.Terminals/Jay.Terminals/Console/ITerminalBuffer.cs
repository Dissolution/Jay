using Jay.Terminals.Native;

namespace Jay.Terminals.Console;

public interface ITerminalBuffer
{
    /// <summary>
    /// Gets or sets the width of the buffer area.
    /// </summary>
    int Width { get; set; }
    
    /// <summary>
    /// Gets or sets the height of the buffer area.
    /// </summary>
    int Height { get; set; }
    
    /// <summary>
    /// Gets or sets the <see cref="Size"/> of the buffer area.
    /// </summary>
    SizeI16 Size { get; set; }
    
    /// <summary>
    /// Copies a specified source <see cref="RectI16"/> of the screen buffer to a specified destination <see cref="PointI16"/>.
    /// </summary>
    /// <param name="area"></param>
    /// <param name="position"></param>
    /// <returns></returns>
    void Copy(RectI16 area, PointI16 position);

    /// <summary>
    /// Clears the <see cref="ITerminalBuffer"/>
    /// </summary>
    void Clear();
}

internal partial class TerminalInstance : ITerminalBuffer
{
    public ITerminalBuffer Buffer => this;

    int ITerminalBuffer.Width
    {
        get => ConsoleFunc(() => SystemConsole.BufferWidth);
        set => ConsoleAction(() => SystemConsole.BufferWidth = value);
    }
    
    int ITerminalBuffer.Height
    {
        get => ConsoleFunc(() => SystemConsole.BufferHeight);
        set => ConsoleAction(() => SystemConsole.BufferHeight = value);
    }
    
    SizeI16 ITerminalBuffer.Size
    {
        get => ConsoleFunc(() => SizeI16.Create(SystemConsole.BufferWidth, SystemConsole.BufferHeight));
        set => ConsoleAction(() => SystemConsole.SetBufferSize(value.Width, value.Height));
    }
    
    void ITerminalBuffer.Copy(RectI16 area, PointI16 position)
    {
        ConsoleAction(() => SystemConsole.MoveBufferArea(area.Left, area.Top, area.Width, area.Height, position.X, position.Y));
    }

    void ITerminalBuffer.Clear() => ConsoleAction(() => SystemConsole.Clear());
}