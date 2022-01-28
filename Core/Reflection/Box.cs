using System.Collections;
using System.Diagnostics.CodeAnalysis;
using System.Reflection;
using System.Runtime.CompilerServices;
using Jay.Collections;
using Jay.Reflection;
using Jay.Validation;

namespace Jay;

public readonly struct Box : IEquatable<Box>, IComparable<Box>, IFormattable
{
    public static bool operator ==(Box a, Box b) => a.Equals(b);
    public static bool operator !=(Box a, Box b) => !a.Equals(b);
    public static bool operator <(Box a, Box b) => a.CompareTo(b) < 0;
    public static bool operator >(Box a, Box b) => a.CompareTo(b) > 0;
    public static bool operator <=(Box a, Box b) => a.CompareTo(b) <= 0;
    public static bool operator >=(Box a, Box b) => a.CompareTo(b) >= 0;

   

    private static readonly ConcurrentTypeDictionary<IEqualityComparer> _equalityComparers;
    private static readonly ConcurrentTypeDictionary<IComparer> _comparers;

    static Box()
    {
        _equalityComparers = new();
        _comparers = new();
    }

    private static IEqualityComparer GetEqualityComparer(Type type)
    {
        return _equalityComparers.GetOrAdd(type, t => typeof(EqualityComparer<>).MakeGenericType(t)
                                               .GetProperty(nameof(EqualityComparer<byte>.Default),
                                                            BindingFlags.Public | BindingFlags.Static)
                                               .ThrowIfNull()
                                               .GetStaticValue<IEqualityComparer>()
                                               .ThrowIfNull());
    }

    private static IComparer GetComparer(Type type)
    {
        return _comparers.GetOrAdd(type, t => typeof(Comparer<>).MakeGenericType(t)
                                                                .GetProperty(nameof(Comparer<byte>.Default),
                                                                             BindingFlags.Public | BindingFlags.Static)
                                                                .ThrowIfNull()
                                                                .GetStaticValue<IComparer>()
                                                                .ThrowIfNull());
    }

    public static Box Wrap(object? value)
    {
        return new Box(value, value?.GetType());
    }

    public static Box Wrap<T>(T? value)
    {
        return new Box(value, typeof(T));
    }

    private readonly object? _obj;
    private readonly Type? _objType;

    public bool IsNull
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        get => _objType == null;
    }

    private Box(object? obj, Type? type)
    {
        _obj = obj;
        _objType = type;
    }

    public bool Is<T>()
    {
        return _objType == typeof(T);
    }

    public bool Is<T>([MaybeNullWhen(false)] out T value)
    {
        return _obj.Is<T>(out value);
    }

    public int CompareTo(Box box)
    {
        if (IsNull)
        {
            if (box.IsNull) return 0;
            return -1;
        }
        if (box.IsNull) return 1;
        if (box._objType != _objType)
            return 0;
        return GetComparer(_objType!).Compare(_obj, box._obj);
    }

    public bool Equals(Box box)
    {
        if (IsNull) return box.IsNull;
        return box._objType == _objType &&
               GetEqualityComparer(_objType!).Equals(box._obj, _obj);
    }

    public override bool Equals(object? obj)
    {
        if (obj is null) return IsNull;
        var objType = obj.GetType();
        return objType == _objType &&
               GetEqualityComparer(_objType!).Equals(obj, _obj);
    }

    public override int GetHashCode()
    {
        if (_objType is null || _obj is null) return 0;
        return GetEqualityComparer(_objType).GetHashCode(_obj);
    }

    public string ToString(string? format, IFormatProvider? formatProvider)
    {
        if (_obj is IFormattable formattable)
        {
            return formattable.ToString(format, formatProvider);
        }
        return _obj?.ToString() ?? "";
    }

    public override string ToString()
    {
        return _obj?.ToString() ?? "";
    }
}