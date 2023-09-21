namespace Jay.Reflection;

[Flags]
public enum Keywords
{
    New = 1 << 1,
    Const = 1 << 2,
    Abstract = 1 << 3,
    Virtual = 1 << 4,
    Sealed = 1 << 5,
    Readonly = 1 << 6,
    Override = 1 << 7,
    Extern = 1 << 8,
    Unsafe = 1 << 9,
    Volatile = 1 << 10,
    Async = 1 << 11,
    Required = 1 << 12,
    Init = 1 << 13,
}