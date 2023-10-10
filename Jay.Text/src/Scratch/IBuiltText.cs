using System.Collections;

namespace Jay.Text.Scratch;

public interface IBuiltText :
#if NET6_0_OR_GREATER
    ISpanFormattable,
#endif
    IReadOnlyList<char>,
    IReadOnlyCollection<char>,
    IEnumerable<char>
{
    new ref char this[int index] { get; }
    ref char this[Index index] { get; }
    Span<char> this[Range range] { get; }
    
    int Length { get; }

    Span<char> AsSpan();
}

internal class BuiltText : IBuiltText
{
    protected char[] _charArray;
    protected int _position;

    int IReadOnlyCollection<char>.Count => _position;
    public int Length => _position;
    
    char IReadOnlyList<char>.this[int index] => this[index];

    public ref char this[int index]
    {
        get
        {
            Validate.Index(_position, index);
            return ref _charArray[index];
        }
    }

    public ref char this[Index index]
    {
        get
        {
            Validate.Index(_position, index);
            return ref _charArray[index];
        }
    }

    public Span<char> this[Range range]
    {
        get
        {
            Validate.Range(_position, range);
            return _charArray.AsSpan(range);
        }
    }

    public BuiltText(char[] charArray, int position = 0)
    {
        _charArray = charArray;
        _position = position;
    }
    
    public Span<char> AsSpan() => _charArray.AsSpan(0, _position);

    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

    public IEnumerator<char> GetEnumerator()
    {
        var chars = _charArray;
        var length = _position;
        for (var i = 0; i < length; i++)
        {
            yield return chars[i];
        }
    }

    public bool TryFormat(Span<char> destination, out int charsWritten, 
        ReadOnlySpan<char> format = default, 
        IFormatProvider? provider = default)
    {
        if (AsSpan().TryCopyTo(destination))
        {
            charsWritten = _position;
            return true;
        }
        charsWritten = 0;
        return false;
    }

    public string ToString(string? format, IFormatProvider? provider = default)
    {
        string str = new string(_charArray, 0, _position);
        if (format is not null)
        {
            if (format.Length == 1)
            {
                switch (format[0])
                {
                    case 'L' or 'l':
                        return str.ToCase(Casing.Lower);
                    case 'U' or 'u':
                        return str.ToCase(Casing.Upper);
                    case 'C' or 'c':
                        return str.ToCase(Casing.Camel);
                    case 'P' or 'p':
                        return str.ToCase(Casing.Pascal);
                    case 'T' or 't':
                        return str.ToCase(Casing.Title);
                }
            }
            else if (Enum.TryParse(format, out Casing casing))
            {
                return str.ToCase(casing);
            }

            throw new ArgumentException($"Invalid format '{format}'", nameof(format));
        }
        
        return str;
    }

    public override string ToString()
    {
        return new string(_charArray, 0, _position);
    }
}