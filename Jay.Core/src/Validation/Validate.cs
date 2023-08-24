namespace Jay.Validation;

/// <summary>
/// A utility class for all sort of Validation
/// </summary>
public static class Validate
{
    private static (int offset, int length) FastOffsetLength(int available, Range range)
    {
        int start;
        Index startIndex = range.Start;
        if (startIndex.IsFromEnd)
            start = available - startIndex.Value;
        else
            start = startIndex.Value;

        int end;
        Index endIndex = range.End;
        if (endIndex.IsFromEnd)
            end = available - endIndex.Value;
        else
            end = endIndex.Value;

        return (start, end - start);
    }


    /// <summary>
    /// Validates <paramref name="value" />:<br />
    /// If <c>null</c>, throws a <see cref="ArgumentNullException" />
    /// </summary>
    /// <typeparam name="T">
    /// The <see cref="Type" /> of <paramref name="value" /> to validate
    /// </typeparam>
    /// <param name="value">
    /// The <typeparamref name="T" /> value to check for <c>null</c>
    /// </param>
    /// <param name="exMessage">
    /// An optional message for a thrown <see cref="ArgumentNullException" />
    /// </param>
    /// <param name="valueName">
    /// The name of the value argument
    /// </param>
    /// <exception cref="ArgumentNullException">
    /// Thrown if <paramref name="value" /> is <c>null</c>
    /// </exception>
    public static void IsNotNull<T>([AllowNull] [NotNull] T value,
        string? exMessage = null,
        [CallerArgumentExpression(nameof(value))]
        string? valueName = null)
    {
        if (value is null)
            throw new ArgumentNullException(valueName, exMessage);
    }

    public static void IsBetween<T>(T value,
        T inclusiveMinimum,
        T inclusiveMaximum,
        [CallerArgumentExpression(nameof(value))]
        string? valueName = null)
    {
        var comparer = Comparer<T>.Default;

        int c = comparer.Compare(value, inclusiveMinimum);
        if (c < 0)
            throw new ArgumentOutOfRangeException(valueName, value, $"Value must be greater than or equal to {inclusiveMinimum}");
        c = comparer.Compare(value, inclusiveMaximum);
        if (c > 0)
            throw new ArgumentOutOfRangeException(valueName, value, $"Value must be lesser than or equal to {inclusiveMaximum}");
    }


    /// <summary>
    /// Validates that the <paramref name="index" /> fits in <paramref name="available" />
    /// </summary>
    /// <param name="available">The number of items available to be indexed into</param>
    /// <param name="index">The index attempting to be referenced</param>
    /// <param name="indexName">The name of the index parameter</param>
    /// <exception cref="ArgumentOutOfRangeException">
    /// Thrown if <paramref name="index" /> <c>is</c> &lt; 0 <c>or</c> &gt;= <paramref name="available" />
    /// </exception>
    public static void Index(int available, 
        int index,
        [CallerArgumentExpression(nameof(index))]
        string? indexName = null)
    {
        if ((uint)index < (uint)available) return;
        throw new ArgumentOutOfRangeException(
            indexName,
            index,
            $"{indexName} must be between 0 and {available - 1}");
    }

    public static void Index(int available, 
        Index index,
        [CallerArgumentExpression(nameof(index))]
        string? indexName = null)
    {
        int offset = index.GetOffset(available);
        if ((uint)offset < available) return;
        throw new ArgumentOutOfRangeException(indexName,
            index,
            $"{indexName} {index} must be between 0 and {available - 1}");
    }

    public static void InsertIndex(int available, int index,
        [CallerArgumentExpression(nameof(index))]
        string? indexName = null)
    {
        if ((uint)index <= available) return;
        throw new ArgumentOutOfRangeException(indexName,
            index,
            $"Insert {indexName} {index} must be between 0 and {available}");
    }
    
    public static void Range(
        int available,
        int start,
        int length,
        [CallerArgumentExpression(nameof(start))]
        string? startName = null,
        [CallerArgumentExpression(nameof(length))]
        string? lengthName = null)
    {
        if ((uint)start > (uint)available)
            throw new ArgumentOutOfRangeException(startName, start, $"Starting Index must be between 0 and {available}");
        if ((uint)length > (uint)(available - start))
            throw new ArgumentOutOfRangeException(lengthName, length, $"Length must be between 0 and {available - start}");
    }


    public static void Range(
        int available,
        Range range,
        [CallerArgumentExpression(nameof(range))]
        string? rangeName = null)
    {
        (int offset, int length) = FastOffsetLength(available, range);
        if ((uint)offset + (uint)length <= (uint)available) return;
        throw new ArgumentOutOfRangeException(
            rangeName,
            range,
            $"{rangeName} must be between 0 and {available - 1}");
    }

    public static (int offset, int length) RangeResolveOffsetLength(
        int available,
        Range range,
        [CallerArgumentExpression(nameof(range))]
        string? rangeName = null)
    {
        (int offset, int length) ol = FastOffsetLength(available, range);
        if ((uint)ol.offset + (uint)ol.length <= (uint)available) return ol;
        throw new ArgumentOutOfRangeException(
            rangeName,
            range,
            $"{rangeName} must be between 0 and {available - 1}");
    }


  



    public static void CanCopyTo(int count, Array? array, int arrayIndex = 0)
    {
        if (array is null)
            throw new ArgumentNullException(nameof(array));
        if (array.Rank != 1)
            throw new ArgumentException("Array must have a rank of 1", nameof(array));
        if (array.GetLowerBound(0) != 0)
            throw new ArgumentException("Array must have a lower bound of 0", nameof(array));
        if ((uint)arrayIndex > array.Length)
            throw new IndexOutOfRangeException($"Array Index '{arrayIndex}' must be between 0 and {array.Length - 1}");
        if (array.Length - arrayIndex < count)
            throw new ArgumentException($"Array must have a capacity of at least {arrayIndex + count}", nameof(array));
    }

    public static void CanCopyTo<T>(int count, T[]? array, int arrayIndex = 0)
    {
        if (array is null)
            throw new ArgumentNullException(nameof(array));
        if ((uint)arrayIndex > array.Length)
            throw new IndexOutOfRangeException($"Array Index '{arrayIndex}' must be between 0 and {array.Length - 1}");
        if (array.Length - arrayIndex < count)
            throw new ArgumentException($"Array must have at a capacity of at least {arrayIndex + count}", nameof(array));
    }
    
    public static void CanCopyTo<T>(int count, Span<T> span, int spanIndex = 0)
    {
        if ((uint)spanIndex > span.Length)
            throw new IndexOutOfRangeException($"Span Index '{spanIndex}' must be between 0 and {span.Length - 1}");
        if (span.Length - spanIndex < count)
            throw new ArgumentException($"Span must have at a capacity of at least {spanIndex + count}", nameof(span));
    }
}