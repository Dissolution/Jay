namespace Jay.CodeGen.Extensions;

internal static class HashCodeExtensions
{
    public static void AddArray<T>(this ref HashCode hashCode, params T?[]? array)
    {
        if (array is null) return;
        int len = array.Length;
        for (var i = 0; i < len; i++)
        {
            hashCode.Add<T?>(array[i]);
        }
    }
    public static void AddEnumerable<T>(this ref HashCode hashCode, IEnumerable<T?>? enumerable)
    {
        if (enumerable is null) return;
        foreach (T? item in enumerable)
        {
            hashCode.Add<T?>(item);
        }
    }
}