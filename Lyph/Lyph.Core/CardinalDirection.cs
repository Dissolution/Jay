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