// ReSharper disable InvokeAsExtensionMethod

#if !NETCOREAPP3_1_OR_GREATER
#pragma warning disable CS8604
#endif

using System.Collections;
using System.Reflection;
using Jay.Collections;

// ReSharper disable once CheckNamespace
namespace Jay;

public sealed partial class Easy :
    IComparer<object?>,
    IComparer
{
    private static readonly ConcurrentTypeMap<IComparer> _comparers = new();

    internal static IComparer FindComparer(Type type)
    {
        return typeof(IComparer<>)
            .MakeGenericType(type)
            .GetProperty("Default", BindingFlags.Public | BindingFlags.Static)
            .ThrowIfNull()
            .GetValue(null)
            .AsValid<IComparer>();
    }

    internal static IComparer<T> FindComparer<T>()
        => Comparer<T>.Default;

    public static IComparer GetDefaultComparer(Type type)
    {
        return _comparers.GetOrAdd(type, FindComparer);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static int Compare<T>(T? left, T? right)
    {
        return Comparer<T>.Default.Compare(left, right);
    }


    int IComparer<object?>.Compare(object? x, object? y) => ObjCompare(x, y);
    int IComparer.Compare(object? x, object? y) => ObjCompare(x, y);

    public static int ObjCompare(object? left, object? right)
    {
        if (ReferenceEquals(left, right)) return 0;
        if (left is not null)
        {
            return GetDefaultComparer(left.GetType()).Compare(left, right);
        }
        if (right is not null)
        {
            return GetDefaultComparer(right.GetType()).Compare(left, right);
        }
        return 0; // they're both null
    }
}