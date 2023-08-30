namespace Jay.SourceGen.Reflection;

[Flags]
public enum SigType
{
    Field = 1 << 0,
    Property = 1 << 1,
    Event = 1 << 2,
    Constructor = 1 << 3,
    Method = 1 << 4,
    Type = 1 << 5,
    Operator = 1 << 6,
    Parameter = 1 << 7,
    Attribute = 1 << 8,
}
