namespace Jay.Comparison;

public sealed class FuncEqualityComparer<T> : EqualityComparer<T>
{
    private readonly Func<T?, T?, bool> _equals;
    private readonly Func<T?, int> _getHashCode;

    public FuncEqualityComparer(Func<T?, T?, bool> equals, Func<T?, int> getHashCode)
    {
        _equals = equals;
        _getHashCode = getHashCode;
    }

    public override bool Equals(T? x, T? y) => _equals(x, y);

    public override int GetHashCode(T? obj) => _getHashCode(obj);
}