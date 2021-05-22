using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

namespace Jay
{
    public static class ArrayExtensions
    {
        public static T? GetOrDefault<T>(this T?[]? array, int index, T? @default = default(T))
        {
            if (array is null)
                return @default;
            if ((uint) index > (uint) array.Length)
                return @default;
            return array[index];
        }

        public static Span<T> Slice<T>(this T[] array, int index) => ((Span<T>) array).Slice(index);
        public static Span<T> Slice<T>(this T[] array, int index, int length) => ((Span<T>) array).Slice(index, length);
        public static Span<T> Slice<T>(this T[] array, Range range) => ((Span<T>) array)[range];

        public static bool Contains<T>(this T?[]? array, T? item)
        {
            if (array is not null && array.Length > 0)
            {
                for (var i = 0; i < array.Length; i++)
                {
                    if (EqualityComparer<T>.Default.Equals(array[i], item))
                        return true;
                }
            }
            return false;
        }
    }
}