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

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
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

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
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

    [return: MaybeNull]
    public static T GetOrDefault<T>(this T[]? array, int index, [AllowNull] T @default = default(T))
    {
        if (array is null)
            return @default;
        if ((uint)index > (uint)array.Length)
            return @default;
        return array[index];
    }

    public static bool TryGetItem<T>(this T[]? array, int index, [MaybeNullWhen(false)] out T item)
    {
        if (array is null || (uint)index > (uint)array.Length)
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
}