namespace Jay.Reflection;

[Flags]
public enum Keywords : ulong
{
    None = 0,
    Abstract = 1 << 0,
    Sealed = 1 << 1,
    Virtual = 1 << 2,
    Extern = 1 << 3,
    Override = 1 << 4,
    Const = 1 << 5,
    Required = 1 << 6,
    Volatile = 1 << 7,
    Readonly = 1 << 8,
    Init = 1 << 9,
    Async = 1 << 10,
    New = 1 << 11,
    Unsafe = 1 << 12,
}