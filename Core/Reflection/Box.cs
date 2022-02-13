using System.Collections;
using System.Diagnostics.CodeAnalysis;
using System.Reflection;
using System.Runtime.CompilerServices;
using Jay.Collections;
using Jay.Reflection;
using Jay.Reflection.Cloning;
using Jay.Validation;

namespace Jay;

public readonly struct Box : IEquatable<Box>, 
                             IComparable<Box>, IComparable,
                             ICloneable,
                             IFormattable
{
    public static bool operator ==(Box a, Box b) => a.Equals(b);
    public static bool operator !=(Box a, Box b) => !a.Equals(b);
    public static bool operator <(Box a, Box b) => a.CompareTo(b) < 0;
    public static bool operator >(Box a, Box b) => a.CompareTo(b) > 0;
    public static bool operator <=(Box a, Box b) => a.CompareTo(b) <= 0;
    public static bool operator >=(Box a, Box b) => a.CompareTo(b) >= 0;

    public static bool operator ==(Box box, object? obj) => box.Equals(obj);
    public static bool operator !=(Box box, object? obj) => !box.Equals(obj);
    public static bool operator ==(object? obj, Box box) => box.Equals(obj);
    public static bool operator !=(object? obj, Box box) => !box.Equals(obj);
    public static bool operator <(Box box, object? obj) => box.CompareTo(obj) < 0;
    public static bool operator >(Box box, object? obj) => box.CompareTo(obj) > 0;
    public static bool operator <(object? obj, Box box) => box.CompareTo(obj) > 0;
    public static bool operator >(object? obj, Box box) => box.CompareTo(obj) < 0;
    public static bool operator <=(Box box, object? obj) => box.CompareTo(obj) <= 0;
    public static bool operator >=(Box box, object? obj) => box.CompareTo(obj) >= 0;
    public static bool operator <=(object? obj, Box box) => box.CompareTo(obj) >= 0;
    public static bool operator >=(object? obj, Box box) => box.CompareTo(obj) <= 0;

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
                                               .ThrowIfNull($"Cannot find the EqualityComparer<{t}>.Default property")
                                               .GetStaticValue<IEqualityComparer>()
                                               .ThrowIfNull($"Cannot cast EqualityComparer<{t}> to IEqualityComparer"));
    }

    private static IComparer GetComparer(Type type)
    {
        return _comparers.GetOrAdd(type, t => typeof(Comparer<>).MakeGenericType(t)
                                                                .GetProperty(nameof(Comparer<byte>.Default),
                                                                             BindingFlags.Public | BindingFlags.Static)
                                                                .ThrowIfNull($"Cannot find the Comparer<{t}>.Default property")
                                                                .GetStaticValue<IComparer>()
                                                                .ThrowIfNull($"Cannot cast Comparer<{t}> to IComparer"));
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


    public bool ContainsNull
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        get => _obj is null;
    }

    private Box(object? obj, Type? type)
    {
        _obj = obj;
        _objType = type;
    }

    // TODO: Should Is<> act like CanBe<>?
    // In the instance of Wrap((int?)null), the objecttype is int?, but we should be able to extract the int inside?

    public bool Is<T>()
    {
        return _obj is T;
    }

    public bool Is<T>([NotNullWhen(true)] out T? value)
    {
        if (_obj is T)
        {
            value = (T)_obj;
            return true;
        }
        value = default;
        return false;
    }

    public bool IsType(Type? type)
    {
        return _objType == type;
    }

    public object? AsObject() => _obj;

    public object Clone()
    {
        //return new Box(Cloner.Clone(in _obj), _objType);
        throw new InvalidOperationException();
    }

    public int CompareTo(Box box)
    {
        if (ContainsNull)
        {
            if (box.ContainsNull) return 0;
            return -1;
        }
        if (box.ContainsNull) return 1;
        if (box._objType != _objType)
            return 0;
        return GetComparer(_objType!).Compare(_obj, box._obj);
    }

    public int CompareTo(object? obj)
    {
        return CompareTo(Wrap(obj));
    }

    public bool Equals(Box box)
    {
        if (ContainsNull) return box.ContainsNull;
        return box._objType == _objType &&
               GetEqualityComparer(_objType!).Equals(box._obj, _obj);
    }

    public override bool Equals(object? obj)
    {
        return Equals(Wrap(obj));
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