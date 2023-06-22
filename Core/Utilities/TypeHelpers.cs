using System.Reflection;
using Jay.Collections;

namespace Jay.Utilities;

public static class TypeHelpers
{
    private static readonly ConcurrentTypeDictionary<bool> _isRefOrContainsRefCache = new();

    public static bool IsReferenceOrContainsReferences<T>()
    {
#if !NETSTANDARD2_0
        return RuntimeHelpers.IsReferenceOrContainsReferences<T>();
#else
        return IsReferenceOrContainsReferences(typeof(T));
#endif
    }

    public static bool IsUnmanaged<T>() => !IsReferenceOrContainsReferences<T>();
    
    public static bool IsReferenceOrContainsReferences(this Type type)
    {
        return _isRefOrContainsRefCache.GetOrAdd(type,
            t =>
            {
                if (!t.IsValueType) return true;
                return t
                    .GetFields(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic)
                    .Select(static field => field.FieldType)
                    .Any(static fieldType => IsReferenceOrContainsReferences(fieldType));
            });
    }

    public static bool IsUnmanaged(this Type type) => !IsReferenceOrContainsReferences(type);

}