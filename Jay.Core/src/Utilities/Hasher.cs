using System.ComponentModel;
using System.Runtime.InteropServices;
using System.Security.Cryptography;

namespace Jay.Utilities;

/// <summary>
/// Hasher is a near-copy of <c>System.HashCode</c> for use in
/// netstandard2.0 and net48 environments.<br />
/// It also has more methods to ease hashcode generation.
/// </summary>
public ref struct Hasher
{
    private static readonly uint _seed = CreateSeed();

    private const uint PRIME1 = 2_654_435_761U;
    private const uint PRIME2 = 2_246_822_519U;
    private const uint PRIME3 = 3_266_489_917U;
    private const uint PRIME4 = 668_265_263U;
    private const uint PRIME5 = 374_761_393U;

    private static uint CreateSeed()
    {
#if NET48 || NETSTANDARD2_0
        using var rng = RandomNumberGenerator.Create();
        byte[] bytes = new byte[sizeof(uint)];
        rng.GetBytes(bytes);
        return MemoryMarshal.Read<uint>(bytes);
#else
        Span<byte> bytes = stackalloc byte[sizeof(uint)];
        RandomNumberGenerator.Fill(bytes);
        return MemoryMarshal.Read<uint>(bytes);
#endif
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static void Initialize(out uint v1, out uint v2, out uint v3, out uint v4)
    {
        v1 = _seed + PRIME1 + PRIME2;
        v2 = _seed + PRIME2;
        v3 = _seed;
        v4 = _seed - PRIME1;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static uint Round(uint hash, uint input)
    {
        return MathHelper.RotateLeft(hash + input * PRIME2, 13) * PRIME1;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static uint QueueRound(uint hash, uint queuedValue)
    {
        return MathHelper.RotateLeft(hash + queuedValue * PRIME3, 17) * PRIME4;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static uint MixState(uint v1, uint v2, uint v3, uint v4)
    {
        return MathHelper.RotateLeft(v1, 1)
            + MathHelper.RotateLeft(v2, 7)
            + MathHelper.RotateLeft(v3, 12)
            + MathHelper.RotateLeft(v4, 18);
    }

    private static uint MixEmptyState()
    {
        return _seed + PRIME5;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static uint MixFinal(uint hash)
    {
        hash ^= hash >> 15;
        hash *= PRIME2;
        hash ^= hash >> 13;
        hash *= PRIME3;
        hash ^= hash >> 16;
        return hash;
    }
    
    /// <summary>
    /// Gets the hash code for a single <paramref name="value"/>
    /// </summary>
    /// <param name="value"></param>
    /// <typeparam name="T"></typeparam>
    /// <returns></returns>
    public static int GetHashCode<T>(T? value)
    {
        var hc1 = (uint)(value?.GetHashCode() ?? 0);

        uint hash = MixEmptyState();
        hash += 4;

        hash = QueueRound(hash, hc1);

        hash = MixFinal(hash);
        return (int)hash;
    }

    
    public static int Combine<T1, T2>(T1? value1, T2? value2)
    {
        var hc1 = (uint)(value1?.GetHashCode() ?? 0);
        var hc2 = (uint)(value2?.GetHashCode() ?? 0);

        uint hash = MixEmptyState();
        hash += 8;

        hash = QueueRound(hash, hc1);
        hash = QueueRound(hash, hc2);

        hash = MixFinal(hash);
        return (int)hash;
    }

    public static int Combine<T1, T2, T3>(T1? value1, T2? value2, T3? value3)
    {
        var hc1 = (uint)(value1?.GetHashCode() ?? 0);
        var hc2 = (uint)(value2?.GetHashCode() ?? 0);
        var hc3 = (uint)(value3?.GetHashCode() ?? 0);

        uint hash = MixEmptyState();
        hash += 12;

        hash = QueueRound(hash, hc1);
        hash = QueueRound(hash, hc2);
        hash = QueueRound(hash, hc3);

        hash = MixFinal(hash);
        return (int)hash;
    }

    public static int Combine<T1, T2, T3, T4>(T1? value1, T2? value2, T3? value3, T4? value4)
    {
        var hc1 = (uint)(value1?.GetHashCode() ?? 0);
        var hc2 = (uint)(value2?.GetHashCode() ?? 0);
        var hc3 = (uint)(value3?.GetHashCode() ?? 0);
        var hc4 = (uint)(value4?.GetHashCode() ?? 0);

        Initialize(out uint v1, out uint v2, out uint v3, out uint v4);

        v1 = Round(v1, hc1);
        v2 = Round(v2, hc2);
        v3 = Round(v3, hc3);
        v4 = Round(v4, hc4);

        uint hash = MixState(v1, v2, v3, v4);
        hash += 16;

        hash = MixFinal(hash);
        return (int)hash;
    }

    public static int Combine<T1, T2, T3, T4, T5>(T1? value1, T2? value2, T3? value3, T4? value4, T5? value5)
    {
        var hc1 = (uint)(value1?.GetHashCode() ?? 0);
        var hc2 = (uint)(value2?.GetHashCode() ?? 0);
        var hc3 = (uint)(value3?.GetHashCode() ?? 0);
        var hc4 = (uint)(value4?.GetHashCode() ?? 0);
        var hc5 = (uint)(value5?.GetHashCode() ?? 0);

        Initialize(out uint v1, out uint v2, out uint v3, out uint v4);

        v1 = Round(v1, hc1);
        v2 = Round(v2, hc2);
        v3 = Round(v3, hc3);
        v4 = Round(v4, hc4);

        uint hash = MixState(v1, v2, v3, v4);
        hash += 20;

        hash = QueueRound(hash, hc5);

        hash = MixFinal(hash);
        return (int)hash;
    }

    public static int Combine<T1, T2, T3, T4, T5, T6>(T1? value1, T2? value2, T3? value3, T4? value4, T5? value5, T6? value6)
    {
        var hc1 = (uint)(value1?.GetHashCode() ?? 0);
        var hc2 = (uint)(value2?.GetHashCode() ?? 0);
        var hc3 = (uint)(value3?.GetHashCode() ?? 0);
        var hc4 = (uint)(value4?.GetHashCode() ?? 0);
        var hc5 = (uint)(value5?.GetHashCode() ?? 0);
        var hc6 = (uint)(value6?.GetHashCode() ?? 0);

        Initialize(out uint v1, out uint v2, out uint v3, out uint v4);

        v1 = Round(v1, hc1);
        v2 = Round(v2, hc2);
        v3 = Round(v3, hc3);
        v4 = Round(v4, hc4);

        uint hash = MixState(v1, v2, v3, v4);
        hash += 24;

        hash = QueueRound(hash, hc5);
        hash = QueueRound(hash, hc6);

        hash = MixFinal(hash);
        return (int)hash;
    }

    public static int Combine<T1, T2, T3, T4, T5, T6, T7>(T1? value1, T2? value2, T3? value3, T4? value4, T5? value5, T6? value6, T7? value7)
    {
        var hc1 = (uint)(value1?.GetHashCode() ?? 0);
        var hc2 = (uint)(value2?.GetHashCode() ?? 0);
        var hc3 = (uint)(value3?.GetHashCode() ?? 0);
        var hc4 = (uint)(value4?.GetHashCode() ?? 0);
        var hc5 = (uint)(value5?.GetHashCode() ?? 0);
        var hc6 = (uint)(value6?.GetHashCode() ?? 0);
        var hc7 = (uint)(value7?.GetHashCode() ?? 0);

        Initialize(out uint v1, out uint v2, out uint v3, out uint v4);

        v1 = Round(v1, hc1);
        v2 = Round(v2, hc2);
        v3 = Round(v3, hc3);
        v4 = Round(v4, hc4);

        uint hash = MixState(v1, v2, v3, v4);
        hash += 28;

        hash = QueueRound(hash, hc5);
        hash = QueueRound(hash, hc6);
        hash = QueueRound(hash, hc7);

        hash = MixFinal(hash);
        return (int)hash;
    }

    public static int Combine<T1, T2, T3, T4, T5, T6, T7, T8>(T1? value1, T2? value2, T3? value3, T4? value4, T5? value5, T6? value6, T7? value7,
        T8? value8)
    {
        var hc1 = (uint)(value1?.GetHashCode() ?? 0);
        var hc2 = (uint)(value2?.GetHashCode() ?? 0);
        var hc3 = (uint)(value3?.GetHashCode() ?? 0);
        var hc4 = (uint)(value4?.GetHashCode() ?? 0);
        var hc5 = (uint)(value5?.GetHashCode() ?? 0);
        var hc6 = (uint)(value6?.GetHashCode() ?? 0);
        var hc7 = (uint)(value7?.GetHashCode() ?? 0);
        var hc8 = (uint)(value8?.GetHashCode() ?? 0);

        Initialize(out uint v1, out uint v2, out uint v3, out uint v4);

        v1 = Round(v1, hc1);
        v2 = Round(v2, hc2);
        v3 = Round(v3, hc3);
        v4 = Round(v4, hc4);

        v1 = Round(v1, hc5);
        v2 = Round(v2, hc6);
        v3 = Round(v3, hc7);
        v4 = Round(v4, hc8);

        uint hash = MixState(v1, v2, v3, v4);
        hash += 32;

        hash = MixFinal(hash);
        return (int)hash;
    }

    public static int Combine<T>(ReadOnlySpan<T> span)
    {
        switch (span.Length)
        {
            case 0: return 0;
            case 1: return GetHashCode(span[0]);
            case 2: return Combine(span[0], span[1]);
            case 3: return Combine(span[0], span[1], span[2]);
            case 4: return Combine(span[0], span[1], span[2], span[3]);
            case 5: return Combine(span[0], span[1], span[2], span[3], span[4]);
            case 6: return Combine(span[0], span[1], span[2], span[3], span[4], span[5]);
            case 7: return Combine(span[0], span[1], span[2], span[3], span[4], span[5], span[6]);
            case 8: return Combine(span[0], span[1], span[2], span[3], span[4], span[5], span[6], span[7]);
            default:
            {
                var hasher = new Hasher();
                for (var i = 0; i < span.Length; i++)
                {
                    hasher.Add(span[i]);
                }
                return hasher.ToHashCode();
            }
        }
    }

    public static int Combine<T>(ReadOnlySpan<T> span, IEqualityComparer<T>? comparer)
    {
        var hasher = new Hasher();
        for (var i = 0; i < span.Length; i++)
        {
            hasher.Add(span[i], comparer);
        }
        return hasher.ToHashCode();
    }

    public static int Combine<T>(params T?[]? array)
    {
        if (array is null) return 0;
        switch (array.Length)
        {
            case 0: return 0;
            case 1: return GetHashCode(array[0]);
            case 2: return Combine(array[0], array[1]);
            case 3: return Combine(array[0], array[1], array[2]);
            case 4: return Combine(array[0], array[1], array[2], array[3]);
            case 5: return Combine(array[0], array[1], array[2], array[3], array[4]);
            case 6: return Combine(array[0], array[1], array[2], array[3], array[4], array[5]);
            case 7: return Combine(array[0], array[1], array[2], array[3], array[4], array[5], array[6]);
            case 8: return Combine(array[0], array[1], array[2], array[3], array[4], array[5], array[6], array[7]);
            default:
            {
                var hasher = new Hasher();
                for (var i = 0; i < array.Length; i++)
                {
                    hasher.Add(array[i]);
                }
                return hasher.ToHashCode();
            }
        }
    }

    public static int Combine<T>(T?[]? array, IEqualityComparer<T>? comparer)
    {
        if (array is null) return 0;
        var hasher = new Hasher();
        for (var i = 0; i < array.Length; i++)
        {
            hasher.Add(array[i], comparer);
        }
        return hasher.ToHashCode();
    }

    public static int Combine<T>(IEnumerable<T>? enumerable)
    {
        if (enumerable is null) return 0;
        var hasher = new Hasher();
        foreach (T value in enumerable)
        {
            hasher.Add(value);
        }
        return hasher.ToHashCode();
    }

    public static int Combine<T>(IEnumerable<T>? enumerable, IEqualityComparer<T>? comparer)
    {
        if (enumerable is null) return 0;
        var hasher = new Hasher();
        foreach (T value in enumerable)
        {
            hasher.Add(value, comparer);
        }
        return hasher.ToHashCode();
    }
    

    // Instance Methods
    private uint _v1, _v2, _v3, _v4;
    private uint _queue1, _queue2, _queue3;
    private uint _length;

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private void AddHash(int value)
    {
        uint val = (uint)value;
        uint previousLength = _length++;
        uint position = previousLength % 4;
        
        if (position == 0)
            _queue1 = val;
        else if (position == 1)
            _queue2 = val;
        else if (position == 2)
            _queue3 = val;
        else // position == 3
        {
            if (previousLength == 3)
                Initialize(out _v1, out _v2, out _v3, out _v4);

            _v1 = Round(_v1, _queue1);
            _v2 = Round(_v2, _queue2);
            _v3 = Round(_v3, _queue3);
            _v4 = Round(_v4, val);
        }
    }

    public void Add<T>(T? value)
    {
        if (value is null)
        {
            AddHash(0);
        }
        else
        {
            AddHash(value.GetHashCode());
        }
    }

    public void Add<T>(T? value, IEqualityComparer<T>? comparer)
    {
        if (value is null)
        {
            AddHash(0);
        }
        else if (comparer is not null)
        {
            AddHash(comparer.GetHashCode(value));
        }
        else
        {
            AddHash(value.GetHashCode());
        }
    }

    public void AddAll<T>(params T?[]? values)
    {
        if (values is null) return;
        for (var i = 0; i < values.Length; i++)
        {
            Add<T>(values[i]);
        }
    }

    public void AddAll<T>(T?[]? values, IEqualityComparer<T>? comparer)
    {
        if (values is null) return;
        for (var i = 0; i < values.Length; i++)
        {
            Add<T>(values[i], comparer);
        }
    }

    public void AddAll<T>(IEnumerable<T?>? values)
    {
        if (values is null) return;
        foreach (T? value in values)
        {
            Add<T>(value);
        }
    }

    public void AddAll<T>(IEnumerable<T?>? values, IEqualityComparer<T>? comparer)
    {
        if (values is null) return;
        foreach (T? value in values)
        {
            Add<T>(value, comparer);
        }
    }

    public int ToHashCode()
    {
        // Storing the value of _length locally shaves of quite a few bytes
        // in the resulting machine code.
        uint length = _length;

        // position refers to the *next* queue position in this method, so
        // position == 1 means that _queue1 is populated; _queue2 would have
        // been populated on the next call to Add.
        uint position = length % 4;

        // If the length is less than 4, _v1 to _v4 don't contain anything
        // yet. xxHash32 treats this differently.

        uint hash = length < 4 ? MixEmptyState() : MixState(_v1, _v2, _v3, _v4);

        // _length is incremented once per Add(Int32) and is therefore 4
        // times too small (xxHash length is in bytes, not ints).

        hash += length * 4;

        // Mix what remains in the queue

        // Switch can't be inlined right now, so use as few branches as
        // possible by manually excluding impossible scenarios (position > 1
        // is always false if position is not > 0).
        if (position > 0)
        {
            hash = QueueRound(hash, _queue1);
            if (position > 1)
            {
                hash = QueueRound(hash, _queue2);
                if (position > 2)
                    hash = QueueRound(hash, _queue3);
            }
        }

        hash = MixFinal(hash);
        return (int)hash;
    }

#pragma warning disable CS0809
    [Obsolete("HashCode is a mutable struct and should not be compared with other HashCodes. Use ToHashCode to retrieve the computed hash code.",
         true),EditorBrowsable(EditorBrowsableState.Never)]
    public override int GetHashCode()
    {
        throw new NotSupportedException();
    }

    [Obsolete("HashCode is a mutable struct and should not be compared with other HashCodes.", true),EditorBrowsable(EditorBrowsableState.Never)]
    public override bool Equals(object? obj)
    {
        throw new NotSupportedException();
    }
#pragma warning restore CS0809
    
    public override string ToString()
    {
        return $"{ToHashCode():X}";
    }
}