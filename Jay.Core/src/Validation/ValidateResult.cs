using static Jay.Result_StaticImport;

namespace Jay.Validation;

public class ValidateResult
{
    public static Result IsNotNull<T>(
        [AllowNull, NotNullWhen(true)] T? value,
        string? exceptionMessage = null,
        [CallerArgumentExpression(nameof(value))]
        string? valueName = null)
    {
        if (value is not null)
            return Ok();

        return new ArgumentNullException(valueName, exceptionMessage);
    }

    public static Result IsNotNullOrEmpty(
        [AllowNull, NotNullWhen(true)] string? str,
        [CallerArgumentExpression(nameof(str))]
        string? strName = null)
    {
        if (!string.IsNullOrEmpty(str))
            return Ok();

        return new ArgumentNullException(strName);
    }

    public static Result IsNotNullOrWhiteSpace(
        [AllowNull, NotNullWhen(true)] string? str,
        [CallerArgumentExpression(nameof(str))]
        string? strName = null)
    {
        if (!string.IsNullOrWhiteSpace(str))
            return Ok();

        return new ArgumentNullException(strName);
    }

#region Indexing
    public static Result Index(
        int available,
        int index,
        bool insert = false,
        [CallerArgumentExpression(nameof(index))]
        string? indexName = null)
    {
        if (!insert)
            available -= 1;
        
        if ((uint)index <= available)
            return Ok();

        return new ArgumentOutOfRangeException(
            indexName,
            index,
            $"{indexName} must be between 0 and {available}");
    }

    public static Result<int> Index(
        int available,
        Index index,
        bool insert = false,
        [CallerArgumentExpression(nameof(index))]
        string? indexName = null)
    {
        int offset = index.GetOffset(available);
       
        if (!insert)
            available -= 1;
        
        if ((uint)offset <= available)
            return Ok(offset);

        return new ArgumentOutOfRangeException(
            indexName,
            index,
            $"{indexName} must be between 0 and {available}");
    }
    
    public static Result Range(
        int available,
        int start,
        int length,
        [CallerArgumentExpression(nameof(start))]
        string? startName = null,
        [CallerArgumentExpression(nameof(length))]
        string? lengthName = null)
    {
        if ((uint)start > available)
            return new ArgumentOutOfRangeException(startName, start, $"Starting Index must be between 0 and {available}");
        if ((uint)length > (available - start))
            return new ArgumentOutOfRangeException(lengthName, length, $"Length must be between 0 and {available - start}");
        return Ok();
    }
    
    public static Result<(int Start, int Length)> Range(
        int available,
        Range range,
        [CallerArgumentExpression(nameof(range))]
        string? rangeName = null)
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

        int length = end - start;

        if ((uint)start + (uint)length <= available)
            return Ok((start, length));

        throw new ArgumentOutOfRangeException(
            rangeName,
            range,
            $"{rangeName} must be between in [0..{available})");
    }
#endregion
    
    public static Result CanCopyTo(int count, Array? array, int arrayIndex = 0)
    {
        if (array is null)
            return new ArgumentNullException(nameof(array));
        if (array.Rank != 1)
            return new ArgumentException("Array must have a rank of 1", nameof(array));
        if (array.GetLowerBound(0) != 0)
            return new ArgumentException("Array must have a lower bound of 0", nameof(array));
        if ((uint)arrayIndex > array.Length)
            return new IndexOutOfRangeException($"Array Index '{arrayIndex}' must be between 0 and {array.Length - 1}");
        if (array.Length - arrayIndex < count)
            return new ArgumentException($"Array must have a capacity of at least {arrayIndex + count}", nameof(array));
        return Ok();
    }

    public static Result CanCopyTo<T>(int count, T[]? array, int arrayIndex = 0)
    {
        if (array is null)
            return new ArgumentNullException(nameof(array));
        if ((uint)arrayIndex > array.Length)
            return new IndexOutOfRangeException($"Array Index '{arrayIndex}' must be between 0 and {array.Length - 1}");
        if (array.Length - arrayIndex < count)
            return new ArgumentException($"Array must have at a capacity of at least {arrayIndex + count}", nameof(array));
        return Ok();
    }

    public static Result CanCopyTo<T>(int count, Span<T> span, int spanIndex = 0)
    {
        if ((uint)spanIndex > span.Length)
            return new IndexOutOfRangeException($"Span Index '{spanIndex}' must be between 0 and {span.Length - 1}");
        if (span.Length - spanIndex < count)
            return new ArgumentException($"Span must have at a capacity of at least {spanIndex + count}", nameof(span));
        return Ok();
    }

}