using System.Collections;
using Jay.Dumping;

namespace Jay.Comparision;

/// <summary>
/// 
/// </summary>
/// <typeparam name="T"></typeparam>
/// <remarks>
/// All of the methods process values in the same way so that value == values[0] == values.First()
/// </remarks>
public class EnumerableEqualityComparer<T> : IEqualityComparer
{
    protected readonly IEqualityComparer<T>? _valueEqualityComparer;

    public IEqualityComparer<T> ValueEqualityComparer => _valueEqualityComparer ?? EqualityComparer<T>.Default;

    public EnumerableEqualityComparer(IEqualityComparer<T>? valueEqualityComparer = default)
    {
        _valueEqualityComparer = valueEqualityComparer;
    }

    bool IEqualityComparer.Equals(object? x, object? y)
    {
        if (ReferenceEquals(x, y)) return true;
        if (x is null || y is null) return false;
        if (x is T xValue) return Equals(xValue, y);
        if (x is T[] xArray) return Equals(xArray, y);
        if (x is IEnumerable<T> xEnumerable) return Equals(xEnumerable, y);
        return false;
    }

    int IEqualityComparer.GetHashCode(object? obj)
    {
        if (obj is null) return 0;
        if (obj is T value) return GetHashCode(value);
        if (obj is T[] array) return GetHashCode(array);
        if (obj is IEnumerable<T> enumerable) return GetHashCode(enumerable);
        return 0;
    }

#region Equals(T, ?)
    public bool Equals(T? left, object? right)
    {
        if (right is T singleValue) return Equals(left, singleValue);
        if (right is T[] array) return Equals(left, array);
        if (right is IEnumerable<T> enumerable) return Equals(left, enumerable);
        return false;
    }

    public bool Equals(T? left, T? right)
    {
        return ValueEqualityComparer.Equals(left, right);
    }

    public bool Equals(T? left, ReadOnlySpan<T> right)
    {
        return right.Length == 1 && ValueEqualityComparer.Equals(left, right[0]);
    }

    public bool Equals(T? left, T[] right)
    {
        return right.Length == 1 && ValueEqualityComparer.Equals(left, right[0]);
    }

    public bool Equals(T? left, IEnumerable<T> right)
    {
        if (right.TryGetNonEnumeratedCount(out int rightCount))
        {
            if (rightCount != 1) return false;

            if (right is IList<T> rightList)
            {
                return ValueEqualityComparer.Equals(left, rightList[0]);
            }

            using var re = right.GetEnumerator();
            re.MoveNext();
            return ValueEqualityComparer.Equals(left, re.Current);
        }
        else
        {
            using var re = right.GetEnumerator();
            if (!re.MoveNext()) return false;
            var eq = ValueEqualityComparer.Equals(left, re.Current);
            if (re.MoveNext()) return false;
            return eq;
        }
    }
    #endregion

#region Equals(ReadOnlySpan<T>, ?)
    public bool Equals(ReadOnlySpan<T> left, object? right)
    {
        if (right is T singleValue) return Equals(left, singleValue);
        if (right is T[] array) return Equals(left, array);
        if (right is IEnumerable<T> enumerable) return Equals(left, enumerable);
        return false;
    }

    public bool Equals(ReadOnlySpan<T> left, T? right)
    {
        return left.Length == 1 && ValueEqualityComparer.Equals(left[0], right);
    }

    public bool Equals(ReadOnlySpan<T> left, ReadOnlySpan<T> right)
    {
        return MemoryExtensions.SequenceEqual(left, right, _valueEqualityComparer);
    }

    public bool Equals(ReadOnlySpan<T> left, T[] right)
    {
        return MemoryExtensions.SequenceEqual(left, right, _valueEqualityComparer);
    }

    public bool Equals(ReadOnlySpan<T> left, IEnumerable<T> right)
    {
        var veq = ValueEqualityComparer;

        if (right.TryGetNonEnumeratedCount(out int rightCount))
        {
            if (rightCount != left.Length) return false;

            if (right is IList<T> rightList)
            {
                for (var i = 0; i < left.Length; i++)
                {
                    if (!veq.Equals(left[i], rightList[i])) return false;
                }
                return true;
            }

            using var re = right.GetEnumerator();
            for (var i = 0; i < left.Length; i++)
            {
                re.MoveNext();
                if (!veq.Equals(left[i], re.Current)) return false;
            }
            return true;
        }
        else
        {
            using var re = right.GetEnumerator();
            for (var i = 0; i < left.Length; i++)
            {
                if (!re.MoveNext()) return false;
                if (!veq.Equals(left[i], re.Current)) return false;
            }
            if (re.MoveNext()) return false;
            return true;
        }
    }
    #endregion

#region Equals(T[], ?)
    public bool Equals(T[] left, object? right)
    {
        if (right is T singleValue) return Equals(left, singleValue);
        if (right is T[] array) return Equals(left, array);
        if (right is IEnumerable<T> enumerable) return Equals(left, enumerable);
        return false;
    }

