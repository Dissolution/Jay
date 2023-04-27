// ReSharper disable UnusedTypeParameter

using Jay.Parsing;
using System.Numerics;

namespace Jay.Utilities;

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

    public readonly struct IsNumberParsable<T> where T : INumberParsable<T>
    {
    }

#if NET7_0_OR_GREATER
    public readonly struct IsNumberBase<T> where T : INumberBase<T>
    {
    }
#endif
}