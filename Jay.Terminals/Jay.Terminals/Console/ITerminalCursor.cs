using Jay.Terminals.Native;
using Jay.Utilities;

namespace Jay.Terminals.Console;

public interface ITerminalCursor
{
    /// <summary>
    /// Gets or sets the column position of the cursor within the buffer area.
    /// </summary>
    short Left { get; set; }
    
    /// <summary>
    /// Gets or sets the row position of the cursor within the buffer area.
    /// </summary>
    short Top { get; set; }
    
    /// <summary>
    /// Gets or sets the cursor position within the buffer area.
    /// </summary>
    PointI16 Position { get; set; }
    
    /// <summary>
    /// Gets or sets the height of the cursor within a cell.
    /// </summary>
    /// <exception cref="ArgumentOutOfRangeException">
    /// Thrown if <see cref="Height"/> is less than 1 or greater than 100
    /// </exception>
    int Height { get; set; }
    
    /// <summary>
    /// Gets or sets whether the cursor is visible.
    /// </summary>
    bool Visible { get; set; }
    
    /// <summary>
    /// Gets an <see cref="IDisposable"/> that, when disposed, resets the <see cref="Terminal"/>'s cursor back to where it was when the <see cref="ColorLock"/> was taken.
    /// </summary>
    IDisposable ResetCursor { get; }
}

internal partial class TerminalInstance : ITerminalCursor
{
    public ITerminalCursor Cursor => this;
    
    /// <summary>
    /// Gets or sets the column position of the cursor within the buffer area.
    /// </summary>
    short ITerminalCursor.Left
    {
        get => ConsoleFunc(() => (short)SystemConsole.CursorLeft);
        set => ConsoleAction(() => SystemConsole.CursorLeft = value);
    }

    /// <summary>
    /// Gets or sets the row position of the cursor within the buffer area.
    /// </summary>
    short ITerminalCursor.Top
    {
        get => ConsoleFunc(() => (short)SystemConsole.CursorTop);
        set => ConsoleAction(() => SystemConsole.CursorTop = value);
    }

    /// <summary>
    /// Gets or sets the cursor position within the buffer area.
    /// </summary>
    PointI16 ITerminalCursor.Position
    {
        get => ConsoleFunc(() => PointI16.Create(SystemConsole.CursorLeft, SystemConsole.CursorTop));
        set => ConsoleAction(() => SystemConsole.SetCursorPosition(value.X, value.Y));
    }

    /// <summary>
    /// Gets or sets the height of the cursor within a cell.
    /// </summary>
    int ITerminalCursor.Height
    {
        get => ConsoleFunc(() => SystemConsole.CursorSize);
        set => ConsoleAction(() => SystemConsole.CursorSize = value);
    }

    /// <summary>
    /// Gets or sets whether the cursor is visible.
    /// </summary>
    bool ITerminalCursor.Visible
    {
        get => ConsoleFunc(() => SystemConsole.CursorVisible);
        set => ConsoleAction(() => SystemConsole.CursorVisible = value);
    }

    public IDisposable ResetCursor
    {
        get
        {
            var (left, top, size, visible) = (
                SystemConsole.CursorLeft, 
                SystemConsole.CursorTop, 
                SystemConsole.CursorSize,
                SystemConsole.CursorVisible);
            return new ActionDisposable(() => ConsoleAction(() =>
            {
                SystemConsole.CursorLeft = left;
                SystemConsole.CursorTop = top;
                SystemConsole.CursorSize = size;
                SystemConsole.CursorVisible = visible;
            }));
        }
    }
}