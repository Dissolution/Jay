using System.Buffers;
using Jay.Text;

namespace Jay.Memory;

public static class RefArrayWriter
{
    public static void Write<T>(
        [AllowNull, NotNull] ref T[]? array, 
        ref int arrayIndex, 
        T item)
    {
        int newIndex = arrayIndex + 1;
        if (array is null)
        {
            array = ArrayPool<T>.Shared.Rent(newIndex);
        }
        else if (newIndex > array.Length)
        {
            var newArray = ArrayPool<T>.Shared.Rent(newIndex);
            Easy.CopyTo(array, newArray);
            ArrayPool<T>.Shared.Return(array);
            array = newArray;
        }

        array[arrayIndex] = item;
        arrayIndex = newIndex;
    }
    
    public static void WriteAll<T>(
        [AllowNull, NotNull] ref T[]? array, 
        ref int arrayIndex, 
        ReadOnlySpan<T> items)
    {
        int newIndex = arrayIndex + items.Length;
        if (array is null)
        {
            array = ArrayPool<T>.Shared.Rent(newIndex);
        }
        else if (newIndex > array.Length)
        {
            var newArray = ArrayPool<T>.Shared.Rent(newIndex);
            Easy.CopyTo(array, newArray);
            ArrayPool<T>.Shared.Return(array);
            array = newArray;
        }

        Easy.CopyTo(items, array.AsSpan(arrayIndex));
        arrayIndex = newIndex;
    }

    public static void WriteAll<T>(
        [AllowNull, NotNull] ref T[]? array,
        ref int arrayIndex,
        params T[] items)
        => WriteAll<T>(ref array, ref arrayIndex, items.AsSpan());

    public static void Write(
        [AllowNull, NotNull] ref char[]? charArray,
        ref int arrayIndex,
        char ch)
    {
        int newIndex = arrayIndex + 1;
        if (charArray is null)
        {
            charArray = ArrayPool<char>.Shared.Rent(newIndex);
        }
        else if (newIndex > charArray.Length)
        {
            var newArray = ArrayPool<char>.Shared.Rent(newIndex);
            Easy.CopyTo(charArray, newArray);
            ArrayPool<char>.Shared.Return(charArray);
            charArray = newArray;
        }

        charArray[arrayIndex] = ch;
        arrayIndex = newIndex;
    }
    
    public static void Write(
        [AllowNull, NotNull] ref char[]? charArray, 
        ref int arrayIndex, 
        ReadOnlySpan<char> text)
    {
        int newIndex = arrayIndex + text.Length;
        if (charArray is null)
        {
            charArray = ArrayPool<char>.Shared.Rent(newIndex);
        }
        else if (newIndex > charArray.Length)
        {
            var newArray = ArrayPool<char>.Shared.Rent(newIndex);
            Easy.CopyTo(charArray, newArray);
            ArrayPool<char>.Shared.Return(charArray);
            charArray = newArray;
        }

        Easy.CopyTo(text, charArray.AsSpan(arrayIndex));
        arrayIndex = newIndex;
    }

    public static void Write(
        [AllowNull, NotNull] ref char[]? charArray,
        ref int arrayIndex,
        params char[]? characters)
        => Write(ref charArray, ref arrayIndex, characters.AsSpan());
    
    public static void Write(
        [AllowNull, NotNull] ref char[]? charArray,
        ref int arrayIndex,
        string? str)
        => Write(ref charArray, ref arrayIndex, str.AsSpan());
}
