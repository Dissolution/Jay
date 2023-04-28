// ReSharper disable UnusedTypeParameter

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

#if NET7_0_OR_GREATER
    public readonly struct IsSpanParsable<T> where T : ISpanParsable<T>
    {
    }

    public readonly struct IsNumberBase<T> where T : INumberBase<T>
    {
    }
#endif
}