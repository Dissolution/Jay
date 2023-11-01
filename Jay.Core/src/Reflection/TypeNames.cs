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
            return $"ref {NameOf(underType)}";
        }

        // Array
        if (type.IsArray)
        {
            underType = type.GetElementType()
                .ThrowIfNull();
            return $"{NameOf(underType)}[]";
        }

        // Pointer
        if (type.IsPointer)
        {
            underType = type.GetElementType()
                .ThrowIfNull();
            return $"{NameOf(underType)}*";
        }

        // Nullable
        underType = Nullable.GetUnderlyingType(type);
        if (underType is not null)
        {
            return $"{NameOf(underType)}?";
        }

        // A parameter?
        if (type.IsGenericParameter)
        {
            var constraints = type.GetGenericParameterConstraints();
            if (constraints.Length > 0)
            {
                Debugger.Break();
            }
            // Print nothing, we want List<>, not List<T>
            return string.Empty;
        }
        
        // After this point, we're building up the name
        var name = StringBuilderPool.Rent();

        // Nested Type?
        if (type.IsNested)
        {
            underType = type.DeclaringType.ThrowIfNull();
            name.Append(NameOf(underType))
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
        name.Append(NameOf(genericTypes[0]));
        for (i = 1; i < genericTypes.Length; i++)
        {
            name.Append(',')
                .Append(NameOf(genericTypes[i]));
        }
        name.Append('>');
        return name.ToStringAndReturn();
    }

    /// <summary>
    /// Gets a <c>C#</c> code representation of this <see cref="Type"/>
    /// </summary>
    public static string NameOf(this Type? type)
    {
        if (type is null)
            return "null";
        return _typeNameCache.GetOrAdd(type, static t => GetTypeName(t));
    }

    /// <summary>
    /// Gets a <c>C#</c> code representation for <typeparamref name="T"/>
    /// </summary>
    public static string NameOf<T>()
    {
        return _typeNameCache.GetOrAdd<T>(static t => GetTypeName(t));
    }
}