namespace Jay.Comparision;

public sealed class FuncComparer<T> : Comparer<T>
{
    private readonly Func<T?, T?, int> _compare;

    public FuncComparer(Func<T?, T?, int> compare)
    {
        _compare = compare;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public override int Compare(T? x, T? y)
    {
        return _compare(x, y);
    }
}