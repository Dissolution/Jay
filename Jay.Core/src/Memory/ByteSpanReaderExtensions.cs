using Jay.Reflection;
using Jay.Utilities;

namespace Jay.Memory;

/// <summary>
/// Extensions on <see cref="SpanReader{T}">SpanReader&lt;byte&gt;</see>
/// </summary>
public static class ByteSpanReaderExtensions
{
    public static Result TryPeek<T>(
        this ref SpanReader<byte> byteReader,
        out T value)
        where T : unmanaged
    {
        if (byteReader.TryPeek(Scary.SizeOf<T>(), out var bytes))
        {
            value = Scary.ReadUnaligned<T>(in bytes.GetPinnableReference());
            return true;
        }
        value = default;
        return new InvalidOperationException($"Cannot peek for a {typeof(T).NameOf()}: Only {byteReader.UnreadCount} bytes remain");
    }
    
    public static Result TryTake<T>(
        this ref SpanReader<byte> byteReader,
        out T value)
        where T : unmanaged
    {
        if (byteReader.TryTake(Scary.SizeOf<T>(), out var bytes))
        {
            value = Scary.ReadUnaligned<T>(in bytes.GetPinnableReference());
            return true;
        }
        value = default;
        return new InvalidOperationException($"Cannot take a {typeof(T).NameOf()}: Only {byteReader.UnreadCount} bytes remain");
    }

    public static T Take<T>(this ref SpanReader<byte> byteReader)
        where T : unmanaged
        => TryTake<T>(ref byteReader, out T value)
            .WithValue(value)
            .OkValueOrThrowError();
}