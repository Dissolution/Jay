using System.Collections;
using System.Drawing;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using Microsoft.Toolkit.HighPerformance;

namespace Lyph;

public enum CardinalDirection : byte
{
    West = 0,
    North = 1,
    East = 2,
    South = 3,

    Left = West,
    Top = North,
    Right = East,
    Bottom = South,
}

public sealed class Cardinals<T> : IEquatable<Cardinals<T>>,
                                   IEnumerable<T>
{
    public static bool operator ==(Cardinals<T> left, Cardinals<T> right) => left.Equals(right);
    public static bool operator !=(Cardinals<T> left, Cardinals<T> right) => !left.Equals(right);

    private readonly T[] _directions;

    public T Left => _directions[(int)CardinalDirection.Left];
    public T Top => _directions[(int)CardinalDirection.Top];
    public T Right => _directions[(int)CardinalDirection.Right];
    public T Bottom => _directions[(int)CardinalDirection.Bottom];

    public Cardinals(T left, T top, T right, T bottom)
    {
        _directions = new T[4] { left, top, right, bottom };
    }

    IEnumerator IEnumerable.GetEnumerator() => _directions.GetEnumerator();
    public IEnumerator<T> GetEnumerator()
    {
        yield return _directions[0];
        yield return _directions[1];
        yield return _directions[2];
        yield return _directions[3];
    }

    public bool Equals(Cardinals<T>? cardinals)
    {
        if (cardinals is not null)
        {
            var otherDirections = cardinals._directions;
            for (var i = 0; i < 4; i++)
            {
                if (!EqualityComparer<T>.Default.Equals(otherDirections[i], _directions[i]))
                    return false;
            }
            return true;
        }
        return false;
    }

    public override bool Equals(object? obj)
    {
        return obj is Cardinals<T> cardinals && Equals(cardinals);
    }

    public override int GetHashCode()
    {
        return HashCode.Combine(Left, Top, Right, Bottom);
    }

    public override string ToString()
    {
        return $"[{Left},{Top},{Right},{Bottom}]";
    }
}

public class World
{
    protected readonly Quadrant[,] _cells;

    public ReadOnlySpan2D<Quadrant> Cells => new ReadOnlySpan2D<Quadrant>(_cells);
    public Size Size { get; }

    public double Intensity { get; set; }

    public World(Size size)
    {
        if (size.Width <= 0)
            throw new ArgumentException($"Invalid Width of {size.Width}: Must be greater than zero!", nameof(size));
        if (size.Height <= 0)
            throw new ArgumentException($"Invalid Height of {size.Height}: Must be greater than zero!", nameof(size));
        this.Size = size;
        _cells = new Quadrant[size.Width, size.Height];
    }
}

public class Quadrant //: IEquatable<Quadrant>
{
    public World World { get; }
    public Point Position { get; }
    public Cardinals<Quadrant> Neighbors { get; }
    public Critter Critter { get; }
}

public struct GlyphCode { }


[Flags]
public enum Color : byte
{
    None =      0b00000000,
    Blue =      0b00001001,
    Green =     0b00001010,
    Cyan =      0b00001011,
    Red =       0b00001100,
    Magenta =   0b00001101,
    Yellow =    0b00001110,
    White =     0b00001111,
}


public class Critter //: IEquatable<Critter>
{
    protected readonly Cell[,] _cells;

    public GlyphCode GlyphCode { get; }
    public ReadOnlySpan2D<Cell> Cells => new(_cells);



}

public abstract class Cell //: IEquatable<Cell>
{
    public Point Position { get; protected init; }
    public Color Color { get; protected init; }
    public Glyph Glyph { get; protected init; }


    public double Bleed { get; protected set; }
    public double Drain { get; protected set; }
}

public sealed class OutsideCell : Cell
{
    public static OutsideCell Instance { get; } = new OutsideCell();

    private OutsideCell()
    {
        Glyph = Glyph.FromSymbol(' ');
        Color = Color.None;
        Bleed = 0.0d;
        Drain = 0.0d;
    }
}