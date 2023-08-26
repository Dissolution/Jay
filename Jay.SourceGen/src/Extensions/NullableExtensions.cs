namespace Jay.CodeGen.Extensions;

public static class NullableExtensions
{
    public static bool TryGetValue<T>(this Nullable<T> nullable, out T value)
    where T : struct
    {
        value = nullable.GetValueOrDefault();
        return nullable.HasValue;
    }
}