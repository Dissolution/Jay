using System.Buffers;

namespace Jay.Text;

public static class TextPool
{
    private static readonly ArrayPool<char> _pool = ArrayPool<char>.Shared;
    
    /// <summary>
    /// The minimum possible capacity of any builder<br/>
    /// 1024 (0x400)
    /// </summary>
    /// <remarks>
    /// We want to have a fairly large minimum capacity to minimize resizes
    /// </remarks>
    public const int MINIMUM_CAPACITY = 1024;
    
    /// <summary>
    /// The maximum possible capacity of any builder<br/>
    /// 1,073,741,791 (0x3FFFFFDF)
    /// </summary>
    /// <remarks>
    /// This is equal to <c>string.MaxLength</c> (which is lower than <c>Array.MaxLength</c>)<br/>
    /// and thus the maximum possible <see cref="string"/> length that a builder could return
    /// </remarks>
    public const int MAXIMUM_CAPACITY = 0x3FFFFFDF;

    public static char[] Rent() => _pool.Rent(MINIMUM_CAPACITY);

    public static char[] Rent(int minCapacity)
    {
        var newCapacity = Math.Min(Math.Max(MINIMUM_CAPACITY, minCapacity), MAXIMUM_CAPACITY);
        return _pool.Rent(newCapacity);
    }

    public static char[] Rent(int literalLength, int formattedCount)
    {
        var newCapacity = (literalLength + (formattedCount * 16)).Clamp(MINIMUM_CAPACITY, MAXIMUM_CAPACITY);
        return _pool.Rent(newCapacity);
    }
    
    public static char[] RentGrowBy(int curCapacity, int growCount)
    {
        var newCapacity = ((curCapacity + growCount) * 2).Clamp(MINIMUM_CAPACITY, MAXIMUM_CAPACITY);
        return _pool.Rent(newCapacity);
    }
    
    public static char[] RentGrowTo(int curCapacity, int minCapacity)
    {
        var newCapacity = (Math.Max(curCapacity, minCapacity) * 2).Clamp(MINIMUM_CAPACITY, MAXIMUM_CAPACITY);
        return _pool.Rent(newCapacity);
    }

    public static void Return(char[]? returnArray)
    {
        if (returnArray is not null)
        {
            _pool.Return(returnArray, true);
        }
    }
}