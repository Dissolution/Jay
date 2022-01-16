using System.Runtime.CompilerServices;

namespace Jay.Reflection.Building.Deconstruction;

public sealed class ByteBuffer
{
    private readonly byte[] _buffer;
    private int _position;

    public int Position => _position;
    public int Length => _buffer.Length;

    public ByteBuffer(byte[] buffer)
    {
        _buffer = buffer;
        _position = 0;
    }

    public Result TryReadBytes(int count, out Span<byte> bytes)
    {
        int i = _position;
        int len = i + count;
        if (len > _buffer.Length)
        {
            bytes = default;
            return new ArgumentOutOfRangeException(nameof(count), count,
                $"Tried to read {count} bytes, only {_buffer.Length - i} were available");
        }
        bytes = _buffer.AsSpan(i, count);
        _position = len;
        return true;
    }

    public Span<byte> ReadBytes(int count)
    {
        TryReadBytes(count, out var bytes).ThrowIfFailed();
        return bytes;
    }

    public Result TryRead<T>(out T value)
        where T : unmanaged
    {
        int i = _position;
        int size = Unsafe.SizeOf<T>();
        int len = i + size;
        if (len > _buffer.Length)
        {
            value = default;
            return new ArgumentException($"Cannot read a {typeof(T)} value ({size} bytes), only {_buffer.Length - i} were available");
        }
        value = Unsafe.ReadUnaligned<T>(ref _buffer[i]);
        _position = len;
        return true;
    }

    public T Read<T>()
        where T : unmanaged
    {
        TryRead<T>(out var value).ThrowIfFailed();
        return value;
    }

    public byte ReadByte()
    {
        if (_position >= _buffer.Length)
            throw new InvalidOperationException("Cannot read a byte");
        return _buffer[_position++];
    }
   
    
}