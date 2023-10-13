using System.Buffers.Text;
using Jay.Utilities;

namespace Jay.Memory;

/// <summary>
/// Extensions on <see cref="SpanReader{T}">SpanReader&lt;byte&gt;</see>
/// </summary>
public static class ByteSpanReaderExtensions
{
    public static bool TryPeek<T>(
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
        return false;
    }
    
    public static bool TryRead<T>(
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
        return false;
    }

    public static T Read<T>(this ref SpanReader<byte> byteReader)
        where T : unmanaged
    {
        if (TryRead<T>(ref byteReader, out T value))
            return value;

        throw new InvalidOperationException();
    }
    
    public static bool TryReadInto(
        this ref SpanReader<byte> byteReader,
        Span<byte> buffer)
    {
        if (byteReader.TryTake(buffer.Length, out var taken))
        {
            Easy.CopyTo(taken, buffer);
            return true;
        }
        return false;
    }
}