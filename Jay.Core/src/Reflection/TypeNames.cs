using System.Diagnostics;
using Jay.Collections;
using Jay.Text;

namespace Jay.Reflection;

public static class TypeNames
{
    private static readonly ConcurrentTypeMap<string> _typeNameCache;

    static TypeNames()
    {
        _typeNameCache = new()
        {
            [typeof(bool)] = "bool",
            [typeof(DBNull)] = "DBNull",
            [typeof(bool)] = "bool",
            [typeof(char)] = "char",
            [typeof(sbyte)] = "sbyte",
            [typeof(byte)] = "byte",
            [typeof(short)] = "short",
            [typeof(ushort)] = "ushort",
            [typeof(int)] = "int",
            [typeof(uint)] = "uint",
            [typeof(long)] = "long",
            [typeof(ulong)] = "ulong",
            [typeof(float)] = "float",
            [typeof(double)] = "double",
            [typeof(decimal)] = "decimal",
            [typeof(string)] = "string",
            [typeof(object)] = "object",
            [typeof(void)] = "void",
            [typeof(nint)] = "nint",
            [typeof(nuint)] = "nuint",
            [typeof(DateTime)] = "DateTime",
            [typeof(DateTimeOffset)] = "DateTimeOffset",
            [typeof(TimeSpan)] = "TimeSpan",
            [typeof(Guid)] = "Guid",
#if NET6_0_OR_GREATER
            [typeof(TimeOnly)] = "TimeOnly",
            [typeof(DateOnly)] = "DateOnly",
#endif
        };
    }

    private static string GetTypeName(Type type)
    {
        Type? underType;

        // ref
        if (type.IsByRef)
        {
            underType = type.GetElementType()
                .ThrowIfNull();
            return $"ref {ToCode(underType)}";
        }

        // Array
        if (type.IsArray)
        {
            underType = type.GetElementType()
                .ThrowIfNull();
            return $"{ToCode(underType)}[]";
        }

        // Pointer
        if (type.IsPointer)
        {
            underType = type.GetElementType()
                .ThrowIfNull();
            return $"{ToCode(underType)}*";
        }

        // Nullable
        underType = Nullable.GetUnderlyingType(type);
        if (underType is not null)
        {
            return $"{ToCode(underType)}?";
        }

        // After this point, we're building up the name
        var name = StringBuilderPool.Shared.Rent();

        // Nested Type?
        if (type.IsNested && !type.IsGenericParameter)
        {
            underType = type.DeclaringType.ThrowIfNull();
            name.Append(ToCode(underType))
                .Append('.');
        }

        // Fast non-generic return
        if (!type.IsGenericType)
        {
            // just the name
            name.Append(type.Name);
            return name.ToStringAndReturn();
        }

        // Process complex name
        ReadOnlySpan<char> typeName = type.Name.AsSpan();

        // A parameter?
        if (type.IsGenericParameter)
        {
            var constraints = type.GetGenericParameterConstraints();
            Debugger.Break();
        }

        // Name is often something like NAME`2 for NAME<,>, so we want to strip that off
        var i = typeName.IndexOf('`');
        if (i >= 0)
        {
            name.Append(typeName[..i]);
        }
        else
        {
            name.Append(typeName);
        }

        // Add our generic types
        name.Append('<');
        var genericTypes = type.GetGenericArguments();
        Debug.Assert(genericTypes.Length > 0);
        name.Append(ToCode(genericTypes[0]));
        for (i = 1; i < genericTypes.Length; i++)
        {
            name.Append(", ")
                .Append(ToCode(genericTypes[i]));
        }
        name.Append('>');
        return name.ToStringAndReturn();
    }

    public static string ToCode(this Type? type)
    {
        if (type is null)
            return "null";

        return _typeNameCache.GetOrAdd(type, GetTypeName);
    }

    public static string ToCode<T>()
    {
        return _typeNameCache.GetOrAdd<T>(GetTypeName);
    }
}