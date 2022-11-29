using Jay.Utilities;

namespace Jay.Comparision;

public sealed class EnumerableEqualityComparer<T> : IEqualityComparer<T>,
                                                    IEqualityComparer<T[]>,
                                                    IEqualityComparer<IEnumerable<T>>
{
    public static EnumerableEqualityComparer<T> Default { get; } = new EnumerableEqualityComparer<T>();

    private readonly IEqualityComparer<T> _equalityComparer;

    public EnumerableEqualityComparer(IEqualityComparer<T>? equalityComparer = default)
    {
        _equalityComparer = equalityComparer ?? EqualityComparer<T>.Default;
    }

    public bool Equals(T? left, T? right)
    {
        return _equalityComparer.Equals(left, right);
    }

    public bool Equals(T? left, ReadOnlySpan<T> right)
    {
        return right.Length == 1 && _equalityComparer.Equals(left, right[0]);
    }

    public bool Equals(T? left, T[]? right)
    {
        if (right is null) return left is null;
        return right.Length == 1 && _equalityComparer.Equals(left, right[0]);
    }

    public bool Equals(T? left, IEnumerable<T>? right)
    {
        if (right is null) return left is null;
        if (right.TryGetNonEnumeratedCount(out int count))
        {
            if (count != 1) return false;
            if (right is IList<T> rightList)
            {
                return _equalityComparer.Equals(left, rightList[0]);
            }

            using var rightEnumerator = right.GetEnumerator();
            rightEnumerator.MoveNext();
            return _equalityComparer.Equals(left, rightEnumerator.Current);
        }
        else
        {
            using var rightEnumerator = right.GetEnumerator();
            if (!rightEnumerator.MoveNext()) return false;
            var equal = _equalityComparer.Equals(left, rightEnumerator.Current);
            if (rightEnumerator.MoveNext()) return false;
            return equal;
        }
    }

    public bool Equals(ReadOnlySpan<T> left, T? right)
    {
        return left.Length == 1 && _equalityComparer.Equals(left[0], right);
    }

    public bool Equals(ReadOnlySpan<T> left, ReadOnlySpan<T> right)
    {
        return MemoryExtensions.SequenceEqual(left, right, _equalityComparer);
    }

    public bool Equals(ReadOnlySpan<T> left, T[]? right)
    {
        return MemoryExtensions.SequenceEqual(left, right, _equalityComparer);
    }

    public bool Equals(ReadOnlySpan<T> left, IEnumerable<T>? right)
    {
        if (right is null) return false;
        Func<T?, T?, bool> equals = _equalityComparer.Equals;
        if (right.TryGetNonEnumeratedCount(out int count))
        {
            if (count != left.Length) return false;
            if (right is IList<T> rightList)
            {
                for (var i = 0; i < count; i++)
                {
                    if (!equals(left[i], rightList[i])) return false;
                }

                return true;
            }
            else
            {
                using var rightEnumerator = right.GetEnumerator();
                for (var i = 0; i < count; i++)
                {
                    rightEnumerator.MoveNext();
                    if (!equals(left[i], rightEnumerator.Current)) return false;
                }

                return true;
            }
        }
        else
        {
            using var rightEnumerator = right.GetEnumerator();
            for (var i = 0; i < left.Length; i++)
            {
                if (!rightEnumerator.MoveNext()) return false;
                if (!equals(left[i], rightEnumerator.Current)) return false;
            }

            if (rightEnumerator.MoveNext()) return false;
            return true;
        }
    }

    public bool Equals(T[]? left, T? right)
    {
        if (left is null) return right is null;
        return left.Length == 1 && _equalityComparer.Equals(left[0], right);
    }

    public bool Equals(T[]? left, ReadOnlySpan<T> right)
    {
        return MemoryExtensions.SequenceEqual(left, right, _equalityComparer);
    }

    public bool Equals(T[]? left, T[]? right)
    {
        return MemoryExtensions.SequenceEqual(left, right, _equalityComparer);
    }

    public bool Equals(T[]? left, IEnumerable<T>? right)
    {
        if (left is null) return right is null;
        if (right is null) return false;
        Func<T?, T?, bool> equals = _equalityComparer.Equals;
        if (right.TryGetNonEnumeratedCount(out int count))
        {
            if (count != left.Length) return false;
            if (right is IList<T> rightList)
            {
                for (var i = 0; i < count; i++)
                {
                    if (!equals(left[i], rightList[i])) return false;
                }

                return true;
            }
            else
            {
                using var rightEnumerator = right.GetEnumerator();
                for (var i = 0; i < count; i++)
                {
                    rightEnumerator.MoveNext();
                    if (!equals(left[i], rightEnumerator.Current)) return false;
                }

                return true;
            }
        }
        else
        {
            using var rightEnumerator = right.GetEnumerator();
            for (var i = 0; i < left.Length; i++)
            {
                if (!rightEnumerator.MoveNext()) return false;
                if (!equals(left[i], rightEnumerator.Current)) return false;
            }

            if (rightEnumerator.MoveNext()) return false;
            return true;
        }
    }

    public bool Equals(IEnumerable<T>? left, T? right)
    {
        if (left is null) return right is null;
        if (left.TryGetNonEnumeratedCount(out int count))
        {
            if (count != 1) return false;
            if (left is IList<T> leftList)
            {
                return _equalityComparer.Equals(leftList[0], right);
            }

            using var leftEnumerator = left.GetEnumerator();
            leftEnumerator.MoveNext();
            return _equalityComparer.Equals(leftEnumerator.Current, right);
        }
        else
        {
            using var leftEnumerator = left.GetEnumerator();
            if (!leftEnumerator.MoveNext()) return false;
            var equal = _equalityComparer.Equals(leftEnumerator.Current, right);
            if (leftEnumerator.MoveNext()) return false;
            return equal;
        }
    }

    public bool Equals(IEnumerable<T>? left, ReadOnlySpan<T> right)
    {
        if (left is null) return false;
        Func<T?, T?, bool> equals = _equalityComparer.Equals;
        if (left.TryGetNonEnumeratedCount(out int count))
        {
            if (count != right.Length) return false;
            if (left is IList<T> leftList)
            {
                for (var i = 0; i < count; i++)
                {
                    if (!equals(leftList[i], right[i])) return false;
                }

                return true;
            }
            else
            {
                using var leftEnumerator = left.GetEnumerator();
                for (var i = 0; i < count; i++)
                {
                    leftEnumerator.MoveNext();
                    if (!equals(leftEnumerator.Current, right[i])) return false;
                }

                return true;
            }
        }
        else
        {
            using var leftEnumerator = left.GetEnumerator();
            for (var i = 0; i < right.Length; i++)
            {
                if (!leftEnumerator.MoveNext()) return false;
                if (!equals(leftEnumerator.Current, right[i])) return false;
            }

            if (leftEnumerator.MoveNext()) return false;
            return true;
        }
    }

    public bool Equals(IEnumerable<T>? left, T[]? right)
    {
        if (left is null) return right is null;
        if (right is null) return false;
        Func<T?, T?, bool> equals = _equalityComparer.Equals;
        if (left.TryGetNonEnumeratedCount(out int count))
        {
            if (count != right.Length) return false;
            if (left is IList<T> leftList)
            {
                for (var i = 0; i < count; i++)
                {
                    if (!equals(leftList[i], right[i])) return false;
                }

                return true;
            }
            else
            {
                using var leftEnumerator = left.GetEnumerator();
                for (var i = 0; i < count; i++)
                {
                    leftEnumerator.MoveNext();
                    if (!equals(leftEnumerator.Current, right[i])) return false;
                }

                return true;
            }
        }
        else
        {
            using var leftEnumerator = left.GetEnumerator();
            for (var i = 0; i < right.Length; i++)
            {
                if (!leftEnumerator.MoveNext()) return false;
                if (!equals(leftEnumerator.Current, right[i])) return false;
            }

            if (leftEnumerator.MoveNext()) return false;
            return true;
        }
    }

    public bool Equals(IEnumerable<T>? left, IEnumerable<T>? right)
    {
        if (ReferenceEquals(left, right)) return true;
        if (left is null || right is null) return false;
        Func<T?, T?, bool> equals = _equalityComparer.Equals;
        // If this succeeds, we do not have to check any left.MoveNext() operations
        if (left.TryGetNonEnumeratedCount(out int leftCount))
        {
            // Can avoid .GetEnumerator()?
            if (left is IList<T> leftList)
            {
                // If this succeeds, we do not have to check any right.MoveNext() operations
                if (right.TryGetNonEnumeratedCount(out int rightCount))
                {
                    // We can immediately check lengths
                    if (leftCount != rightCount) return false;

                    // Can avoid right.GetEnumerator()?
                    if (right is IList<T> rightList)
                    {
                        for (var i = 0; i < leftCount; i++)
                        {
                            if (!equals(leftList[i], rightList[i])) return false;
                        }

                        return true;
                    }
                    // We have to right.GetEnumerator, but we trust its length
                    else
                    {
                        using var rightEnumerator = right.GetEnumerator();
                        for (var i = 0; i < leftCount; i++)
                        {
                            rightEnumerator.MoveNext();
                            if (!equals(leftList[i], rightEnumerator.Current)) return false;
                        }

                        return true;
                    }
                }
                // We cannot count right
                else
                {
                    using var rightEnumerator = right.GetEnumerator();
                    for (var i = 0; i < leftCount; i++)
                    {
                        if (!rightEnumerator.MoveNext()) return false;
                        if (!equals(leftList[i], rightEnumerator.Current)) return false;
                    }

                    if (rightEnumerator.MoveNext()) return false;
                    return true;
                }
            }
            // We have to left.GetEnumerator, but we trust its length
            else
            {
                using var leftEnumerator = left.GetEnumerator();

                // If this succeeds, we do not have to check any right.MoveNext() operations
                if (right.TryGetNonEnumeratedCount(out int rightCount))
                {
                    // We can immediately check lengths
                    if (leftCount != rightCount) return false;

                    // Can avoid right.GetEnumerator()?
                    if (right is IList<T> rightList)
                    {
                        for (var i = 0; i < leftCount; i++)
                        {
                            leftEnumerator.MoveNext();
                            if (!equals(leftEnumerator.Current, rightList[i])) return false;
                        }

                        return true;
                    }
                    // We have to right.GetEnumerator, but we trust its length
                    else
                    {
                        using var rightEnumerator = right.GetEnumerator();
                        for (var i = 0; i < leftCount; i++)
                        {
                            leftEnumerator.MoveNext();
                            rightEnumerator.MoveNext();
                            if (!equals(leftEnumerator.Current, rightEnumerator.Current)) return false;
                        }

                        return true;
                    }
                }
                // We cannot count right
                else
                {
                    using var rightEnumerator = right.GetEnumerator();
                    for (var i = 0; i < leftCount; i++)
                    {
                        leftEnumerator.MoveNext();
                        if (!rightEnumerator.MoveNext()) return false;
                        if (!equals(leftEnumerator.Current, rightEnumerator.Current)) return false;
                    }

                    if (rightEnumerator.MoveNext()) return false;
                    return true;
                }
            }
        }
        // We cannot count left
        else
        {
            using var leftEnumerator = left.GetEnumerator();
            // If this succeeds, we do not have to check any right.MoveNext() operations
            if (right.TryGetNonEnumeratedCount(out int rightCount))
            {
                // Can avoid right.GetEnumerator()?
                if (right is IList<T> rightList)
                {
                    for (var i = 0; i < leftCount; i++)
                    {
                        if (!leftEnumerator.MoveNext()) return false;
                        if (!equals(leftEnumerator.Current, rightList[i])) return false;
                    }

                    if (leftEnumerator.MoveNext()) return false;
                    return true;
                }
                // We have to right.GetEnumerator, but we trust its length
                else
                {
                    using var rightEnumerator = right.GetEnumerator();
                    for (var i = 0; i < leftCount; i++)
                    {
                        if (!leftEnumerator.MoveNext()) return false;
                        rightEnumerator.MoveNext();
                        if (!equals(leftEnumerator.Current, rightEnumerator.Current)) return false;
                    }

                    if (leftEnumerator.MoveNext()) return false;
                    return true;
                }
            }
            // We cannot count right
            else
            {
                using var rightEnumerator = right.GetEnumerator();
                for (var i = 0; i < leftCount; i++)
                {
                    if (!leftEnumerator.MoveNext()) return false;
                    if (!rightEnumerator.MoveNext()) return false;
                    if (!equals(leftEnumerator.Current, rightEnumerator.Current)) return false;
                }

                if (leftEnumerator.MoveNext()) return false;
                if (rightEnumerator.MoveNext()) return false;
                return true;
            }
        }
    }

    public int GetHashCode(T? value)
    {
        return Hasher.Generate(value, _equalityComparer);
    }

    public int GetHashCode(ReadOnlySpan<T> values)
    {
        return Hasher.Generate<T>(values, _equalityComparer);
    }

    public int GetHashCode(T[]? values)
    {
        return Hasher.Generate<T>(values, _equalityComparer);
    }

    public int GetHashCode(IEnumerable<T>? values)
    {
        return Hasher.Generate<T>(values, _equalityComparer);
    }
}