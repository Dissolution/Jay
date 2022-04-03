using System.Drawing;
using Microsoft.Toolkit.HighPerformance;

namespace Lyph;

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