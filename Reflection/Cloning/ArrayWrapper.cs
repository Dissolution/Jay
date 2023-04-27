namespace Jay.Reflection.Cloning;

public class ArrayWrapper : IEnumerable<object?>
{
    protected readonly Array _array;

    public int Rank { get; }

    public int[] LowerBounds { get; }
    public int[] UpperBounds { get; }
    public int[] RankLengths { get; }
    public Type ElementType { get; }

 
    public object? this[int[] indices]
    {
        get => GetValue(indices);
        set => SetValue(indices, value);
    }
    
    public long Capacity => _array.LongLength;
    
    public ArrayWrapper(Array array)
    {
        _array = array;
        ElementType = _array.GetType().GetElementType()!;
        int rank = Rank = array.Rank;
        LowerBounds = new int[rank];
        UpperBounds = new int[rank];
        RankLengths = new int[rank];
        for (var r = 0; r < rank; r++)
        {
            int lower = array.GetLowerBound(r);
            int upper = array.GetUpperBound(r);
            LowerBounds[r] = lower;
            UpperBounds[r] = upper;
            RankLengths[r] = upper - lower;
        }
    }
    
    public object? GetValue(int[] indices)
    {
        return _array.GetValue(indices);
    }

    public void SetValue(int[] indices, object? value)
    {
        _array.SetValue(value, indices);
    }

    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
    IEnumerator<object?> IEnumerable<object?>.GetEnumerator() => GetEnumerator();
    public ArrayEnumerator GetEnumerator()
    {
        return new ArrayEnumerator(this);
    }
}