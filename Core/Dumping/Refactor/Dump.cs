using System.Buffers;
using System.Collections.Concurrent;
using System.Diagnostics;
using System.Reflection;
using Jay.Collections;
using Jay.Exceptions;
using Jay.Reflection;
using Jay.Reflection.Building;
using Jay.Text;
using Jay.Validation;
using static InlineIL.IL;

namespace Jay.Dumping.Refactor;

public static class DumperTest
{

    public static string DumpWith(ref Dumper dumper)
    {
        return dumper.GetStringAndDispose();
    }
}


public readonly struct DumpOptions
{
    public static readonly DumpOptions Default = default;
}

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
    
    public void AppendLiteral(string? text)
    {
        if (text is not null)
        {
            while (!TextHelper.TryCopyTo(text, Available))
            {
                Grow(text.Length);
            }
            _index += text.Length;
        }
    }

    public void AppendFormatted<T>(T value)
    {
        if (!DumpCache.TryDump<T>(value, ref this))
        {
            if (value is IFormattable)
            {
                if (value is ISpanFormattable)
                {
                    int charsWritten;
                    while (!((ISpanFormattable)value).TryFormat(Available, out charsWritten, default, default))
                    {
                        Grow(charsWritten);
                    }
                    _index += charsWritten;
                }
                else
                {
                    AppendLiteral(((IFormattable)value).ToString(null, null));
                }
            }
            else
            {
                AppendLiteral(value?.ToString());
            }
        }
    }
    
    public void AppendFormatted<T>(T value, string? format)
    {
        if (!DumpCache.TryDump<T>(value, ref this))
        {
            if (value is IFormattable)
            {
                if (value is ISpanFormattable)
                {
                    int charsWritten;
                    while (!((ISpanFormattable)value).TryFormat(Available, out charsWritten, format, default))
                    {
                        Grow(charsWritten);
                    }
                    _index += charsWritten;
                }
                else
                {
                    AppendLiteral(((IFormattable)value).ToString(format, null));
                }
            }
            else
            {
                AppendLiteral(value?.ToString());
            }
        }
    }

    public void AppendFormatted<T>(T value, DumpOptions options)
    {
        
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

public delegate void Dump<TInstance>(TInstance instance, ref Dumper dumper);

public interface IDumpable
{
    void Dump(ref Dumper dumper);
}

public static class DumpCache
{
    private static readonly ConcurrentTypeDictionary<Delegate?> _dumpDelegateCache;

    static DumpCache()
    {
        _dumpDelegateCache = new();
    }

    public static bool TryDump<T>(T value, ref Dumper dumper)
    {
        if (_dumpDelegateCache.GetOrAdd<T>(FindDumpDelegate) is Dump<T> dump)
        {
            dump(value, ref dumper);
            return true;
        }
        return false;
    }
    
    private static Delegate? FindDumpDelegate(Type instanceType)
    {
        // Check implements
        // TODO: order by distance between pair.Key and InstanceType
        var implemented = _dumpDelegateCache.Where(pair => pair.Key.Implements(instanceType))
            .Select(pair => pair.Value)
            .FirstOrDefault();
        if (implemented is not null)
        {
            return implemented;
        }

        MethodInfo? method;
        
        // Find IDumpable / delegate (duck-typed)
        method = instanceType.GetMethod(name: "Dump",
            bindingAttr: BindingFlags.Public | BindingFlags.Instance,
            types: new Type[1] { typeof(Dumper).MakeByRefType() });
        if (method is not null)
        {
            return RuntimeBuilder.CreateDelegate(typeof(Dump<>).MakeGenericType(instanceType),
                "Invoke",
                emitter => emitter.Ldarg(0).Ldarg(1).Call(method).Ret());
        }

        // Todo: What else can we manually implement?

        return null;
    }
}