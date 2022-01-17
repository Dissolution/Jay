using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using InlineIL;

namespace Jay.Text;

public static class TextHelper
{
    public static bool Equals(string? x, string? y)
    {
        return string.Equals(x, y);
    }

    public static bool Equals(string x, ReadOnlySpan<char> y)
    {
        return MemoryExtensions.SequenceEqual(x, y);
    }

    public static bool Equals(ReadOnlySpan<char> x, string y)
    {
        return MemoryExtensions.SequenceEqual(x, y);
    }

    public static bool Equals(ReadOnlySpan<char> x, ReadOnlySpan<char> y)
    {
        return MemoryExtensions.SequenceEqual(x, y);
    }

    public static bool Equals(string? x, string? y, StringComparison comparison)
    {
        return MemoryExtensions.Equals(x, y, comparison);
    }

    public static bool Equals(string x, ReadOnlySpan<char> y, StringComparison comparison)
    {
        return MemoryExtensions.Equals(x, y, comparison);
    }

    public static bool Equals(ReadOnlySpan<char> x, string y, StringComparison comparison)
    {
        return MemoryExtensions.Equals(x, y, comparison);
    }

    public static bool Equals(ReadOnlySpan<char> x, ReadOnlySpan<char> y, StringComparison comparison)
    {
        return MemoryExtensions.Equals(x, y, comparison);
    }


    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    internal static void CopyTo(in char source, ref char dest, int count)
    {
        IL.Emit.Ldarg(nameof(dest));
        IL.Emit.Ldarg(nameof(source));
        IL.Emit.Ldarg(nameof(count));
        IL.Emit.Ldc_I4_2(); // sizeof(char)
        IL.Emit.Mul();
        IL.Emit.Cpblk();
    }
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    internal static void CopyTwoCharsTo(in char source, ref char dest)
    {
        IL.Emit.Ldarg(nameof(dest));
        IL.Emit.Ldarg(nameof(source));
        IL.Emit.Ldc_I4_4(); // = sizeof(char) * 2
        IL.Emit.Mul();
        IL.Emit.Cpblk();
    }

    public static void CopyTo(ReadOnlySpan<char> source, Span<char> dest)
    {
        if (source.Length <= dest.Length)
        {
            CopyTo(in source.GetPinnableReference(),
                ref dest.GetPinnableReference(),
                source.Length);
        }
        throw new ArgumentException("Destination cannot contain source", nameof(dest));
    }

    public static void CopyTo(ReadOnlySpan<char> source, char[] dest)
    {
        if (source.Length <= dest.Length)
        {
            CopyTo(in source.GetPinnableReference(),
                ref MemoryMarshal.GetArrayDataReference(dest),
                source.Length);
        }
        throw new ArgumentException("Destination cannot contain source", nameof(dest));
    }

    public static void CopyTo(string? source, Span<char> dest)
    {
        if (source is null) return;
        if (source.Length <= dest.Length)
        {
            CopyTo(in source.GetPinnableReference(),
                ref dest.GetPinnableReference(),
                source.Length);
        }
        throw new ArgumentException("Destination cannot contain source", nameof(dest));
    }

    public static void CopyTo(string? source, char[] dest)
    {
        if (source is null) return;
        if (source.Length <= dest.Length)
        {
            CopyTo(in source.GetPinnableReference(),
                ref MemoryMarshal.GetArrayDataReference(dest),
                source.Length);
        }
        throw new ArgumentException("Destination cannot contain source", nameof(dest));
    }

    public static bool TryCopyTo(ReadOnlySpan<char> source, Span<char> dest)
    {
        if (source.Length <= dest.Length)
        {
            CopyTo(in source.GetPinnableReference(),
                ref dest.GetPinnableReference(),
                source.Length);
            return true;
        }
        return false;
    }
}