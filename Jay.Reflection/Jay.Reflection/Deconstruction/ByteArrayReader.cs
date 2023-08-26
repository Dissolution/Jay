using Jay.Reflection.Exceptions;
using Jay.Utilities;

namespace Jay.Reflection.Deconstruction;

internal sealed class ByteArrayReader
{
    private readonly byte[] _bytes;
    private int _position;

    public int Position
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        get => _position;
    }

    public int AvailableByteCount
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        get => _bytes.Length - _position;
    }

    public ReadOnlySpan<byte> AvailableBytes
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        get => _bytes.AsSpan(_position);
    }

    public ByteArrayReader(byte[] bytes)
    {
        _bytes = bytes;
        _position = 0;
    }
    
    public bool TryPeek(out byte b)
    {
        var position = _position;
        var bytes = _bytes;

        if (position < 0 || position >= bytes.Length)
        {
            b = default;
            return false;
        }
        b = bytes[position];
        return true;
    }
    
    public bool TryPeek<T>(out T value)
        where T : unmanaged
    {
        var position = _position;
        var bytes = _bytes;

        var size = Scary.SizeOf<T>();
        if (position < 0 || (position + size) > bytes.Length)
        {
            value = default;
            return false;
        }

        value = Scary.ReadUnaligned<T>(bytes.AsSpan(position, size));
        return true;
    }
    
    public bool TryPeek(Span<byte> buffer)
    {
        var position = _position;
        var bytes = _bytes;

        var size = buffer.Length;
        if (position < 0 || (position + size) > bytes.Length)
        {
            return false;
        }

        Easy.CopyTo(bytes.AsSpan(position, size), buffer);
        return true;
    }
    
    public bool TryRead(out byte b)
    {
        var position = _position;
        var bytes = _bytes;

        if (position < 0 || position >= bytes.Length)
        {
            b = default;
            return false;
        }
        b = bytes[position];
        _position = position + 1;
        return true;
    }
    
    
    public bool TryRead<T>(out T value)
        where T : unmanaged
    {
        var position = _position;
        var bytes = _bytes;

        var size = Scary.SizeOf<T>();
        if (position < 0 || (position + size) > bytes.Length)
        {
            value = default;
            return false;
        }

        value = Scary.ReadUnaligned<T>(bytes.AsSpan(position, size));
        _position = position + size;
        return true;
    }
    
    public bool TryRead(Span<byte> buffer)
    {
        var position = _position;
        var bytes = _bytes;

        var size = buffer.Length;
        if (position < 0 || (position + size) > bytes.Length)
        {
            return false;
        }

        Easy.CopyTo(bytes.AsSpan(position, size), buffer);
        _position = position + size;
        return true;
    }

    public byte Read()
    {
        if (TryRead(out byte b))
            return b;
        throw new InvalidOperationException("Could not read a byte value");
    }
    
    public T Read<T>()
        where T : unmanaged
    {
        if (TryRead<T>(out T value))
            return value;
        throw Except.New<InvalidOperationException>($"Could not read a {typeof(T)} value");
    }
}