using System.Runtime.CompilerServices;

﻿namespace Jay;

public static class ArrayExtensions
{
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
    }

