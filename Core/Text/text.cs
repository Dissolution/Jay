using System.Runtime.CompilerServices;
using Jay.Reflection;
// ReSharper disable InconsistentNaming

namespace Jay.Text;

public readonly ref struct text
{
    public static implicit operator text(in char ch) => new text(ch);
    public static implicit operator text(string? str) => new text(str);
    public static implicit operator text(ReadOnlySpan<char> chars) => new text(chars);
    public static implicit operator text(char[]? chars) => new text(chars);

    public static implicit operator ReadOnlySpan<char>(text text) => text.AsSpan();
    public static explicit operator string(text text) => text.ToString();

    public static bool operator ==(text left, text right) => left.Equals(right);
    public static bool operator !=(text left, text right) => !left.Equals(right);
    public static bool operator ==(text left, string? right) => left.Equals(right);
    public static bool operator !=(text left, string? right) => !left.Equals(right);
    public static bool operator ==(text left, ReadOnlySpan<char> right) => left.Equals(right);
    public static bool operator !=(text left, ReadOnlySpan<char> right) => !left.Equals(right);
    public static bool operator ==(text left, char[]? right) => left.Equals(right);
    public static bool operator !=(text left, char[]? right) => !left.Equals(right);
    public static bool operator ==(text left, char right) => left.Equals(right);
    public static bool operator !=(text left, char right) => !left.Equals(right);
    
    private readonly unsafe char* _firstChar;
    private readonly int _length;

    internal ref char FirstChar
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        get
        {
            unsafe
            {
                return ref Danger.PointerToRef<char>(_firstChar);
            }
        }
    }

    public text(in char ch)
    {
        unsafe
        {
            _firstChar = Danger.InToPointer(in ch);
        }
        _length = 1;
    }

    public text(ReadOnlySpan<char> chars)
    {
        unsafe
        {
            _firstChar = Danger.InToPointer(in chars.GetPinnableReference());
        }
        _length = chars.Length;
    }

    public text(string? str)
    {
        if (str is null)
        {
            unsafe
            {
                _firstChar = Danger.NullPointer<char>();
            }
            _length = 0;
        }
        else
        {
            unsafe
            {
                _firstChar = Danger.InToPointer<char>(in str.GetPinnableReference());
            }
            _length = str.Length;
        }
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public ReadOnlySpan<char> AsSpan()
    {
        unsafe
        {
            return new ReadOnlySpan<char>(_firstChar, _length);
        }
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public bool TryCopyTo(Span<char> destination)
    {
        if (_length <= destination.Length)
        {
            unsafe
            {
                TextHelper.CopyTo(_firstChar,
                                  ref destination.GetPinnableReference(),
                                  _length);
            }
            return true;
        }
        return false;
    }
    
    public bool Equals(text text)
    {
        return TextHelper.Equals(AsSpan(), text.AsSpan());
    }
    
    public bool Equals(ReadOnlySpan<char> chars)
    {
        return TextHelper.Equals(AsSpan(), chars);
    }
    
    public bool Equals(string? str)
    {
        return TextHelper.Equals(AsSpan(), str);
    }
    
    public bool Equals(char[]? chars)
    {
        return TextHelper.Equals(AsSpan(), chars);
    }
    
    public bool Equals(char ch)
    {
        if (_length == 1)
        {
            unsafe
            {
                return Danger.Read<char>(_firstChar) == ch;
            }
        }
        return false;
    }

    public override bool Equals(object? obj)
    {
        if (obj is string str) return Equals(str);
        if (obj is char[] chars) return Equals(chars);
        return false;
    }

    public override int GetHashCode()
    {
        unsafe
        {
            return Hasher.Create<char>(_firstChar, _length);
        }
    }

    public override string ToString()
    {
        unsafe
        {
            return new string(_firstChar, 0, _length);
        }
    }
}