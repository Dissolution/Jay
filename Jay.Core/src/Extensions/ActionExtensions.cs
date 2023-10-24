using Jay.Utilities;

namespace Jay.Extensions;

public static class ActionExtensions
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