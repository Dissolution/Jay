namespace Jay.Extensions;

public static class TypeExtensions
{
    public static bool Implements<T>(this Type? type)
    {
        return Implements(type, typeof(T));
    }

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

    public static bool CanContainNull(this Type? type)
    {
        return type switch
        {
            null => true,
            { IsAbstract: true, IsSealed: true } => false, // static
            { IsValueType: true } => Nullable.GetUnderlyingType(type) != null, // Is Nullable
            _ => true,
        };
    }

    /// <summary>
    /// Is this <see cref="Type"/> a <see cref="Nullable{T}"/>?
    /// </summary>
    public static bool IsNullable(this Type? type)
    {
        return type is not null && type.Implements(typeof(Nullable<>));
    }

    public static bool IsNullable(this Type? type, [NotNullWhen(true)] out Type? underlyingType)
    {
        if (type is { IsValueType: true, IsGenericType: true }
            && type.GetGenericTypeDefinition() == typeof(Nullable<>))
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

    public static IEnumerable<Type> GetAllBaseTypes(this Type type, bool includeSelf = false)
    {
        if (includeSelf)
            yield return type;
        Type? baseType = type.BaseType;
        while (baseType is not null)
        {
            yield return baseType;
            baseType = baseType.BaseType;
        }
    }

    public static IReadOnlyCollection<Type> GetAllImplementedTypes(this Type type)
    {
        var types = new HashSet<Type>();
        Type? baseType = type;
        while (baseType != null)
        {
            types.Add(baseType);
            foreach (Type face in baseType.GetInterfaces())
            {
                types.Add(face);
            }
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