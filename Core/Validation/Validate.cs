using System.Runtime.CompilerServices;

namespace Jay.Validation;

public static class Validate
{
    public static void Index(int index, int length, [CallerArgumentExpression("index")] string? indexArgName = null)
    {
        if ((uint)index >= (uint)length)
        {
            throw new ArgumentOutOfRangeException(indexArgName, index, $"Index must be between 0 and {length - 1}");
        }
    }
    public static void Insert(int index, int length, [CallerArgumentExpression("index")] string? indexArgName = null)
    {
        if ((uint)index > (uint)length)
        {
            throw new ArgumentOutOfRangeException(indexArgName, index, $"Insert index must be between 0 and {length}");
        }
    }
}