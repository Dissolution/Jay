using System.Runtime.InteropServices;
using Jay.Collections;

namespace Jay.Reflection.Utilities;

public sealed class TypeHelper
{
    private static readonly ConcurrentTypeMap<TypeHelper> _cache = new();

    private static TypeHelper GetTypeHelper(Type type)
    {
        return _cache.GetOrAdd(type, static t => new TypeHelper(t));
    }

    public static bool IsReferenceOrContainsReferences<T>() => GetTypeHelper(typeof(T))._isReferenceOrContainsReferences;
    public static bool IsReferenceOrContainsReferences(Type type) => GetTypeHelper(type)._isReferenceOrContainsReferences;

    public static int Size<T>() => GetTypeHelper(typeof(T))._size;

    public static int Size(Type type) => GetTypeHelper(type)._size;
    
    private readonly Type _type;
    private readonly bool _isReferenceOrContainsReferences;
    private readonly int _size;

    private TypeHelper(Type type)
    {
        _type = type;
        if (!type.IsValueType)
        {
            _isReferenceOrContainsReferences = true;
        }
        else
        {
            _isReferenceOrContainsReferences = false;
            var fields = type.GetFields(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
            foreach (var field in fields)
            {
                var fieldTypeHelper = GetTypeHelper(field.FieldType);
                if (fieldTypeHelper._isReferenceOrContainsReferences)
                {
                    _isReferenceOrContainsReferences = true;
                    break;
                }
            }
        }

        _size = Marshal.SizeOf(type);
    }
}