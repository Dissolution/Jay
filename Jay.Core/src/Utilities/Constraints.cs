// ReSharper disable UnusedTypeParameter
namespace Jay.Utilities;

/// <summary>
/// Constraints are added to generic methods so that generic type constraints can co-exist without compiler error<br />
/// <br/>
/// Usually, if you had:<br />
/// <c>public T DoThing&lt;T&gt;(T value) where T : struct;</c><br />
/// <c>public T DoThing&lt;T&gt;(T value) where T : class;</c><br />
/// <br/>
/// The compiler will have an error: 'member with the same signature is already declared'<br />
/// <br />
/// You can use <see cref="Constraints"/> to fix it:<br />
/// <c>public static T DoThing&lt;T&gt;(T value, IsStruct&lt;T&gt; _ = default) where T : struct</c><br />
/// <c>public static T DoThing&lt;T&gt;(T value, IsClass&lt;T&gt; _ = default) where T : class</c><br />
/// <br/>
/// </summary>
public static class Constraints
{
    /// <summary>
    /// Constrains <typeparamref name="T"/> to only non-<c>static</c> types that have a default constructor
    /// </summary>
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