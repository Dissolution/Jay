namespace Jay.Terminals.Native;

[Flags]
public enum Direction
{
    None = 0,
    
    Left = 1 << 0,
    Top = 1 << 1,
    Right = 1 << 2,
    Bottom = 1 << 3,
    
    UpperLeft = Left | Top,
    UpperRight = Right | Top,
    LowerLeft = Left | Bottom,
    LowerRight = Right | Bottom,
}