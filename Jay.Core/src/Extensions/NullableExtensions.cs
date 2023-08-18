namespace Jay.Extensions;

public static class NullableExtensions
{
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool TryGetValue<T>(this T? nullable, out T value)
        where T : struct
    {
        value = nullable.GetValueOrDefault();
        return nullable.HasValue;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Option<T> ToOption<T>(this T? nullable)
        where T : struct
    {
        if (nullable.HasValue)
        {
            return Option<T>.Some(nullable.Value);
        }
        return Option<T>.None;
    }
}