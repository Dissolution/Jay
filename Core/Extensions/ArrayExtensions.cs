using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;
using Jay.Collections;

namespace Jay
{
    public static class ArrayExtensions
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool NullOrNone<T>([NotNullWhen(false)] this T[]? array)
        {
            if (array is null) return true;
            if (array.Length == 0) return true;
            return false;
        }
        
        public static T? GetOrDefault<T>(this T?[]? array, int index, T? @default = default(T))
        {
            if (array is null)
                return @default;
            if ((uint) index > (uint) array.Length)
                return @default;
            return array[index];
        }

        public static bool TryGet<T>(this T?[]? array, int index, out T? item)
        {
            if (array != null && (uint) index < (uint) array.Length)
            {
                item = array[index];
                return true;
            }
            item = default;
            return false;
        }
        
        public static bool TryGet<T>(this T?[]? array, Index index, out T? item)
        {
            if (array != null)
            {
                int offset = index.GetOffset(array.Length);
                if ((uint) offset < (uint) array.Length)
                {
                    item = array[index];
                    return true;
                }
            }
            item = default;
            return false;
        }

        public static Span<T> Slice<T>(this T[] array, int index) => ((Span<T>) array).Slice(index);
        public static Span<T> Slice<T>(this T[] array, Index index) => ((Span<T>) array)[Range.StartAt(index)];
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

        public static IEnumerator<T> GetEnumerator<T>(this T[]? array)
        {
            if (array != null)
            {
                var len = array.Length;
                for (var i = 0; i < len; i++)
                {
                    yield return array[i];
                }
            }
        }
        
        public static ArrayEnumerator GetArrayEnumerator(this Array array) => ArrayEnumerator.Create(array);
        public static ArrayWrapper ToArrayEnumerable(this Array array) => new ArrayWrapper(array);
        
        public static void ForEach<T>(this T[]? array, Action<T>? forEach)
        {
            if (array is null) return;
            if (forEach is null) return;
            int length = array.Length;
            for (var i = 0; i < length; i++)
            {
                forEach(array[i]);
            }
        }
    }
}