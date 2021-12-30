﻿using System.Collections;

namespace Jay.Comparision;

public sealed class EnumerableEqualityComparer<T> : IEqualityComparer<T[]>,
                                                    IEqualityComparer<IEnumerable<T>>,
                                                    IEqualityComparer<T>,
                                                    IEqualityComparer
{
    public static EnumerableEqualityComparer<T> Default { get; } = new EnumerableEqualityComparer<T>(EqualityComparer<T>.Default);

    private readonly EqualityComparer<T> _equalityComparer;

    public EnumerableEqualityComparer(EqualityComparer<T> equalityComparer)
    {
        _equalityComparer = equalityComparer ?? EqualityComparer<T>.Default;
    }


    public bool Equals(T[]? x, T[]? y)
    {
        if (ReferenceEquals(x, y)) return true;
        if (x is null || y is null) return false;
        if (x.Length != y.Length) return false;
        for (int i = 0; i < x.Length; i++)
        {
            if (!_equalityComparer.Equals(x[i], y[i]))
                return false;
        }
        return true;
    }
        
    public bool Equals(IEnumerable<T>? x, IEnumerable<T>? y)
    {
        if (x is null)
            return y is null;
        if (y is null) return false;
        using (var xe = x.GetEnumerator())
        using (var ye = y.GetEnumerator())
        {
            var xMoved = xe.MoveNext();
            var yMoved = ye.MoveNext();
            while (xMoved && yMoved)
            {
                if (!_equalityComparer.Equals(xe.Current, ye.Current))
                    return false;
                xMoved = xe.MoveNext();
                yMoved = ye.MoveNext();
            }
            return xMoved == yMoved;
        }
    }
        
    public bool Equals(T? x, T? y)
    {
        return _equalityComparer.Equals(x, y);
    }

    bool IEqualityComparer.Equals(object? x, object? y)
    {
        if (x.CanBe<T>(out var xValue))
        {
            return y.CanBe<T>(out var yValue) && _equalityComparer.Equals(xValue, yValue);
        }

        if (x.CanBe<T[]>(out var xArray))
        {
            return y.CanBe<T[]>(out var yArray) && Equals(xArray, yArray);
        }

        if (x.CanBe<IEnumerable<T>>(out var xEnumerable))
        {
            return y.CanBe<IEnumerable<T>>(out var yEnumerable) && Equals(xEnumerable, yEnumerable);
        }

        return false;
    }

    public int GetHashCode(T[]? values)
    {
        if (values is null) return 0;
        var hashCode = new HashCode();
        for (var i = 0; i < values.Length; i++)
        {
            hashCode.Add(values[i], _equalityComparer);
        }
        return hashCode.ToHashCode();
    }

    public int GetHashCode(IEnumerable<T>? values)
    {
        if (values is null) return 0;
        var hashCode = new HashCode();
        foreach (var value in values)
        {
            hashCode.Add(value, _equalityComparer);
        }
        return hashCode.ToHashCode();
    }

    public int GetHashCode(T? value)
    {
        if (value is null) return 0;
        return _equalityComparer.GetHashCode(value);
    }

    int IEqualityComparer.GetHashCode(object? obj)
    {
        if (obj is null) return 0;
        if (obj is T value) return GetHashCode(value);
        if (obj is T[] array) return GetHashCode(array);
        if (obj is IEnumerable<T> values) return GetHashCode(values);
        return obj.GetHashCode();
    }
}