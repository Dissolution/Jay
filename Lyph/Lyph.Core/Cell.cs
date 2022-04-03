using System.Drawing;

namespace Lyph;

public abstract class Cell //: IEquatable<Cell>
{
    public Point Position { get; protected init; }
    public Color Color { get; protected init; }
    public Glyph Glyph { get; protected init; }


    public double Bleed { get; protected set; }
    public double Drain { get; protected set; }
}