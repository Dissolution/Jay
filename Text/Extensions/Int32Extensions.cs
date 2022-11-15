namespace Jay.Text.Extensions;

public static class Int32Extensions
{
    public static int Clamp(this int value, int min, int max)
    {
#if NETSTANDARD2_0_OR_GREATER
        if (value < min) return min;
        if (value > max) return max;
        return value;
#else
        return Math.Clamp(value, min, max);
#endif
    }
}