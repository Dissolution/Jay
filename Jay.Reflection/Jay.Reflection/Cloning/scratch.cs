using Jay.Collections;
using Jay.Reflection.Searching;

namespace Jay.Reflection.Cloning;

/// <summary>
/// Deep-clones the given <paramref name="value"/>
/// </summary>
/// <typeparam name="T">
/// The <see cref="Type"/> of value to deep clone
/// </typeparam>
/// <param name="value">
/// The <typeparamref name="T"/> value to be cloned
/// </param>
[return: NotNullIfNotNull(nameof(value))]
public delegate T? DeepClone<T>(T? value);

public static class Cloner
{
    private static readonly ConcurrentTypeMap<Delegate> _deepCloneCache;
    private static readonly MethodInfo _getDeepCloneDelegateMethod;

    static Cloner()
    {
        _getDeepCloneDelegateMethod = MemberSearch
            .One<MethodInfo>(typeof(Cloner), nameof(GetDeepCloneDelegate));

        _deepCloneCache = new()
        {
            (typeof(string), (DeepClone<string>)UnmanagedClone),
            (typeof(bool), (DeepClone<bool>)UnmanagedClone),
            (typeof(byte), (DeepClone<byte>)UnmanagedClone),
            (typeof(sbyte), (DeepClone<sbyte>)UnmanagedClone),
            (typeof(short), (DeepClone<short>)UnmanagedClone),
            (typeof(ushort), (DeepClone<ushort>)UnmanagedClone),
            (typeof(int), (DeepClone<int>)UnmanagedClone),
            (typeof(uint), (DeepClone<uint>)UnmanagedClone),
            (typeof(long), (DeepClone<long>)UnmanagedClone),
            (typeof(ulong), (DeepClone<ulong>)UnmanagedClone),
            (typeof(float), (DeepClone<float>)UnmanagedClone),
            (typeof(double), (DeepClone<double>)UnmanagedClone),
            (typeof(decimal), (DeepClone<decimal>)UnmanagedClone),
        };
         }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static T UnmanagedClone<T>(T value) => value;
    
    private static Delegate CreateDeepCloneDelegate(Type type)
    {
        
        
        
        throw new NotImplementedException();
    }

    internal static Delegate GetDeepCloneDelegate(Type type)
    {
        return _deepCloneCache.GetOrAdd(type, CreateDeepCloneDelegate);
    }

    public static DeepClone<T> GetDeepCloneDelegate<T>()
    {
        var del = GetDeepCloneDelegate(typeof(T));
        if (del is DeepClone<T> deepClone)
            return deepClone;
        throw new InvalidOperationException();
    }

    [return: NotNullIfNotNull(nameof(value))]
    public static T? DeepClone<T>(this T? value)
    {
        return GetDeepCloneDelegate<T>().Invoke(value);
    }
}