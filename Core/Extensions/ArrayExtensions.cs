using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;

namespace Jay;

public static class ArrayExtensions
{
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool IsNullOrEmpty<T>([NotNullWhen(false)] this T?[]? array)
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

        public static T? GetOrDefault<T>(this T?[]? array, int index, T? @default = default(T))
        {
            if (array is null)
                return @default;
            if ((uint)index > (uint)array.Length)
                return @default;
            return array[index];
        }
}

