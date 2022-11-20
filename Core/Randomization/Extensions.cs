namespace Jay.Randomization;

public static class Extensions
{
    public static IReadOnlyList<T> MixToList<T>(this T[] array)
    {
        return Randomizer.Instance.MixToList(array);
    }

    public static IReadOnlyList<T> MixToList<T>(this IList<T> list)
    {
        return Randomizer.Instance.MixToList(list);
    }

    public static IReadOnlyList<T> MixToList<T>(this IEnumerable<T> enumerable)
    {
        return Randomizer.Instance.MixToList(enumerable);
    }
}