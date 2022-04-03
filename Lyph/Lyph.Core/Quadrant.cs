using System.Drawing;

namespace Lyph;

public class Quadrant //: IEquatable<Quadrant>
{
    public World World { get; }
    public Point Position { get; }
    public Cardinals<Quadrant> Neighbors { get; }
    public Critter Critter { get; }
}