    public bool Equals(T[] left, T? right)
    {
        return left.Length == 1 && ValueEqualityComparer.Equals(left[0], right);
    }

    public bool Equals(T[] left, ReadOnlySpan<T> right)
    {
        return MemoryExtensions.SequenceEqual(left, right, _valueEqualityComparer);
    }

    public bool Equals(T[] left, T[] right)
    {
        return MemoryExtensions.SequenceEqual(left, right, _valueEqualityComparer);
    }

    public bool Equals(T[] left, IEnumerable<T> right)
    {
        var veq = ValueEqualityComparer;

        if (right.TryGetNonEnumeratedCount(out int rightCount))
        {
            if (rightCount != left.Length) return false;

            if (right is IList<T> rightList)
            {
                for (var i = 0; i < left.Length; i++)
                {
                    if (!veq.Equals(left[i], rightList[i])) return false;
                }
                return true;
            }

            using var re = right.GetEnumerator();
            for (var i = 0; i < left.Length; i++)
            {
                re.MoveNext();
                if (!veq.Equals(left[i], re.Current)) return false;
            }
            return true;
        }
        else
        {
            using var re = right.GetEnumerator();
            for (var i = 0; i < left.Length; i++)
            {
                if (!re.MoveNext()) return false;
                if (!veq.Equals(left[i], re.Current)) return false;
            }
            if (re.MoveNext()) return false;
            return true;
        }
    }
    #endregion

#region Equals(IEnumerable<T>, ?)
    public bool Equals(IEnumerable<T> left, object? right)
    {
        if (right is T singleValue) return Equals(left, singleValue);
        if (right is T[] array) return Equals(left, array);
        if (right is IEnumerable<T> enumerable) return Equals(left, enumerable);
        return false;
    }

    public bool Equals(IEnumerable<T> left, T? right)
    {
        if (left.TryGetNonEnumeratedCount(out int leftCount))
        {
            if (leftCount != 1) return false;

            if (left is IList<T> leftList)
            {
                return ValueEqualityComparer.Equals(leftList[0], right);
            }

            using var le = left.GetEnumerator();
            le.MoveNext();
            return ValueEqualityComparer.Equals(le.Current, right);
        }
        else
        {
            using var le = left.GetEnumerator();
            if (!le.MoveNext()) return false;
            var eq = ValueEqualityComparer.Equals(le.Current, right);
            if (le.MoveNext()) return false;
            return eq;
        }
    }

    public bool Equals(IEnumerable<T> left, ReadOnlySpan<T> right)
    {
        var veq = ValueEqualityComparer;

        if (left.TryGetNonEnumeratedCount(out int leftCount))
        {
            if (leftCount != right.Length) return false;

            if (left is IList<T> leftList)
            {
                for (var i = 0; i < right.Length; i++)
                {
                    if (!veq.Equals(leftList[i], right[i])) return false;
                }
                return true;
            }

            using var le = left.GetEnumerator();
            for (var i = 0; i < right.Length; i++)
            {
                le.MoveNext();
                if (!veq.Equals(le.Current, right[i])) return false;
            }
            return true;
        }
        else
        {
            using var le = left.GetEnumerator();
            for (var i = 0; i < right.Length; i++)
            {
                if (!le.MoveNext()) return false;
                if (!veq.Equals(le.Current, right[i])) return false;
            }
            if (le.MoveNext()) return false;
            return true;
        }
    }

