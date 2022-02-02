namespace Jay;

public static class Clamp
{
    public static T Value<T>(T value, T inclusiveMin, T inclusiveMax)
        where T : IComparable<T>
    {
        var comparer = Comparer<T>.Default;
        int c = comparer.Compare(inclusiveMin, inclusiveMax);
        if (c > 0)
        {
            throw new ArgumentException($"Inclusive Min '{inclusiveMin}' is larger than Inclusive Max '{inclusiveMax}",
                nameof(inclusiveMax));
        }

        c = comparer.Compare(value, inclusiveMin);
        if (c < 0)
            return inclusiveMin;

        c = comparer.Compare(value, inclusiveMax);
        if (c > 0)
            return inclusiveMax;

        return value;
    }
}