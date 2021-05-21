using System;

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
    }
}