    public bool Equals(IEnumerable<T> left, T[] right)
    {
        var veq = ValueEqualityComparer;

        if (left.TryGetNonEnumeratedCount(out int leftCount))
        {
            if (leftCount != right.Length) return false;

            if (left is IList<T> leftList)
            {
                for (var i = 0; i < right.Length; i++)
                {
                    if (!veq.Equals(leftList[i], right[i])) return false;
                }
                return true;
            }

            using var le = left.GetEnumerator();
            for (var i = 0; i < right.Length; i++)
            {
                le.MoveNext();
                if (!veq.Equals(le.Current, right[i])) return false;
            }
            return true;
        }
        else
        {
            using var le = left.GetEnumerator();
            for (var i = 0; i < right.Length; i++)
            {
                if (!le.MoveNext()) return false;
                if (!veq.Equals(le.Current, right[i])) return false;
            }
            if (le.MoveNext()) return false;
            return true;
        }
    }

    public bool Equals(IEnumerable<T> left, IEnumerable<T> right)
    {
        var veq = this.ValueEqualityComparer;

        // Can get/trust Left.Count
        if (left.TryGetNonEnumeratedCount(out int leftCount))
        {
            // Can get/trust Right.Count
            if (right.TryGetNonEnumeratedCount(out int rightCount))
            {
                // Fast length compare
                if (leftCount != rightCount) return false;

                // List we get indexer
                if (left is IList<T> leftList)
                {
                    // see above
                    if (right is IList<T> rightList)
                    {
                        for (var i = 0; i < leftCount; i++)
                        {
                            if (!veq.Equals(leftList[i], rightList[i])) return false;
                        }
                        return true;
                    }
                    // Cannot use indexer for right
                    else
                    {
                        using var re = right.GetEnumerator();
                        for (var i = 0; i < leftCount; i++)
                        {
                            re.MoveNext();
                            if (!veq.Equals(leftList[i], re.Current)) return false;
                        }
                        return true;
                    }
                }
                // Cannot use indexer for left
                else
                {
                    // List we get indexer
                    if (right is IList<T> rightList)
                    {
                        using var le = left.GetEnumerator();
                        for (var i = 0; i < leftCount; i++)
                        {
                            le.MoveNext();
                            if (!veq.Equals(le.Current, rightList[i])) return false;
                        }
                        return true;
                    }
                    // Cannot use indexer for right
                    else
                    {
                        using var le = left.GetEnumerator();
                        using var re = right.GetEnumerator();
                        for (var i = 0; i < leftCount; i++)
                        {
                            le.MoveNext();
                            re.MoveNext();
                            if (!veq.Equals(le.Current, re.Current)) return false;
                        }
                        return true;
                    }
                }
            }
            // Cannot trust Right.Count
            else
            {
                // List we get indexer
                if (left is IList<T> leftList)
                {
                    using var re = right.GetEnumerator();
                    for (var i = 0; i < leftCount; i++)
                    {
                        // Not enough items
                        if (!re.MoveNext()) return false;
                        if (!veq.Equals(leftList[i], re.Current)) return false;
                    }
                    // Too many items
                    if (re.MoveNext()) return false; 
                    return true;
                }
                // Cannot use indexer for left
                else
                {
                    using var le = left.GetEnumerator();
                    using var re = right.GetEnumerator();
                    for (var i = 0; i < leftCount; i++)
                    {
                        le.MoveNext();
                        // Not enough items
                        if (!re.MoveNext()) return false;
                        if (!veq.Equals(le.Current, re.Current)) return false;
                    }
                    // Too many items
                    if (re.MoveNext()) return false;
                    return true;
                }
            }
        }
        // Cannot trust Left.Count
        else
        {
            
        }

        if (left is ICollection<T> leftCollection)
        {
            if (leftCollection is IList<T> leftList)
            {
                if (right is ICollection<T> rightCollection)
                {
                    if (leftCollection.Count != rightCollection.Count) return false;
                    leftCollection.TryGetNonEnumeratedCount()
                }


                if (rightIsList)
                {
                    for (var i = 0; i < leftList.Count; i++)
                    {
                        if (!veq.Equals(leftList[i], rightList![i])) return false;
                    }
                    return true;
                }

                using var re = right.GetEnumerator();
                for (var i = 0; i < leftList.Count; i++)
                {
                    re.MoveNext();
                    if (!veq.Equals(leftList[i], re.Current)) return false;
                }
                return true;
            }

            if (rightIsList)
            {
                using var le = leftCollection.GetEnumerator();
                for (var i = 0; i < rightList!.Count; i++)
                {
                    le.MoveNext();
                    if (!veq.Equals(le.Current, rightList![i])) return false;
                }
                return true;
            }

        }

        using (var leftEnumerator = left.GetEnumerator())
        using (var rightEnumerator = right.GetEnumerator())
        {
            var leftMoved = leftEnumerator.MoveNext();
            var rightMoved = rightEnumerator.MoveNext();
            while (leftMoved && rightMoved)
            {
                if (!veq.Equals(leftEnumerator.Current, rightEnumerator.Current))
                    return false;
                leftMoved = leftEnumerator.MoveNext();
                rightMoved = rightEnumerator.MoveNext();
            }
            return leftMoved == rightMoved;
        }

    }
#endregion

