using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using static InlineIL.IL;

namespace Jay.Text;

public static class TextHelper
{
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void CopyTo(in char sourcePtr, ref char destPtr, int charCount)
    {
        Emit.Ldarg(nameof(destPtr));
        Emit.Ldarg(nameof(sourcePtr));
        Emit.Ldarg(nameof(charCount));
        Emit.Sizeof<char>();
        Emit.Mul();
        Emit.Cpblk();
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void CopyTo(ReadOnlySpan<char> source, Span<char> dest)
    {
        CopyTo(in source.GetPinnableReference(),
               ref dest.GetPinnableReference(),
               source.Length);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void CopyTo(ReadOnlySpan<char> source, char[] dest)
    {
        CopyTo(in source.GetPinnableReference(),
               ref MemoryMarshal.GetArrayDataReference<char>(dest),
               source.Length);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void CopyTo(char[] source, Span<char> dest)
    {
        CopyTo(in MemoryMarshal.GetArrayDataReference<char>(source),
               ref dest.GetPinnableReference(),
               source.Length);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void CopyTo(char[] source, char[] dest)
    {
        CopyTo(in MemoryMarshal.GetArrayDataReference<char>(source),
               ref MemoryMarshal.GetArrayDataReference<char>(dest),
               source.Length);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void CopyTo(string source, Span<char> dest)
    {
        CopyTo(in source.GetPinnableReference(),
               ref dest.GetPinnableReference(),
               source.Length);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void CopyTo(string source, char[] dest)
    {
        CopyTo(in source.GetPinnableReference(),
               ref MemoryMarshal.GetArrayDataReference<char>(dest),
               source.Length);
    }
}