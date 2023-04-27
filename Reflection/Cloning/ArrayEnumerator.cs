using Jay.Utilities;

namespace Jay.Reflection.Cloning;

public sealed class ArrayEnumerator : IEnumerator<object?>, IEnumerator
{
    private readonly ArrayWrapper _arrayEnumerable;
    private int[]? _indices;
    private object? _current;

    public int[] Indices => _indices ?? throw new InvalidOperationException();

    public object? Current => _current;

    public ArrayEnumerator(ArrayWrapper arrayEnumerable)
    {
        _arrayEnumerable = arrayEnumerable;
    }

    private bool TryIncrementIndex(int rank)
    {
        // Are we trying to imcrement a non-existent rank? can't!
        if (rank > _arrayEnumerable.Rank) return false;

        int nextIndex = _indices![rank] + 1;
        // Will we go over upper bound?
        if (nextIndex > _arrayEnumerable.UpperBounds[rank])
        {
            // Increment the next rank
            if (!TryIncrementIndex(rank + 1))
                return false;

            // Reset my rank back to its lowest bound
            _indices[rank] = _arrayEnumerable.LowerBounds[rank];
        }
        else
        {
            // Increment my index
            _indices[rank] = nextIndex;
        }
        return true;
    }

    public bool MoveNext()
    {
        if (_indices is null)
        {
            _indices = new int[_arrayEnumerable.Rank];
            Fast.Copy<int>(_arrayEnumerable.LowerBounds, _indices);
        }

        if (TryIncrementIndex(0))
        {
            _current = _arrayEnumerable.GetValue(_indices);
            return true;
        }

        _current = null;
        return false;
    }

    public void Reset()
    {
        _indices = null;
        _current = null;
    }
    
    public void Dispose() { }

    public override string ToString()
    {
        if (_indices is null)
            return "Enumeration has not started";
        return Dump($"{_indices}: {_current:V}");
    }
}