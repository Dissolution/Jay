using TK = Microsoft.CodeAnalysis.TypeKind;

namespace Jay.SourceGen.Extensions;

public static class TypeExtensions
{
    public static TK TypeKind(this Type type)
    {
        if (type.IsArray)
            return TK.Array;
        if (type.IsEnum)
            return TK.Enum;
        if (type.IsInterface)
            return TK.Interface;
        if (type.IsPointer)
            return TK.Pointer;
        if (type.IsValueType)
            return TK.Struct;
        if (type.IsClass)
        {
            if (typeof(Delegate).IsAssignableFrom(type))
                return TK.Delegate;
            return TK.Class;
        }

        return TK.Unknown;
    }
}