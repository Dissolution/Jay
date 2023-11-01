using System.Collections;
using System.Diagnostics;
using Jay.Utilities;


namespace Jay.Reflection.Utilities;

/// <summary>
/// A wrapper around an <see cref="Array"/> that provides clean access to N-dimensional arrays.
/// </summary>
public class ArrayWrapper : 
    IEnumerable<object?>, IEnumerable
{
    protected readonly Array _array;

    /// <summary>
    /// The number of dimensions in the <see cref="Array"/>
    /// </summary>
    public int Rank { get; }

    /// <summary>
    /// The indexes of the lower bounds of the dimensions in the <see cref="Array"/>
    /// </summary>
    public int[] LowerBounds { get; }
    /// <summary>
    /// The indexes of the upper bounds of the dimensions in the <see cref="Array"/>
    /// </summary>
    public int[] UpperBounds { get; }
    /// <summary>
    /// The lengths of the dimensions in the <see cref="Array"/>
    /// </summary>
    public int[] RankLengths { get; }
    
    /// <summary>
    /// The <see cref="Type"/> of the elements stored in the <see cref="Array"/>
    /// </summary>
    public Type ElementType { get; }

    /// <summary>
    /// Gets or sets the <see cref="object"/> at the given <paramref name="indices"/>
    /// </summary>
    /// <param name="indices">The <see cref="Rank"/>-dimensional indices of the <see cref="object"/></param>
    public object? this[int[] indices]
    {
        get => GetValue(indices);
        set => SetValue(indices, value);
    }
    
    /// <summary>
    /// Gets the total number of items in the <see cref="Array"/>
    /// </summary>
    public long Capacity => _array.LongLength;
    
    public ArrayWrapper(Array array)
    {
        _array = array;
        ElementType = _array.GetType().GetElementType().ThrowIfNull();
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

    public bool Equals(ArrayWrapper? arrayWrapper)
    {
        return ReferenceEquals(_array, arrayWrapper?._array);
    }

    public bool Equals(Array? array)
    {
        return ReferenceEquals(_array, array);
    }
    
    public override bool Equals(object? obj)
    {
        if (obj is ArrayWrapper arrayWrapper) return Equals(arrayWrapper);
        if (obj is Array array) return Equals(array);
        return false;
    }
    
    public override int GetHashCode()
    {
        var hasher = new Hasher();
        using var e = GetEnumerator();
        while (e.MoveNext())
        {
            hasher.Add<object?>(e.Current);
        }
        return hasher.ToHashCode();
    }

    public override string ToString()
    {
        Debug.Assert(Rank > 0);
        // 1D array is much easier
        if (Rank == 1)
        {
            return TextBuilder.New.Append('[').Delimit(static c => c.Write(", "), this, static (c, o) => c.Write(o)).ToStringAndDispose();
        }
        else
        {
            throw new NotImplementedException();
        }
    }
}