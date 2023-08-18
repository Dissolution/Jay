// ReSharper disable UnusedTypeParameter

#if NET7_0_OR_GREATER
using System.Numerics;
#endif

namespace Jay.Utilities;

/*
public static T DoThing<T>(T value, IsStruct<T> _ = default) where T : struct
{
    throw new NotImplementedException();
}
public static T DoThing<T>(T value, IsClass<T> _ = default) where T : class
{
    throw new NotImplementedException();
}
*/

/// <summary>
/// Constraints are used to allow coexisting constrained generic methods<br />
/// Normally, if you had:<br />
/// public T DoThing&lt;T&gt;(T value) where T : struct;<br />
/// public T DoThing&lt;T&gt;(T value) where T : class;<br />
/// The compiler will have an error: 'member with the same signature is already declared'<br />
/// <br />
/// We can use <see cref="Constraints" /> to fix it:<br />
/// public static T DoThing&lt;T&gt;(T value, IsStruct&lt;T&gt; _ = default) where T : struct<br />
/// public static T DoThing&lt;T&gt;(T value, IsClass&lt;T&gt; _ = default) where T : class<br />
/// </summary>
public static class Constraints
{
    public readonly struct IsNew<T> where T : new()
    {
    }

    public readonly struct IsDisposable<T> where T : IDisposable
    {
    }

    public readonly struct IsClass<T> where T : class
    {
    }

    public readonly struct IsStruct<T> where T : struct
    {
    }

    public readonly struct IsUnmanaged<T> where T : unmanaged
    {
    }

    public readonly struct IsNewDisposable<T> where T : IDisposable, new()
    {
    }

#if NET7_0_OR_GREATER
    public readonly struct IsSpanParsable<T> where T : ISpanParsable<T>
    {
    }

    public readonly struct IsNumberBase<T> where T : INumberBase<T>
    {
    }
#endif
}