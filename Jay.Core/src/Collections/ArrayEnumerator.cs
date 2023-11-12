namespace Jay.Collections;

public sealed class ArrayEnumerator<T> : 
    IEnumerator<T>, 
    IEnumerator,
    IDisposable
{
    private Array _array;
    private readonly int _rank;
    private readonly int[] _lowerBounds;
    private readonly int[] _upperBounds;
    
    private readonly int[] _arrayIndex;

    public int Rank => _rank;
    public int[] Index => _arrayIndex;
    object IEnumerator.Current => _array.GetValue(_arrayIndex)!;
    public T Current => (T)_array.GetValue(_arrayIndex)!;
    
    internal ArrayEnumerator(Array array, int rank, int[] lowerBounds, int[] upperBounds)
    {
        _array = array;
        _rank = rank;
        _lowerBounds = lowerBounds;
        _upperBounds = upperBounds;
        var index = _arrayIndex = new int[rank];
        
        // The last item in arrayIndex needs to be at '-1'
        int end = rank - 1;
        for (var dim = 0; dim <= end; dim++)
        {
            if (dim < end)
            {
                index[dim] = lowerBounds[dim];
            }
            else
            {
                index[dim] = lowerBounds[dim] - 1;
            }
        }
    }

    public bool TryMoveNext([MaybeNullWhen(false)] out T value)
    {
        if (MoveNext())
        {
            value = Current;
            return true;
        }
        else
        {
            value = default;
            return false;
        }
    }

    private bool TryRollNext(int index)
    {
        if (index < 0) return false;
        var arrayIndex = _arrayIndex;
        var rankValue = arrayIndex[index];
        if (rankValue >= _upperBounds[index])
        {
            // We have to be able to roll the one to the left
            if (!TryRollNext(index - 1))
                return false;
            // Reset me
            arrayIndex[index] = _lowerBounds[index];
            return true;
        }
        
        // Roll Me
        arrayIndex[index] = rankValue + 1;
        return true;
    }
    
    public bool MoveNext()
    {
        return TryRollNext(_rank - 1);
    }

    public void Reset()
    {
        var arrayIndex = _arrayIndex;
        int i = _rank - 1;
        arrayIndex[i] = _lowerBounds[i] - 1;
        i--;
        while (i > 0)
        {
            arrayIndex[i] = _lowerBounds[i];
            i--;
        }
    }

    void IDisposable.Dispose()
    {
        _array = null!;
    }
}