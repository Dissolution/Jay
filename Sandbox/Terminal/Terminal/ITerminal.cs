using System;
using System.Diagnostics.CodeAnalysis;
using System.Drawing;
using Jay.Sandbox.Native;

namespace Jay.Sandbox
{
    public interface ITerminalSection
    {
        Size Size { get; }
    }

    public interface ITextSection : ITerminalSection
    {
        // Wrap, Truncate, Scroll

        Point Position { get; }

        ITextSection Append(char c);
        ITextSection Append(string? str);
        ITextSection Append(ReadOnlySpan<char> text);
        ITextSection Append(object? obj);
        ITextSection Append<T>([AllowNull] T value);

        ITextSection NewLine();

        ITextSection Colors(CoxelColor? foreColor, CoxelColor? backColor);
        ITextSection Colors(CoxelColors colors);

        ITextSection Colored(CoxelColor? foreColor, CoxelColor? backColor, Action<ITextSection> coloredAction);
        ITextSection Colored(CoxelColors colors, Action<ITextSection> coloredAction);
        IDisposable Colored(CoxelColor? foreColor, CoxelColor? backColor);
        IDisposable Colored(CoxelColors colors);

        ITextSection ColorAppend<T>(CoxelColor? foreColor, CoxelColor? backColor, [AllowNull] T value);
        ITextSection ColorAppend<T>(CoxelColors colors, [AllowNull] T value);
    }
    
    public class ConsoleText : ITextSection
    {
        /// <inheritdoc />
        public Size Size => new Size(Console.BufferWidth, Console.BufferHeight);

        /// <inheritdoc />
        public Point Position => new Point(Console.CursorLeft, Console.CursorTop);

        /// <inheritdoc />
        public ITextSection Append(char c)
        {
            Console.Write(c);
            return this;
        }

        /// <inheritdoc />
        public ITextSection Append(string? str)
        {
            Console.Write(str);
            return this;
        }

        /// <inheritdoc />
        public ITextSection Append(ReadOnlySpan<char> text)
        {
            Console.Out.Write(text);
            return this;
        }

        /// <inheritdoc />
        public ITextSection Append(object? obj)
        {
            Console.Write(obj);
            return this;
        }

        /// <inheritdoc />
        public ITextSection Append<T>([AllowNull] T value)
        {
            Console.Write(value?.ToString());
            return this;
        }

        /// <inheritdoc />
        public ITextSection NewLine()
        {
            Console.WriteLine();
            return this;
        }

        /// <inheritdoc />
        public ITextSection Colors(CoxelColor? foreColor, CoxelColor? backColor)
        {
            if (foreColor.TryGetValue(out var fc))
            {
                Console.ForegroundColor = (ConsoleColor) fc;
            }

            if (backColor.TryGetValue(out var bc))
            {
                Console.BackgroundColor = (ConsoleColor) bc;
            }

            return this;
        }

        /// <inheritdoc />
        public ITextSection Colors(CoxelColors colors)
        {
            Console.ForegroundColor = (ConsoleColor)colors.Foreground;
            Console.BackgroundColor = (ConsoleColor) colors.Background;
            return this;
        }

        /// <inheritdoc />
        public ITextSection Colored(CoxelColor? foreColor, CoxelColor? backColor, Action<ITextSection> coloredAction)
        {
            var oldColors = new CoxelColors((CoxelColor)Console.ForegroundColor, (CoxelColor)Console.BackgroundColor);
            Colors(foreColor, backColor);
            coloredAction(this);
            Colors(oldColors);
            return this;
        }

        /// <inheritdoc />
        public ITextSection Colored(CoxelColors colors, Action<ITextSection> coloredAction)
        {
            var oldColors = new CoxelColors((CoxelColor)Console.ForegroundColor, (CoxelColor)Console.BackgroundColor);
            Colors(colors);
            coloredAction(this);
            Colors(oldColors);
            return this;
        }

        /// <inheritdoc />
        public IDisposable Colored(CoxelColor? foreColor, CoxelColor? backColor)
        {
            var oldColors = new CoxelColors((CoxelColor)Console.ForegroundColor, (CoxelColor)Console.BackgroundColor);
            Colors(foreColor, backColor);
            return Disposable.Action(() => Colors(oldColors));
        }

        /// <inheritdoc />
        public IDisposable Colored(CoxelColors colors)
        {
            var oldColors = new CoxelColors((CoxelColor)Console.ForegroundColor, (CoxelColor)Console.BackgroundColor);
            Colors(colors);
            return Disposable.Action(() => Colors(oldColors));
        }

        /// <inheritdoc />
        public ITextSection ColorAppend<T>(CoxelColor? foreColor, CoxelColor? backColor, [AllowNull] T value)
        {
            var oldColors = new CoxelColors((CoxelColor)Console.ForegroundColor, (CoxelColor)Console.BackgroundColor);
            Colors(foreColor, backColor);
            Console.Write(value?.ToString());
            Colors(oldColors);
            return this;
        }

        /// <inheritdoc />
        public ITextSection ColorAppend<T>(CoxelColors colors, [AllowNull] T value)
        {
            var oldColors = new CoxelColors((CoxelColor)Console.ForegroundColor, (CoxelColor)Console.BackgroundColor);
            Colors(colors);
            Console.Write(value?.ToString());
            Colors(oldColors);
            return this;
        }
    }

}