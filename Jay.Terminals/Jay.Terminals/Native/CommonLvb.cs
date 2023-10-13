namespace Jay.Terminals.Native;

/// <see href="https://learn.microsoft.com/en-us/windows/console/char-info-str"/>
[Flags]
public enum CommonLvb : byte
{
    LeadingByte       = 0b00000001,
    TrailingByte      = 0b00000010,
    GridTopHorizontal = 0b00000100,
    GridLeftVertical  = 0b00001000,
    GridRightVertical = 0b00010000,
    //Gap             = 0b00100000,
    ReverseVideo      = 0b01000000,
    Underscore        = 0b10000000,
}