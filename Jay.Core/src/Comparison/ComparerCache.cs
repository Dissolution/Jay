// ReSharper disable InvokeAsExtensionMethod

#if !NETCOREAPP3_1_OR_GREATER
#pragma warning disable CS8604
#endif

using System.Collections;
using System.Reflection;
using Jay.Collections;

namespace Jay.Comparison;

public sealed class ComparerCache : IComparer<object?>, IComparer
{
    private static readonly ConcurrentTypeMap<IComparer> _comparers = new();

    private static IComparer FindComparer(Type type)
    {
        return typeof(IComparer<>)
            .MakeGenericType(type)
            .GetProperty("Default", BindingFlags.Public | BindingFlags.Static)
            .ThrowIfNull()
            .GetValue(null)
            .AsValid<IComparer>();
    }
    
    public static IComparer Default(Type type)
    {
        return _comparers
            .GetOrAdd(type, static t => FindComparer(t));
    }

    public static IComparer<T> Default<T>()
    {
        return _comparers
            .GetOrAdd<T>(static _ => Comparer<T>.Default)
            .AsValid<IComparer<T>>();
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static int Compare<T>(T? left, T? right)
    {
        return Comparer<T>.Default.Compare(left, right);
    }
    
    public static int Compare(object? left, object? right)
    {
        if (ReferenceEquals(left, right)) return 0;
        if (left is not null)
        {
            return Default(left.GetType()).Compare(left, right);
        }
        // Right cannot be null, we checked refequals
        return Default(right!.GetType()).Compare(left, right);
    }

    public static IComparer<T> Create<T>(Comparison<T> compare) => Comparer<T>.Create(compare);
    
    int IComparer<object?>.Compare(object? x, object? y) => Compare(x, y);
    int IComparer.Compare(object? x, object? y) => Compare(x, y);
}