using Jay.Utilities;

namespace Jay.Reflection.Extensions;

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
        if (type is null)
            return visibility;
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

#if !NET6_0_OR_GREATER
    public static ConstructorInfo? GetConstructor(
        this Type type,
        BindingFlags bindingFlags,
        params Type[] parameterTypes)
    {
        return type.GetConstructor(
            bindingFlags,
            default,
            parameterTypes,
            default);
    }
#endif

    public static bool HasDefaultConstructor(this Type type, [NotNullWhen(true)] out ConstructorInfo? defaultCtor)
    {
        defaultCtor = type.GetConstructor(
            BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic,
            Type.EmptyTypes);
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
        return Scary.GetUninitializedObject(type);
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
    
    
    /// <summary>
    /// 
    /// </summary>
    /// <param name="type"></param>
    /// <returns></returns>
    /// <see href="https://gist.github.com/Splamy/6c79cfc85a911dc54c5aae96f86c59fe"/>
    public static object? GetDefaultValue(this Type? type)
    {
        if (type is null || type.CanContainNull())
            return null;
        return Scary.GetUninitializedObject(type);
    }
}