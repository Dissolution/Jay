namespace Jay.SourceGen.Reflection;

[Flags]
public enum ObjType
{
    Struct = 1 << 0,

    Class = 1 << 1,

    Interface = 1 << 2 | Class,

    Delegate = 1 << 3 | Class,

    Any = Struct | Class | Interface | Delegate,
}