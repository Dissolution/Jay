// ReSharper disable EntityNameCapturedOnly.Global
namespace Jay.Extensions;

public static class ObjectExtensions
{
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool Is<T>(this object? input, [NotNullWhen(true)] out T? output)
    {
        if (input is T)
        {
            output = (T)input;
            return true;
        }
        output = default;
        return false;
    }

    /// <summary>
    /// If this <see cref="object"/> can be a <typeparamref name="T"/> value,
    /// cast it to that value and return <c>true</c>.
    /// </summary>
    /// <param name="obj"></param>
    /// <param name="value"></param>
    /// <typeparam name="T"></typeparam>
    /// <returns></returns>
    /// <remarks>
    /// This differs from the <c>is</c> keyword in that,
    /// if <paramref name="obj"/> is <c>null</c> and <typeparamref name="T"/> can contain <c>null</c>,
    /// returning <c>null</c> as <paramref name="value"/> is valid.
    /// </remarks>
    public static bool CanBe<T>(this object? obj, out T? value)
    {
        if (obj is T)
        {
            value = (T)obj;
            return true;
        }
        value = default;
        return obj is null && typeof(T).CanContainNull();
    }
}