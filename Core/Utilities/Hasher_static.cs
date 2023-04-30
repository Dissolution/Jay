using System.Security.Cryptography;

namespace Jay.Utilities;

public partial struct Hasher
{
    private static readonly uint _seed = GenerateGlobalSeed();

    private const uint Prime1 = 2654435761U;
    private const uint Prime2 = 2246822519U;
    private const uint Prime3 = 3266489917U;
    private const uint Prime4 = 668265263U;
    private const uint Prime5 = 374761393U;

    private static uint GenerateGlobalSeed()
    {
        using var rng = new RNGCryptoServiceProvider();
#if NETSTANDARD2_0
        byte[] bytes = new byte[sizeof(uint)];
        rng.GetBytes(bytes);
        return BitConverter.ToUInt32(bytes, 0);
#else
        Span<byte> bytes = stackalloc byte[sizeof(uint)];
        rng.GetBytes(bytes);
        return BitConverter.ToUInt32(bytes);
#endif
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static void Initialize(out uint v1, out uint v2, out uint v3, out uint v4)
    {
        v1 = _seed + Prime1 + Prime2;
        v2 = _seed + Prime2;
        v3 = _seed;
        v4 = _seed - Prime1;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static uint Round(uint hash, uint input)
    {
        return Maths.RotateLeft(hash + input * Prime2, 13) * Prime1;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static uint QueueRound(uint hash, uint queuedValue)
    {
        return Maths.RotateLeft(hash + queuedValue * Prime3, 17) * Prime4;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static uint MixState(uint v1, uint v2, uint v3, uint v4)
    {
        return Maths.RotateLeft(v1, 1) + Maths.RotateLeft(v2, 7) + Maths.RotateLeft(v3, 12) + Maths.RotateLeft(v4, 18);
    }

    private static uint MixEmptyState()
    {
        return _seed + Prime5;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static uint MixFinal(uint hash)
    {
        hash ^= hash >> 15;
        hash *= Prime2;
        hash ^= hash >> 13;
        hash *= Prime3;
        hash ^= hash >> 16;
        return hash;
    }


    public static int Combine<T1>(T1? value1)
    {
        // Provide a way of diffusing bits from something with a limited
        // input hash space. For example, many enums only have a few
        // possible hashes, only using the bottom few bits of the code. Some
        // collections are built on the assumption that hashes are spread
        // over a larger space, so diffusing the bits may help the
        // collection work more efficiently.

        uint hc1 = (uint)(value1?.GetHashCode() ?? 0);

        uint hash = MixEmptyState();
        hash += 4;

        hash = QueueRound(hash, hc1);

        hash = MixFinal(hash);
        return (int)hash;
    }

    public static int Combine<T1, T2>(T1? value1, T2? value2)
    {
        uint hc1 = (uint)(value1?.GetHashCode() ?? 0);
        uint hc2 = (uint)(value2?.GetHashCode() ?? 0);

        uint hash = MixEmptyState();
        hash += 8;

        hash = QueueRound(hash, hc1);
        hash = QueueRound(hash, hc2);

        hash = MixFinal(hash);
        return (int)hash;
    }

    public static int Combine<T1, T2, T3>(T1? value1, T2? value2, T3? value3)
    {
        uint hc1 = (uint)(value1?.GetHashCode() ?? 0);
        uint hc2 = (uint)(value2?.GetHashCode() ?? 0);
        uint hc3 = (uint)(value3?.GetHashCode() ?? 0);

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
        uint hc1 = (uint)(value1?.GetHashCode() ?? 0);
        uint hc2 = (uint)(value2?.GetHashCode() ?? 0);
        uint hc3 = (uint)(value3?.GetHashCode() ?? 0);
        uint hc4 = (uint)(value4?.GetHashCode() ?? 0);

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
        uint hc1 = (uint)(value1?.GetHashCode() ?? 0);
        uint hc2 = (uint)(value2?.GetHashCode() ?? 0);
        uint hc3 = (uint)(value3?.GetHashCode() ?? 0);
        uint hc4 = (uint)(value4?.GetHashCode() ?? 0);
        uint hc5 = (uint)(value5?.GetHashCode() ?? 0);

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
        uint hc1 = (uint)(value1?.GetHashCode() ?? 0);
        uint hc2 = (uint)(value2?.GetHashCode() ?? 0);
        uint hc3 = (uint)(value3?.GetHashCode() ?? 0);
        uint hc4 = (uint)(value4?.GetHashCode() ?? 0);
        uint hc5 = (uint)(value5?.GetHashCode() ?? 0);
        uint hc6 = (uint)(value6?.GetHashCode() ?? 0);

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
        uint hc1 = (uint)(value1?.GetHashCode() ?? 0);
        uint hc2 = (uint)(value2?.GetHashCode() ?? 0);
        uint hc3 = (uint)(value3?.GetHashCode() ?? 0);
        uint hc4 = (uint)(value4?.GetHashCode() ?? 0);
        uint hc5 = (uint)(value5?.GetHashCode() ?? 0);
        uint hc6 = (uint)(value6?.GetHashCode() ?? 0);
        uint hc7 = (uint)(value7?.GetHashCode() ?? 0);

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
        uint hc1 = (uint)(value1?.GetHashCode() ?? 0);
        uint hc2 = (uint)(value2?.GetHashCode() ?? 0);
        uint hc3 = (uint)(value3?.GetHashCode() ?? 0);
        uint hc4 = (uint)(value4?.GetHashCode() ?? 0);
        uint hc5 = (uint)(value5?.GetHashCode() ?? 0);
        uint hc6 = (uint)(value6?.GetHashCode() ?? 0);
        uint hc7 = (uint)(value7?.GetHashCode() ?? 0);
        uint hc8 = (uint)(value8?.GetHashCode() ?? 0);

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

    public static int Combine<T>(ReadOnlySpan<T?> span)
    {
        switch (span.Length)
        {
            case 0: return 0;
            case 1: return Hasher.Combine(span[0]);
            case 2: return Hasher.Combine(span[0], span[1]);
            case 3: return Hasher.Combine(span[0], span[1], span[2]);
            case 4: return Hasher.Combine(span[0], span[1], span[2], span[3]);
            case 5: return Hasher.Combine(span[0], span[1], span[2], span[3], span[4]);
            case 6: return Hasher.Combine(span[0], span[1], span[2], span[3], span[4], span[5]);
            case 7: return Hasher.Combine(span[0], span[1], span[2], span[3], span[4], span[5], span[6]);
            case 8: return Hasher.Combine(span[0], span[1], span[2], span[3], span[4], span[5], span[6], span[7]);
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
            hasher.Add<T>(span[i], comparer);
        }
        return hasher.ToHashCode();
    }

    public static int Combine<T>(params T?[]? array)
    {
        if (array is null) return 0;
        switch (array.Length)
        {
            case 0: return 0;
            case 1: return Hasher.Combine(array[0]);
            case 2: return Hasher.Combine(array[0], array[1]);
            case 3: return Hasher.Combine(array[0], array[1], array[2]);
            case 4: return Hasher.Combine(array[0], array[1], array[2], array[3]);
            case 5: return Hasher.Combine(array[0], array[1], array[2], array[3], array[4]);
            case 6: return Hasher.Combine(array[0], array[1], array[2], array[3], array[4], array[5]);
            case 7: return Hasher.Combine(array[0], array[1], array[2], array[3], array[4], array[5], array[6]);
            case 8: return Hasher.Combine(array[0], array[1], array[2], array[3], array[4], array[5], array[6], array[7]);
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
            hasher.Add<T>(array[i], comparer);
        }
        return hasher.ToHashCode();
    }

    public static int Combine<T>(IEnumerable<T>? enumerable)
    {
        if (enumerable is null) return 0;
        var hasher = new Hasher();
        foreach (T value in enumerable)
        {
            hasher.Add<T>(value);
        }
        return hasher.ToHashCode();
    }

    public static int Combine<T>(IEnumerable<T>? enumerable, IEqualityComparer<T>? comparer)
    {
        if (enumerable is null) return 0;
        var hasher = new Hasher();
        foreach (T value in enumerable)
        {
            hasher.Add<T>(value, comparer);
        }
        return hasher.ToHashCode();
    }

    public static int Combine(params object?[]? objects)
    {
        if (objects is null) return 0;
        switch (objects.Length)
        {
            case 0: return 0;
            case 1: return Hasher.Combine(objects[0]);
            case 2: return Hasher.Combine(objects[0], objects[1]);
            case 3: return Hasher.Combine(objects[0], objects[1], objects[2]);
            case 4: return Hasher.Combine(objects[0], objects[1], objects[2], objects[3]);
            case 5: return Hasher.Combine(objects[0], objects[1], objects[2], objects[3], objects[4]);
            case 6: return Hasher.Combine(objects[0], objects[1], objects[2], objects[3], objects[4], objects[5]);
            case 7: return Hasher.Combine(objects[0], objects[1], objects[2], objects[3], objects[4], objects[5], objects[6]);
            case 8: return Hasher.Combine(objects[0], objects[1], objects[2], objects[3], objects[4], objects[5], objects[6], objects[7]);
            default:
            {
                var hasher = new Hasher();
                for (var i = 0; i < objects.Length; i++)
                {
                    hasher.Add<object?>(objects[i]);
                }
                return hasher.ToHashCode();
            }
        }
    }
}