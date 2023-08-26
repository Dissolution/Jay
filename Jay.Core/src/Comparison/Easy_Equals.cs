﻿// ReSharper disable InvokeAsExtensionMethod

#if !NETCOREAPP3_1_OR_GREATER
#pragma warning disable CS8604
#endif

using System.Collections;
using System.Reflection;
using Jay.Collections;

// ReSharper disable once CheckNamespace
namespace Jay;

public sealed partial class Easy :
    IEqualityComparer<object?>,
    IEqualityComparer
{
    private static readonly ConcurrentTypeMap<IEqualityComparer> _equalityComparers = new();

    internal static IEqualityComparer FindEqualityComparer(Type type)
    {
        return typeof(IEqualityComparer<>)
            .MakeGenericType(type)
            .GetProperty("Default", BindingFlags.Public | BindingFlags.Static)
            .ThrowIfNull()
            .GetValue(null)
            .MustBe<IEqualityComparer>();
    }

    internal static IEqualityComparer<T> FindEqualityComparer<T>()
        => EqualityComparer<T>.Default;

    public static IEqualityComparer GetDefaultEqualityComparer(Type type)
    {
        return _equalityComparers.GetOrAdd(type, FindEqualityComparer);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool Equal<T>(T? left, T? right)
    {
        return EqualityComparer<T>.Default.Equals(left, right);
    }

    bool IEqualityComparer<object?>.Equals(object? x, object? y) => ObjEqual(x, y);
    bool IEqualityComparer.Equals(object? x, object? y) => ObjEqual(x, y);

    public static bool ObjEqual(object? left, object? right)
    {
        if (ReferenceEquals(left, right)) return true;
        if (left is not null)
            return GetDefaultEqualityComparer(left.GetType()).Equals(left, right);
        if (right is not null)
            return GetDefaultEqualityComparer(right.GetType()).Equals(right, left);
        return true; // both are null
    }

#if NET6_0_OR_GREATER
    public static bool SeqEqual<T>(T[]? left, T[]? right)
    {
        return MemoryExtensions.SequenceEqual<T>(left, right);
    }
    public static bool SeqEqual<T>(T[]? left, Span<T> right)
    {
        return MemoryExtensions.SequenceEqual<T>(left, right);
    }
    public static bool SeqEqual<T>(T[]? left, ReadOnlySpan<T> right)
    {
        return MemoryExtensions.SequenceEqual<T>(left, right);
    }
    public static bool SeqEqual<T>(Span<T> left, T[]? right)
    {
        return MemoryExtensions.SequenceEqual<T>(left, right);
    }
    public static bool SeqEqual<T>(Span<T> left, Span<T> right)
    {
        return MemoryExtensions.SequenceEqual<T>(left, right);
    }
    public static bool SeqEqual<T>(Span<T> left, ReadOnlySpan<T> right)
    {
        return MemoryExtensions.SequenceEqual<T>(left, right);
    }
    public static bool SeqEqual<T>(ReadOnlySpan<T> left, T[]? right)
    {
        return MemoryExtensions.SequenceEqual<T>(left, right);
    }
    public static bool SeqEqual<T>(ReadOnlySpan<T> left, Span<T> right)
    {
        return MemoryExtensions.SequenceEqual<T>(left, right);
    }
    public static bool SeqEqual<T>(ReadOnlySpan<T> left, ReadOnlySpan<T> right)
    {
        return MemoryExtensions.SequenceEqual<T>(left, right);
    }
#else
    public static bool SeqEqual<T>(T[]? left, T[]? right)
    {
        return SpanExtensions.SequenceEqual<T>(left, right);
    }
    public static bool SeqEqual<T>(T[]? left, Span<T> right)
    {
        return SpanExtensions.SequenceEqual<T>(left, right);
    }
    public static bool SeqEqual<T>(T[]? left, ReadOnlySpan<T> right)
    {
        return SpanExtensions.SequenceEqual<T>(left, right);
    }
    public static bool SeqEqual<T>(Span<T> left, T[]? right)
    {
        return SpanExtensions.SequenceEqual<T>(left, right);
    }
    public static bool SeqEqual<T>(Span<T> left, Span<T> right)
    {
        return SpanExtensions.SequenceEqual<T>(left, right);
    }
    public static bool SeqEqual<T>(Span<T> left, ReadOnlySpan<T> right)
    {
        return SpanExtensions.SequenceEqual<T>(left, right);
    }
    public static bool SeqEqual<T>(ReadOnlySpan<T> left, T[]? right)
    {
        return SpanExtensions.SequenceEqual<T>(left, right);
    }
    public static bool SeqEqual<T>(ReadOnlySpan<T> left, Span<T> right)
    {
        return SpanExtensions.SequenceEqual<T>(left, right);
    }
    public static bool SeqEqual<T>(ReadOnlySpan<T> left, ReadOnlySpan<T> right)
    {
        return SpanExtensions.SequenceEqual<T>(left, right);
    }

#endif

#region Text
    public static bool TextEqual(string? left, string? right)
    {
        return string.Equals(left, right);
    }
    
    public static bool TextEqual(string? left, ReadOnlySpan<char> right)
    {
        return MemoryExtensions.SequenceEqual<char>(left.AsSpan(), right);
    }
    
    public static bool TextEqual(string? left, char[]? right)
    {
        return MemoryExtensions.SequenceEqual<char>(left.AsSpan(), right);
    }
    
    public static bool TextEqual(ReadOnlySpan<char> left, string? right)
    {
        return MemoryExtensions.SequenceEqual<char>(left, right.AsSpan());
    }
    
    public static bool TextEqual(ReadOnlySpan<char> left, ReadOnlySpan<char> right)
    {
        return MemoryExtensions.SequenceEqual<char>(left, right);
    }
    
    public static bool TextEqual(ReadOnlySpan<char> left, char[]? right)
    {
        return MemoryExtensions.SequenceEqual<char>(left, right);
    }
    
    public static bool TextEqual(char[]? left, string? right)
    {
        return MemoryExtensions.SequenceEqual<char>(left, right.AsSpan());
    }
    
    public static bool TextEqual(char[]? left, ReadOnlySpan<char> right)
    {
        return MemoryExtensions.SequenceEqual<char>(left, right);
    }
    
    public static bool TextEqual(char[]? left, char[]? right)
    {
        return MemoryExtensions.SequenceEqual<char>(left, right);
    }
    
    
    public static bool TextEqual(string? left, string? right, StringComparison comparison)
    {
        return string.Equals(left, right, comparison);
    }
    
    public static bool TextEqual(string? left, ReadOnlySpan<char> right, StringComparison comparison)
    {
        return MemoryExtensions.Equals(left.AsSpan(), right, comparison);
    }
    
    public static bool TextEqual(string? left, char[]? right, StringComparison comparison)
    {
        return MemoryExtensions.Equals(left.AsSpan(), right, comparison);
    }
    
    public static bool TextEqual(ReadOnlySpan<char> left, string? right, StringComparison comparison)
    {
        return MemoryExtensions.Equals(left, right.AsSpan(), comparison);
    }
    
    public static bool TextEqual(ReadOnlySpan<char> left, ReadOnlySpan<char> right, StringComparison comparison)
    {
        return MemoryExtensions.Equals(left, right, comparison);
    }
    
    public static bool TextEqual(ReadOnlySpan<char> left, char[]? right, StringComparison comparison)
    {
        return MemoryExtensions.Equals(left, right, comparison);
    }
    
    public static bool TextEqual(char[]? left, string? right, StringComparison comparison)
    {
        return MemoryExtensions.Equals(left, right.AsSpan(), comparison);
    }
    
    public static bool TextEqual(char[]? left, ReadOnlySpan<char> right, StringComparison comparison)
    {
        return MemoryExtensions.Equals(left, right, comparison);
    }
    
    public static bool TextEqual(char[]? left, char[]? right, StringComparison comparison)
    {
        return MemoryExtensions.Equals(left, right, comparison);
    }
#endregion

    int IEqualityComparer<object?>.GetHashCode(object? obj) => GetHashCode(obj);

    int IEqualityComparer.GetHashCode(object? obj) => GetHashCode(obj);

    public static int GetHashCode(object? obj)
    {
        if (obj is null) return 0;
        return GetDefaultEqualityComparer(obj.GetType())
            .GetHashCode(obj);
    }
}