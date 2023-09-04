namespace Jay.SourceGen.Reflection;

[Flags]
public enum ObjType
{
    Struct = 1 << 0,

    Class = 1 << 1,

    Record = 1 << 2,
    
    Interface = 1 << 3 | Class,

    Delegate = 1 << 4 | Class,
}