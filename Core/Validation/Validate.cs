using System.Runtime.CompilerServices;

namespace Jay.Validation;

public static class Validate
{
    public static void Index(int available, int index, 
        [CallerArgumentExpression(nameof(index))] string? indexName = null)
    {
        if ((uint)index < available) return;
        throw new ArgumentOutOfRangeException(indexName, index,
            $"{indexName} {index} must be between 0 and {available - 1}");
    }

    public static void Insert(int available, int index, 
        [CallerArgumentExpression(nameof(index))] string? indexName = null)
    {
        if ((uint)index <= available) return;
        throw new ArgumentOutOfRangeException(indexName, index,
            $"Insert {indexName} {index} must be between 0 and {available}");
    }
}