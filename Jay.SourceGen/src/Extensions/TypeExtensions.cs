namespace Jay.CodeGen.Extensions;

using static Visibility;

public static class TypeExtensions
{
    /// <summary>
    /// Gets a singular public method named <c>Invoke</c> this <see cref="Type"/> has, or <c>null</c> if it doesn't have exactly one.
    /// </summary>
    /// <param name="delegateType"></param>
    /// <returns></returns>
    public static MethodInfo? GetInvokeMethod(this Type? delegateType)
    {
        return delegateType?
            .GetMethods(BindingFlags.Public | BindingFlags.Instance)
            .Where(method => method.Name == "Invoke")
            .OneOrDefault();
    }

    public static bool IsStatic(this Type? type)
    {
        return type is null || (type.IsAbstract && type.IsSealed);
    }

    public static Visibility Visibility(this Type? type)
    {
        var visibility = None;
        if (type is null) return visibility;
        if (type.IsStatic())
            visibility |= Static;
        else
            visibility |= Instance;
        if (IsPublic(type))
            visibility |= Public;
        if (IsInternal(type))
            visibility |= Internal;
        if (IsProtected(type))
            visibility |= Protected;
        if (IsPrivate(type))
            visibility |= Private;
        return visibility;
    }

    private static bool IsPublic(Type type)
    {
        return type.IsVisible
               && type.IsPublic
               && !type.IsNotPublic
               && !type.IsNested
               && !type.IsNestedPublic
               && !type.IsNestedFamily
               && !type.IsNestedPrivate
               && !type.IsNestedAssembly
               && !type.IsNestedFamORAssem
               && !type.IsNestedFamANDAssem;
    }

    private static bool IsInternal(Type type)
    {
        return !type.IsVisible
               && !type.IsPublic
               && type.IsNotPublic
               && !type.IsNested
               && !type.IsNestedPublic
               && !type.IsNestedFamily
               && !type.IsNestedPrivate
               && !type.IsNestedAssembly
               && !type.IsNestedFamORAssem
               && !type.IsNestedFamANDAssem;
    }

    // only nested types can be declared "protected"
    private static bool IsProtected(Type type)
    {
        return !type.IsVisible
               && !type.IsPublic
               && !type.IsNotPublic
               && type.IsNested
               && !type.IsNestedPublic
               && type.IsNestedFamily
               && !type.IsNestedPrivate
               && !type.IsNestedAssembly
               && !type.IsNestedFamORAssem
               && !type.IsNestedFamANDAssem;
    }

    // only nested types can be declared "private"
    private static bool IsPrivate(Type type)
    {
        return !type.IsVisible
               && !type.IsPublic
               && !type.IsNotPublic
               && type.IsNested
               && !type.IsNestedPublic
               && !type.IsNestedFamily
               && type.IsNestedPrivate
               && !type.IsNestedAssembly
               && !type.IsNestedFamORAssem
               && !type.IsNestedFamANDAssem;
    }

    public static bool HasDefaultConstructor(this Type type, [NotNullWhen(true)] out ConstructorInfo? defaultCtor)
    {
        defaultCtor = type.GetConstructor(Instance, Type.EmptyTypes);
        return defaultCtor is not null;
    }
    
    public static object? GetDefault(this Type type)
    {
        if (type.IsClass || type.IsInterface)
            return null;
        return Activator.CreateInstance(type);
    }

    public static object GetUninitialized(this Type type)
    {
        return RuntimeHelpers.GetUninitializedObject(type);
    }
    
    public static bool IsObjectArray(this Type type) => type == typeof(object[]);
    
    public static ParameterAccess GetAccess(this Type type, out Type baseType)
    {
        if (type.IsByRef)
        {
            baseType = type.GetElementType()!;
            return ParameterAccess.Ref;
        }
        baseType = type;
        return ParameterAccess.Default;
    }
    
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
    
    public static bool CanContainNull(this Type? type)
    {
        return type switch
        {
            null => true,
            { IsAbstract: true, IsSealed: true } => false,  // static
            { IsValueType: true } => type.Implements(typeof(Nullable<>)),
            _ => true,
        };
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