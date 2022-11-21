using System.Reflection;
using Jay.Collections;
using Jay.Extensions;
using Jay.Validation;

namespace Jay;

public static class TypeExtensions
{
    public static bool Implements<T>(this Type? type) => Implements(type, typeof(T));

    public static bool Implements(this Type? type, Type? otherType)
    {
        if (type == otherType) return true;
        if (type is null || otherType is null) return false;
        if (otherType.IsAssignableFrom(type)) return true;
        if (type.IsGenericType && otherType.IsGenericTypeDefinition)
            return type.GetGenericTypeDefinition() == otherType;
        if (otherType.HasAttribute<DynamicAttribute>()) return true;
        if (otherType.IsGenericTypeDefinition)
        {
            // Check interface generic types
            // e.g. List<int> : IList<>
            if (type.GetInterfaces().Any(i => i.IsGenericType && i.GetGenericTypeDefinition() == otherType))
                return true;
        }
        return false;
    }

    public static bool IsStatic(this Type? type)
    {
        return type is null || (type.IsAbstract && type.IsSealed);
    }

    public static bool CanContainNull(this Type? type)
    {
        if (type is null) return true;
        if (type.IsStatic()) return false;
        if (type.IsValueType)
            return type.Implements(typeof(Nullable<>));
        return true;
    }

    public static bool IsNullable(this Type? type)
    {
        return type is not null && type.Implements(typeof(Nullable<>));
    }

    public static bool IsNullable(this Type? type, [NotNullWhen(true)] out Type? underlyingType)
    {
        if (type is not null && type.IsValueType && type.IsGenericType && type.GetGenericTypeDefinition() == typeof(Nullable<>))
        {
            underlyingType = type.GetGenericArguments()[0];
            return true;
        }
        underlyingType = null;
        return false;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Type MakeGenericType<T>(this Type type)
    {
        return type.MakeGenericType(typeof(T));
    }

    public static bool IsByRef(this Type? type, out Type underlyingType)
    {
        if (type is null)
        {
            underlyingType = typeof(void);
            return false;
        }

        if (type.IsByRef)
        {
            underlyingType = type.GetElementType()
                                 .ThrowIfNull();
            return true;
        }

        underlyingType = type;
        return false;
    }

    public static (bool ByRef, Type UnderlyingType) IsByRef(this Type? type)
    {
        if (type is null)
        {
            return (false, typeof(void));
        }

        if (type.IsByRef)
        {
            return (true, type.GetElementType()!);
        }

        return (false, type);
    }

    public static bool HasInterface(this Type type, Type interfaceType)
    {
        return type.GetInterfaces().Any(t => t == interfaceType);
    }

    public static bool HasAttribute<TAttribute>(this Type type)
        where TAttribute : Attribute
    {
        return Attribute.IsDefined(type, typeof(TAttribute));
    }

    public static IReadOnlyCollection<Type> GetAllImplementedTypes(this Type type)
    {
        var types = new HashSet<Type>();
        Type? baseType = type;
        while (baseType != null)
        {
            types.Add(baseType);
            foreach (var face in baseType.GetInterfaces())
                types.Add(face);
            baseType = type.BaseType;
        }
        return types;
    }

    public static object? GetDefaultValue(this Type? type)
    {
        if (type is null || type.CanContainNull())
            return null;
        return Activator.CreateInstance(type);
    }

}
