namespace Jay.Utilities;

public static class Easy
{
    #if NETSTANDARD2_0 || NETSTANDARD2_1
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool Equals<T>(T? first, T? second)
    {
        return EqualityComparer<T>.Default.Equals(first!, second!);
    }
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool Equals<T>(T[]? first, T[]? second)
    {
        return Equals((ReadOnlySpan<T>)first, (ReadOnlySpan<T>)second);
    }
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool Equals<T>(T[]? first, Span<T> second)
    {
        return Equals((ReadOnlySpan<T>)first, (ReadOnlySpan<T>)second);
    }
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool Equals<T>(T[]? first, ReadOnlySpan<T> second)
    {
        return Equals((ReadOnlySpan<T>)first, (ReadOnlySpan<T>)second);
    }
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool Equals<T>(Span<T> first, T[]? second)
    {
        return Equals((ReadOnlySpan<T>)first, (ReadOnlySpan<T>)second);
    }
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool Equals<T>(Span<T> first, Span<T> second)
    {
        return Equals((ReadOnlySpan<T>)first, (ReadOnlySpan<T>)second);
    }
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool Equals<T>(Span<T> first, ReadOnlySpan<T> second)
    {
        return Equals((ReadOnlySpan<T>)first, (ReadOnlySpan<T>)second);
    }
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool Equals<T>(ReadOnlySpan<T> first, T[]? second)
    {
        return Equals((ReadOnlySpan<T>)first, (ReadOnlySpan<T>)second);
    }
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool Equals<T>(ReadOnlySpan<T> first, Span<T> second)
    {
        return Equals((ReadOnlySpan<T>)first, (ReadOnlySpan<T>)second);
    }
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool Equals<T>(ReadOnlySpan<T> first, ReadOnlySpan<T> second)
    {
        var firstLen = first.Length;
        if (second.Length != firstLen) return false;
        for (var i = 0; i < firstLen; i++)
        {
            if (!EqualityComparer<T>.Default.Equals(first[i]!, second[i]!))
                return false;
        }
        return true;
    }
    #else
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool Equal<T>(T? first, T? second)
    {
        return EqualityComparer<T>.Default.Equals(first, second);
    }
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool Equal<T>(T[]? first, T[]? second)
    {
        return MemoryExtensions.SequenceEqual<T>(first.AsSpan(), second.AsSpan());
    }
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool Equal<T>(T[]? first, Span<T> second)
    {
        return MemoryExtensions.SequenceEqual<T>(first.AsSpan(), second);
    }
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool Equal<T>(T[]? first, ReadOnlySpan<T> second)
    {
        return MemoryExtensions.SequenceEqual<T>(first.AsSpan(), second);
    }
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool Equal<T>(Span<T> first, T[]? second)
    {
        return MemoryExtensions.SequenceEqual<T>(first, second.AsSpan());
    }
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool Equal<T>(Span<T> first, Span<T> second)
    {
        return MemoryExtensions.SequenceEqual<T>(first, second);
    }
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool Equal<T>(Span<T> first, ReadOnlySpan<T> second)
    {
        return MemoryExtensions.SequenceEqual<T>(first, second);
    }
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool Equal<T>(ReadOnlySpan<T> first, T[]? second)
    {
        return MemoryExtensions.SequenceEqual<T>(first, second.AsSpan());
    }
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool Equal<T>(ReadOnlySpan<T> first, Span<T> second)
    {
        return MemoryExtensions.SequenceEqual<T>(first, second);
    }
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool Equal<T>(ReadOnlySpan<T> first, ReadOnlySpan<T> second)
    {
        return MemoryExtensions.SequenceEqual<T>(first, second);
    }
#endif
    
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void CopyTo<T>(T[]? source, T[]? dest)
    {
        source.AsSpan().CopyTo(dest.AsSpan());
    }
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void CopyTo<T>(T[]? source, Span<T> dest)
    {
        source.AsSpan().CopyTo(dest);
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