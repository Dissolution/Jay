using Microsoft.Toolkit.HighPerformance;

namespace Lyph;

public class Critter //: IEquatable<Critter>
{
    protected readonly Cell[,] _cells;

    public GlyphCode GlyphCode { get; }
    public ReadOnlySpan2D<Cell> Cells => new(_cells);



}