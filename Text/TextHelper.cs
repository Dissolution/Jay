using System.Runtime.CompilerServices;
using static InlineIL.IL;

namespace Jay.Text;

public static class TextHelper
{
    public static bool Equals(text x, text y)
    {
        return MemoryExtensions.SequenceEqual(x, y);
    }

    public static bool Equals(text x, text y, StringComparison comparison)
    {
        return MemoryExtensions.Equals(x, y, comparison);
    }


    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    internal static void CopyTo(in char source, ref char dest, int count)
    {
        Emit.Ldarg(nameof(dest));
        Emit.Ldarg(nameof(source));
        Emit.Ldarg(nameof(count));
        Emit.Ldc_I4_2(); // sizeof(char)
        Emit.Mul();
        Emit.Cpblk();
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

    public static bool TryCopyTo(ReadOnlySpan<char> source, Span<char> dest)
    {
        if (source.Length <= dest.Length)
        {
            CopyTo(in source.GetPinnableReference(),
                   ref dest.GetPinnableReference(),
                   source.Length);
        }
        return false;
    }
}