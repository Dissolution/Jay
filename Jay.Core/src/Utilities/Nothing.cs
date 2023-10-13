using System.Runtime.InteropServices;

namespace Jay.Utilities;

public static class NothingExtensions
{
    public static Func<Nothing> ToFunc(this Action action)
    {
        return () =>
        {
            action();
            return default(Nothing);
        };
    }

    public static Func<T1, Nothing> ToFunc<T1>(this Action<T1> action)
    {
        return v1 =>
        {
            action(v1);
            return default(Nothing);
        };
    }

    public static Func<T1, T2, Nothing> ToFunc<T1, T2>(this Action<T1, T2> action)
    {
        return (v1, v2) =>
        {
            action(v1, v2);
            return default(Nothing);
        };
    }

    public static Func<T1, T2, T3, Nothing> ToFunc<T1, T2, T3>(this Action<T1, T2, T3> action)
    {
        return (v1, v2, v3) =>
        {
            action(v1, v2, v3);
            return default(Nothing);
        };
    }

    public static Func<T1, T2, T3, T4, Nothing> ToFunc<T1, T2, T3, T4>(this Action<T1, T2, T3, T4> action)
    {
        return (v1, v2, v3, v4) =>
        {
            action(v1, v2, v3, v4);
            return default(Nothing);
        };
    }

    public static Func<T1, T2, T3, T4, T5, Nothing> ToFunc<T1, T2, T3, T4, T5>(this Action<T1, T2, T3, T4, T5> action)
    {
        return (v1, v2, v3, v4, v5) =>
        {
            action(v1, v2, v3, v4, v5);
            return default(Nothing);
        };
    }

    public static Func<T1, T2, T3, T4, T5, T6, Nothing> ToFunc<T1, T2, T3, T4, T5, T6>(this Action<T1, T2, T3, T4, T5, T6> action)
    {
        return (v1, v2, v3, v4, v5, v6) =>
        {
            action(v1, v2, v3, v4, v5, v6);
            return default(Nothing);
        };
    }

    public static Func<T1, T2, T3, T4, T5, T6, T7, Nothing> ToFunc<T1, T2, T3, T4, T5, T6, T7>(this Action<T1, T2, T3, T4, T5, T6, T7> action)
    {
        return (v1, v2, v3, v4, v5, v6, v7) =>
        {
            action(v1, v2, v3, v4, v5, v6, v7);
            return default(Nothing);
        };
    }

    public static Func<T1, T2, T3, T4, T5, T6, T7, T8, Nothing> ToFunc<T1, T2, T3, T4, T5, T6, T7, T8>(this Action<T1, T2, T3, T4, T5, T6, T7, T8> action)
    {
        return (v1, v2, v3, v4, v5, v6, v7, v8) =>
        {
            action(v1, v2, v3, v4, v5, v6, v7, v8);
            return default(Nothing);
        };
    }
}

/// <summary>
/// A representation of a <c>void</c> value<br/>
/// Not <c>null</c>, but literally <c>Nothing</c>
/// Create with <c>default</c>
/// </summary>
[StructLayout(LayoutKind.Auto, Size = 0)]
public readonly struct Nothing :
#if NET7_0_OR_GREATER
    IEqualityOperators<Nothing, Nothing, bool>,
    IEqualityOperators<Nothing, object, bool>,
#endif
    IEquatable<Nothing>
{
    public static bool operator ==(Nothing a, Nothing z) => true;
    public static bool operator !=(Nothing a, Nothing z) => false;
    public static bool operator ==(Nothing _, object? obj) => obj is Nothing;
    public static bool operator !=(Nothing _, object? obj) => obj is not Nothing;
    public static bool operator ==(object? obj, Nothing _) => obj is Nothing;
    public static bool operator !=(object? obj, Nothing _) => obj is not Nothing;

    public bool Equals(Nothing _) => true;

    public override bool Equals(object? obj) => obj is Nothing;

    public override int GetHashCode() => 0;

    public override string ToString() => nameof(Nothing);
}