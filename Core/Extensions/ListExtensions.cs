namespace Jay;

public static class ListExtensions
{
    public static bool TryGetItem<T>(this IList<T> list, 
        int index, 
        [NotNullWhen(true)] out T? value)
    {
        if (index < 0 || index >= list.Count)
        {
            value = default;
            return false;
        }

        value = list[index]!;
        return true;
    }
        
    public static int IndexOf<T>(this IReadOnlyList<T> list, 
        T value)
    {
        for (var i = 0; i < list.Count; i++)
        {
            if (EqualityComparer<T>.Default.Equals(list[i], value))
                return i;
        }
        return -1;
    }

    public static IEnumerable<T> Reversed<T>(this IReadOnlyList<T>? readOnlyList)
    {
        if (readOnlyList is null)
            yield break;
        for (var i = readOnlyList.Count - 1; i >= 0; i--)
        {
            yield return readOnlyList[i];
        }
    }
    public static IEnumerable<T> Reversed<T>(this IList<T>? list)
    {
        if (list is null)
            yield break;
        for (var i = list.Count - 1; i >= 0; i--)
        {
            yield return list[i];
        }
    }

    public static bool TryRemoveAt<T>(this IList<T>? list, int index, out T? value)
    {
        if (list is null ||
            list.Count == 0 ||
            index < 0 ||
            index >= list.Count)
        {
            value = default;
            return false;
        }

        value = list[index];
        list.RemoveAt(index);
        return true;
    }
}