// ReSharper disable InvokeAsExtensionMethod

#if !NETCOREAPP3_1_OR_GREATER
#pragma warning disable CS8604
#endif

// ReSharper disable once CheckNamespace
namespace Jay;

public sealed partial class Easy
{
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void CopyTo<T>(T[]? source, T[]? dest)
    {
        MemoryExtensions.CopyTo<T>(source, dest.AsSpan());
    }
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void CopyTo<T>(T[]? source, Span<T> dest)
    {
        MemoryExtensions.CopyTo<T>(source, dest);
    }
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void CopyTo<T>(Span<T> source, T[]? dest)
    {
        source.CopyTo(dest.AsSpan());
    }
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void CopyTo<T>(Span<T> source, Span<T> dest)
    {
        source.CopyTo(dest);
    }
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void CopyTo<T>(ReadOnlySpan<T> source, T[]? dest)
    {
        source.CopyTo(dest.AsSpan());
    }
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void CopyTo<T>(ReadOnlySpan<T> source, Span<T> dest)
    {
        source.CopyTo(dest);
    }
}