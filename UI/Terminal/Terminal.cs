using System;
using System.Drawing;

namespace Jay.UI.Terminal
{
    public sealed class Terminal : TerminalSection<Terminal>
    {
        public static TerminalBuffer Buffer { get; } = new TerminalBuffer();

        public Terminal()
            : base(new Rectangle(Point.Empty, new Size(Buffer.Width, Buffer.Height)))
        {
            
        }
    }

    public abstract class TerminalSection
    {
        // This is my bounds within Buffer
        protected readonly Rectangle _bounds;
        // This is my position within my Bounds (not Buffer)
        protected Point _position;

        public Size Size => _bounds.Size;
        
        protected TerminalSection(Rectangle bounds)
        {
            _bounds = bounds;
            _position = Point.Empty;
        }

        protected int GetRealIndex(int x, int y)
        {
            return (x + _bounds.X) + ((y + _bounds.Y) * Terminal.Buffer.Width);
        }
        
        protected void Write(ReadOnlySpan<char> characters)
        {
            using (var writer = Terminal.Buffer.Write)
            {
                var bounds = this._bounds;
                // Use real x/y for the Buffer
                var x = _position.X;
                var y = _position.Y;
                for (var t = 0; t < characters.Length; t++)
                {
                    writer[GetRealIndex(x,y)].UnicodeChar = characters[t];
                    x++;
                    if (x >= bounds.Right)
                    {
                        x = _bounds.Left;
                        y++;
                    }
                }




                // var xPos = writer.
                //
                // var xPos = _position.X;
                // var yPos = _position.Y;
                // var bounds = _bounds;
                // var writeCount = characters.Length;
                // int bufferOffset = (xPos + bounds.X) + ((yPos + bounds.Y) * writer.Width);
                // Debug.Assert(bufferOffset >= 0 && bufferOffset < writer.Length);
                // int textPos = 0;
                // while (true)
                // {
                //     while (xPos < bounds.Right && textPos < writeCount)
                //     {
                //         Debug.Assert(bufferOffset >= 0 && bufferOffset < writer.Length);
                //         writer[bufferOffset++].UnicodeChar = characters[textPos++];
                //     }
                //
                //     if (textPos >= writeCount)
                //     {
                //         break;
                //     }
                //     else
                //     {
                //         xPos = 0;
                //     }
                //     
                //     for (; xPos < _bounds.X; xPos++)
                //     {
                //         
                //         writer[bufferOffset++].UnicodeChar = characters[textPos++];
                //         if (textPos >= characters.Length)
                //             break;
                //     }
                //
                //     if (textPos < characters.Length)
                //     {
                //         x = _bounds.X;
                //         bufferOffset += ((writer.Width - bounds.Right) + (bounds.Left));
                //     }
                //     else
                //     {
                //         break;
                //     }
                // }
            }
        }
    }
    
    public abstract class TerminalSection<TSelf> : TerminalSection
        where TSelf : TerminalSection<TSelf>
    {
        protected readonly TSelf _self;
        
        protected TerminalSection(Rectangle bounds)
            : base(bounds)
        {
            _self = (this as TSelf)!;
        }

        // public TSelf Write<T>([AllowNull] T value)
        // {
        //     ReadOnlySpan<char> text = value?.ToString();
        //     Write(text);
        //     return _self;
        // }
    }
}