    public int GetHashCode(T? value)
    {
        var hasher = new Hasher();
        hasher.Add<T>(value, _valueEqualityComparer);
        return hasher.ToHashCode();
    }

    public int GetHashCode(ReadOnlySpan<T> values)
    {
        var hasher = new Hasher();
        for (var i = 0; i < values.Length; i++)
        {
            hasher.Add<T>(values[i], _valueEqualityComparer);
        }
        return hasher.ToHashCode();
    }

    public int GetHashCode(params T[] values)
    {
        var hasher = new Hasher();
        for (var i = 0; i < values.Length; i++)
        {
            hasher.Add<T>(values[i], _valueEqualityComparer);
        }
        return hasher.ToHashCode();
    }

    public int GetHashCode(IEnumerable<T> values)
    {
        var hasher = new Hasher();
        foreach (var value in values)
        {
            hasher.Add<T>(value, _valueEqualityComparer);
        }
        return hasher.ToHashCode();
    }
}






/*public sealed class EnumerableEqualityComparer<T> : IEqualityComparer<T[]>,
                                                    IEqualityComparer<IEnumerable<T>>,
                                                    IEqualityComparer<T>,
                                                    IEqualityComparer
{
    public static EnumerableEqualityComparer<T> Default { get; } = new EnumerableEqualityComparer<T>(null);

    private readonly EqualityComparer<T>? _equalityComparer;

    public EnumerableEqualityComparer(EqualityComparer<T>? equalityComparer = null)
    {
        _equalityComparer = equalityComparer;
    }

    bool IEqualityComparer.Equals(object? x, object? y)
    {
        if (ReferenceEquals(x, y)) return true;
        if (x is null || y is null) return false;
        if (x is T xValue)
        {
            return y is T yValue && Equals(xValue, yValue);
        }
        else if (x is T[] xArray)
        {
            if (y is T[] yArray)
                return Equals(xArray, yArray);
            else if (y is IEnumerable<T> yEnumerable)
                return Equals(xArray, yEnumerable);
        }
    }

    public bool Equals(T? x, T? y)
    {
        if (_equalityComparer is null)
        {
            return EqualityComparer<T>.Default.Equals(x, y);
        }
        return _equalityComparer.Equals(x, y);
    }

    public bool Equals(T? value, object? obj)
    {
        if (obj is T typed) return Equals(value, typed);
        if (obj is )
    }

    public bool Equals(ReadOnlySpan<T> x, ReadOnlySpan<T> y)
    {
        if (_equalityComparer is null)
        {
            return MemoryExtensions.SequenceEqual(x, y);
        }
        if (x.Length != y.Length) return false;
        for (int i = 0; i < x.Length; i++)
        {
            if (!_equalityComparer.Equals(x[i], y[i]))
                return false;
        }
        return true;
    }

    public bool Equals(T[]? x, T[]? y)
    {
        if (ReferenceEquals(x, y)) return true;
        if (x is null || y is null) return false;
        if (_equalityComparer is null)
        {
            return MemoryExtensions.SequenceEqual<T>(x, y);
        }
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
        var comparer = _equalityComparer ?? EqualityComparer<T>.Default;
        using (var xe = x.GetEnumerator())
        using (var ye = y.GetEnumerator())
        {
            var xMoved = xe.MoveNext();
            var yMoved = ye.MoveNext();
            while (xMoved && yMoved)
            {
                if (!comparer.Equals(xe.Current, ye.Current))
                    return false;
                xMoved = xe.MoveNext();
                yMoved = ye.MoveNext();
            }
            return xMoved == yMoved;
        }
    }


    public int GetHashCode(T? value)
    {
        if (value is null) return 0;
        if (_equalityComparer is null) return value.GetHashCode();
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

    public int GetHashCode(ReadOnlySpan<T> values)
    {
        var hashCode = new HashCode();
        for (var i = 0; i < values.Length; i++)
        {
            hashCode.Add(values[i], _equalityComparer);
        }
        return hashCode.ToHashCode();
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
}*/