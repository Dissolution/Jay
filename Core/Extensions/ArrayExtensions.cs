namespace Jay;

public static class ArrayExtensions
{
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool IsNullOrEmpty<T>([NotNullWhen(false)] this T[]? array)
    {
        return array is null || array.Length == 0;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool IsNullOrEmpty<T>([NotNullWhen(false)] this T?[]? array,
        out int length)
    {
        if (array is not null)
        {
            length = array.Length;
            return length > 0;
        }

        length = 0;
        return false;
    }

    public static bool Any<T>(this T[] array, Func<T, bool> predicate)
    {
        for (var i = 0; i < array.Length; i++)
        {
            if (predicate(array[i]))
                return true;
        }

        return false;
    }

    public static bool All<T>(this T[] array, Func<T, bool> predicate)
    {
        for (var i = 0; i < array.Length; i++)
        {
            if (!predicate(array[i]))
                return false;
        }

        return true;
    }

    public static bool Contains<T>(this T?[]? array, T? value)
    {
        if (array is null) return false;
        for (var i = 0; i < array.Length; i++)
        {
            if (EqualityComparer<T>.Default.Equals(array[i], value))
                return true;
        }

        return false;
    }

    public static bool Contains<T>(this T?[]? array, T? value, IEqualityComparer<T>? comparer)
    {
        if (array is null) return false;
        if (comparer is null) return Contains(array, value);
        for (var i = 0; i < array.Length; i++)
        {
            if (comparer.Equals(array[i], value))
                return true;
        }

        return false;
    }

    [return: NotNullIfNotNull("default")]
    public static T? GetOrDefault<T>(this T[]? array, int index, T? @default = default(T))
    {
        if (array is null)
            return @default;
        if ((uint)index >= (uint)array.Length)
            return @default;
        return array[index];
    }

    public static bool TryGetItem<T>(this T[]? array, int index, [MaybeNullWhen(false)] out T item)
    {
        if (array is null || (uint)index >= (uint)array.Length)
        {
            item = default;
            return false;
        }

        item = array[index];
        return true;
    }

    internal sealed class ArrayEnumerator<T> : IEnumerator<T>, IEnumerator,
                                               IDisposable
    {
        private readonly IEnumerator _arrayEnumerator;

        public T Current => (T)_arrayEnumerator.Current!;

        object IEnumerator.Current => _arrayEnumerator.Current!;

        internal ArrayEnumerator(Array array)
        {
            _arrayEnumerator = array.GetEnumerator();
        }

        public bool MoveNext()
        {
            return _arrayEnumerator.MoveNext();
        }

        public void Reset()
        {
            _arrayEnumerator.Reset();
        }

        public void Dispose()
        {
            Result.Dispose(_arrayEnumerator);
        }
    }

    public static IEnumerator<T> GetEnumerator<T>(this T[] array) => new ArrayEnumerator<T>(array);


    public static int IndexOf<T>(this T[] array, T value)
    {
        for (var i = 0; i < array.Length; i++)
        {
            if (EqualityComparer<T>.Default.Equals(array[i], value))
                return i;
        }

        return -1;
    }

    /// <summary>
    /// Converts an array of <see cref="object"/>s to an array of their <see cref="Type"/>s.
    /// </summary>
    public static Type[] ToTypeArray(this object?[]? objectArray)
    {
        if (objectArray is null) return Type.EmptyTypes;
        var len = objectArray.Length;
        if (len == 0) return Type.EmptyTypes;
        Type[] types = new Type[len];
        for (var i = 0; i < len; i++)
        {
            types[i] = objectArray[i]?.GetType() ?? typeof(void);
        }
        return types;
    }

/// <summary>
/// Gets every possible pair of values in this array
/// </summary>
/// <param name="array"></param>
/// <typeparam name="T"></typeparam>
/// <returns></returns>
    public static IEnumerable<T[]> GetPairs<T>(this T[] array)
    {
        var len = array.Length;
        if (len < 2)
            yield break;

        for (var s = 0; s < len; s++)
        {
            for (var e = (s + 1); e < len; e++)
            {
                yield return new T[2] { array[s], array[e] };
            }
        }
    }

    /// <summary>
    /// Initializes each element of this <paramref name="array"/> to the <paramref name="@default"/> value.
    /// </summary>
    public static void Initialize<T>(this T[] array, T value)
    {
        var len = array.Length;
        for (var i = 0; i < len; i++)
        {
            array[i] = value;
        }
    }

    public static TOut[] SelectToArray<TIn, TOut>(this TIn[] array,
        Func<TIn, TOut> transform)
    {
        var len = array.Length;
        var output = new TOut[len];
        for (var i = 0; i < len; i++)
        {
            output[i] = transform(array[i]);
        }
        return output;
    }
    
    public static IEnumerable<T> Reversed<T>(this T[] array)
    {
        for (var i = array.Length - 1; i >= 0; i--)
        {
            yield return array[i];
        }
    }

    public static bool SequenceEqual<T>(this T[] array, ReadOnlySpan<T> items)
    {
        return MemoryExtensions.SequenceEqual(array, items);
    }
}