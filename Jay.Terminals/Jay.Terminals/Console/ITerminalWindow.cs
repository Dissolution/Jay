using System.Drawing;
using Jay.Terminals.Native;

namespace Jay.Terminals.Console;

public interface ITerminalWindow
{
    /// <summary>
    /// Gets or sets the title to display in the <see cref="Terminal"/> window.
    /// </summary>
    string Title { get; set; }

    Point Location { get; set; }

    Size Size { get; set; }

    /// <summary>
    /// Gets or sets the bounds of the <see cref="Terminal"/> window.
    /// </summary>
    Rectangle Bounds { get; set; }
}

internal partial class TerminalInstance : ITerminalWindow
{
    public ITerminalWindow Window => this;
    
    /// <summary>
    /// Gets or sets the title to display in the <see cref="Terminal"/> window.
    /// </summary>
    string ITerminalWindow.Title
    {
        get => ConsoleFunc(() => SystemConsole.Title);
        set => ConsoleAction(() => SystemConsole.Title = value);
    }

    Point ITerminalWindow.Location
    {
        get => ConsoleFunc(() => new Point(SystemConsole.WindowLeft, SystemConsole.WindowTop));
        set => ConsoleAction(() =>
        {
            var newBounds = new Rectangle(value.X, value.Y, SystemConsole.WindowWidth, SystemConsole.WindowHeight);
            NativeMethods.MoveAndResizeWindow(_consoleWindowHandle, newBounds);
        });
    }

    Size ITerminalWindow.Size
    {
        get => ConsoleFunc(() => new Size(SystemConsole.WindowWidth, SystemConsole.WindowHeight));
        set => ConsoleAction(() =>
        {
            var newBounds = new Rectangle(SystemConsole.WindowLeft, SystemConsole.WindowTop, value.Width, value.Height);
            NativeMethods.MoveAndResizeWindow(_consoleWindowHandle, newBounds);
        });
    }
    
    /// <summary>
    /// Gets or sets the bounds of the <see cref="Terminal"/> window.
    /// </summary>
    Rectangle ITerminalWindow.Bounds
    {
        get
        {
            return ConsoleFunc(() =>
            {
                if (NativeMethods.GetWindowRect(_consoleWindowHandle, out var bounds))
                    return bounds;
                return Rectangle.Empty;
            });
        }
        set
        {
            ConsoleAction(() =>
            {
                NativeMethods.MoveAndResizeWindow(_consoleWindowHandle, value, true);
            });
        }
    }
}