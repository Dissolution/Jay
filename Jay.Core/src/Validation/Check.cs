#pragma warning disable CS8777 // Parameter must have a non-null value when exiting.
using static Jay.StaticImports;

namespace Jay.Validation;

public static class Check
{
    public static Result IfNull<T>(
        [AllowNull, NotNullWhen(true)] T? value,
        [CallerArgumentExpression(nameof(value))]
        string? valueName = null)
    {
        if (value is not null)
        {
            return Ok();
        }
        return Error(new ArgumentNullException(valueName));
    }

    public static Result IfNullOrEmpty(
        [AllowNull, NotNull] string? str,
        [CallerArgumentExpression(nameof(str))]
        string? strName = null)
    {
        if (!string.IsNullOrEmpty(str))
        {
            return Ok();
        }
        return Error(new ArgumentException("String cannot be null or empty", strName));
    }

    public static Result IfNullOrWhiteSpace(
        [AllowNull, NotNull] string? str,
        [CallerArgumentExpression(nameof(str))]
        string? strName = null)
    {
        if (!string.IsNullOrWhiteSpace(str))
        {
            return Ok();
        }
        return Error(new ArgumentException("String cannot be null, empty, or whitespace", strName));
    }

    public static Result IfNotIn<T>(
        T value,
        T inclusiveMinimum,
        T inclusiveMaximum,
        [CallerArgumentExpression(nameof(value))]
        string? valueName = null)
    {
        var comparer = Comparer<T>.Default;
        if (comparer.Compare(value, inclusiveMinimum) >= 0 && comparer.Compare(value, inclusiveMaximum) <= 0)
        {
            return Ok();
        }
        return new ArgumentOutOfRangeException(valueName, value, $"{valueName} must be in [{inclusiveMinimum}..{inclusiveMaximum}]");
    }
    
    public static Result IfNotIn<T>(
        T value,
        IEnumerable<T> options,
        [CallerArgumentExpression(nameof(value))]
        string? valueName = null)
    {
        var comparer = EqualityComparer<T>.Default;
        if (options.Any(opt => comparer.Equals(opt, value)))
        {
            return Ok();
        }
        return new ArgumentOutOfRangeException(valueName, value, $"{valueName} must be in {options}");
    }

    public static Result Index(
        int available,
        int index,
        bool insert = false,
        [CallerArgumentExpression(nameof(index))]
        string? indexName = null)
    {
        if (!insert)
        {
            if ((uint)index < available)
                return Ok();

            return new ArgumentOutOfRangeException(
                indexName,
                index,
                $"'{indexName}' {index} is not in [0..{available})");
        }
        else
        {
            if ((uint)index <= available)
                return Ok();

            return new ArgumentOutOfRangeException(
                indexName,
                index,
                $"'{indexName}' {index} is not in [0..{available}]");
        }
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
        {
            if ((uint)offset < available)
                return Ok(offset);

            return new ArgumentOutOfRangeException(
                indexName,
                offset,
                $"'{indexName}' {offset} is not in [0..{available})");
        }
        else
        {
            if ((uint)offset <= available)
                return Ok(offset);

            return new ArgumentOutOfRangeException(
                indexName,
                offset,
                $"'{indexName}' {offset} is not in [0..{available}]");
        }
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
        {
            return new ArgumentOutOfRangeException(
                startName,
                start,
                $"({startName}, {lengthName}) ({start}, {length}) is not in [0..{available})");
        }
        if ((uint)length > (available - start))
        {
            return new ArgumentOutOfRangeException(
                lengthName,
                length,
                $"({startName}, {lengthName}) ({start}, {length}) is not in [0..{available})");
        }

        return Ok();
    }

    public static Result<(int Start, int Length)> Range(
        int available,
        Range range,
        [CallerArgumentExpression(nameof(range))]
        string? rangeName = null)
    {
        int start = range.Start.GetOffset(available);
        int end = range.End.GetOffset(available);
        int length = end - start;

        if ((uint)start + (uint)length <= available)
            return Ok((start, length));

        throw new ArgumentOutOfRangeException(
            rangeName,
            range,
            $"'{rangeName}' {range} is not in [0..{available})");
    }

    public static Result CanCopyTo(
        int count,
        Array? array,
        int arrayIndex = 0,
        [CallerArgumentExpression(nameof(array))]
        string? arrayName = null)
    {
        if (array is null)
            return new ArgumentNullException(arrayName);
        if (array.Rank != 1)
            return new ArgumentException("Array must have a rank of 1", arrayName);
        if (array.GetLowerBound(0) != 0)
            return new ArgumentException("Array must have a lower bound of 0", arrayName);
        if ((uint)arrayIndex > array.Length)
            return new IndexOutOfRangeException($"Array Index {arrayIndex} is not in [0..{array.Length})");
        if (array.Length - arrayIndex < count)
            return new ArgumentException($"Array must have a capacity of at least {arrayIndex + count}", arrayName);

        return Ok();
    }

    public static Result CanCopyTo<T>(
        int count,
        T[]? array,
        int arrayIndex = 0,
        [CallerArgumentExpression(nameof(array))]
        string? arrayName = null)
    {
        if (array is null)
            return new ArgumentNullException(arrayName);
        if ((uint)arrayIndex > array.Length)
            return new IndexOutOfRangeException($"Array Index {arrayIndex} is not in [0..{array.Length})");
        if (array.Length - arrayIndex < count)
            return new ArgumentException($"Array must have a capacity of at least {arrayIndex + count}", arrayName);

        return Ok();
    }
}


#pragma warning restore CS8777