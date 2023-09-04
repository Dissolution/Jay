using System.Collections;

namespace Jay.Reflection.Utilities;

public class ArrayEnumerator : IEnumerator<object?>, IEnumerator
{
    protected readonly ArrayWrapper _wrappedArray;
    
    private int[]? _indices;

    public int[] Indices => _indices ?? throw new InvalidOperationException("Enumeration has not started");

    public object? Current => _wrappedArray.GetValue(Indices);

    public ArrayEnumerator(ArrayWrapper wrappedArray)
    {
        _wrappedArray = wrappedArray;
    }

    private bool TryIncrementIndex(int rank)
    {
        var wrappedArray = _wrappedArray;
        var indexes = _indices!;
        
        // cannot increment a rank we do not have
        if (rank > wrappedArray.Rank) return false;

        int nextIndex = indexes[rank] + 1;
        
        // Will we go over upper bound?
        if (nextIndex > wrappedArray.UpperBounds[rank])
        {
            // Increment the next rank
            if (!TryIncrementIndex(rank + 1))
                return false;

            // Reset my rank back to its lowest bound
            indexes[rank] = wrappedArray.LowerBounds[rank];
        }
        else
        {
            // Increment my index
            indexes[rank] = nextIndex;
        }
        return true;
    }

    public bool MoveNext()
    {
        if (_indices is null)
        {
            _indices = new int[_wrappedArray.Rank];
            Easy.CopyTo<int>(_wrappedArray.LowerBounds, _indices);
        }

        if (TryIncrementIndex(0))
        {
            return true;
        }
        
        return false;
    }

    public void Reset()
    {
        _indices = null;
    }
    
    public void Dispose() { }

    public override string ToString()
    {
        if (_indices is null)
            return "Enumeration has not started";
        return CodePart.ToCode($"{_indices}: {Current}");
    }
}