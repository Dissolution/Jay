using Jay.Collections;
using Jay.Validation;

namespace Jay.Reflection;

public static partial class TypeCache
{
    public static object? Default(Type? type)
    {
        if (type is null) 
            return null;
        if (type.IsClass || type.IsInterface)
            return null;
        return Activator.CreateInstance(type);
    }

    /// <summary>
    /// Create a new, uninitialized <see cref="object"/> of the given <paramref name="type"/>
    /// </summary>
    [return: NotNullIfNotNull("type")]
    public static object? Raw(Type? type)
    {
        if (type is null) return null;
        return RuntimeHelpers.GetUninitializedObject(type);
    }

    internal static readonly ConcurrentTypeDictionary<Func<object?, object?, bool>> _typeEqualsCache = new();

    private static Func<object?, object?, bool> CreateEquals(Type type)
    {
        return (Delegate.CreateDelegate(typeof(Func<object?, object?, bool>),
            typeof(TypeCache<>).MakeGenericType(type)
                .GetMethod("Equals",
                    Reflect.StaticFlags,
                    new Type[2] { typeof(object), typeof(object) })
                .ThrowIfNull()) as Func<object?, object?, bool>)!;
    }
    
    public static bool Equals(Type? type, object? left, object? right)
    {
        if (type is null) return false;
        var equals = _typeEqualsCache.GetOrAdd(type, t => CreateEquals(t));
        return equals(left, right);
    }
}

public static partial class TypeCache<T>
{
    public static T? Default() => default(T);
    
    /// <summary>
    /// Create a new, uninitialized <typeparamref name="T"/> value
    /// </summary>
    public static T Raw()
    {
        return (T)RuntimeHelpers.GetUninitializedObject(typeof(T));
    }

    public static bool Equals(T? left, T? right)
    {
        return EqualityComparer<T>.Default.Equals(left, right);
    }

    new public static bool Equals(object? left, object? right)
    {
        if (left.CanBe<T>(out T? leftT) && right.CanBe<T>(out T? rightT))
        {
            return EqualityComparer<T>.Default.Equals(leftT, rightT);
        }
        return false;
    }
}
