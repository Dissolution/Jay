using System.Buffers;
using System.Collections.Concurrent;
using Jay.Exceptions;
using Jay.Text;
using static InlineIL.IL;

namespace Jay.Dumping.Refactor;

[InterpolatedStringHandler]
public ref struct Dumper
{
    
    private static int GetCapacity(int literalLength, int formattedCount) 
        => Math.Min(1024, literalLength + (formattedCount * 16));
    
    
    private char[]? _charArray;
    private Span<char> _charSpan;
    private int _index;

    public int Length => _index;

    internal Span<char> Written => _charSpan[.._index];
    internal Span<char> Available => _charSpan[_index..];

    public Dumper(int literalLength, int formattedCount)
    {
        _charSpan = _charArray = ArrayPool<char>.Shared.Rent(GetCapacity(literalLength, formattedCount));
        _index = 0;
    }

    [MethodImpl(MethodImplOptions.NoInlining)]
    private void Grow(int adding)
    {
        // string.MaxLength < array.MaxLength
        const int maxCapacity = 0x3FFFFFDF;
        int newCapacity = Math.Clamp(_index + adding, _charSpan.Length * 2, maxCapacity);
        char[] newArray = ArrayPool<char>.Shared.Rent(newCapacity);
        TextHelper.Copy(in _charSpan.GetPinnableReference(),
            ref newArray[0],
            _index);
        char[]? toReturn = _charArray;
        _charSpan = _charArray = newArray;
        if (toReturn is not null)
        {
            ArrayPool<char>.Shared.Return(toReturn);
        }
    }
    
    public void AppendLiteral(string text)
    {
        while (!text.TryCopyTo(Available))
        {
            Grow(text.Length);
        }
        _index += text.Length;
    }

    public void AppendFormatted<T>(T value)
    {
        DumpCache
    }

    public void Clear()
    {
        _index = 0;
    }
    
    public void Dispose()
    {
        char[]? toReturn = _charArray;
        this = default; // defensive clear
        if (toReturn is not null)
        {
            ArrayPool<char>.Shared.Return(toReturn);
        }
    }

    public string GetStringAndDispose()
    {
        string str = new string(Written);
        Dispose();
        return str;
    }
    
    public override bool Equals(object? obj) => UnsuitableException.ThrowEquals(typeof(Dumper));

    public override int GetHashCode() => UnsuitableException.ThrowGetHashCode(typeof(Dumper));

    public override string ToString()
    {
        return new string(Written);
    }
}

public delegate void Dump(ref Dumper dumper);

public static class DumpCache
{
    private static readonly List<Dump> _dumpDelegateList;
    private static readonly ConcurrentDictionary<Type, Dump> _dumpDelegateCache;

    static DumpCache()
    {
        _dumpDelegateList = new();
        _dumpDelegateCache = new();
    }

    public static Dump GetDumpDelegate(Type type)
    {
        if (_dumpDelegateCache.TryGetValue(type, out var dump))
        {
            return dump;
        }

        for (var i = 0; i < _dumpDelegateList.Count; i++)
        {
            dump = _dumpDelegateList[i];
            if (dump.)
        }
    }
}