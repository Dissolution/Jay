using System;
using System.Runtime.CompilerServices;

using static InlineIL.IL;
using static InlineIL.IL.Emit;

namespace Jay.NotSafe
{
    public static class Unmanaged
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int SizeOf<T>()
            where T : unmanaged
        {
            Emit.Sizeof<T>();
            return Return<int>();
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static ref T GetRef<T>(Span<T> span)
            where T : unmanaged
        {
            return ref span.GetPinnableReference();
        }
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static ref readonly T GetRef<T>(ReadOnlySpan<T> span)
            where T : unmanaged
        {
            return ref span.GetPinnableReference();
        }
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void BlockCopy<T>(in T source, ref T dest, int itemCount)
            where T : unmanaged
        {
            Emit.Ldarg(nameof(dest));
            Emit.Ldarg(nameof(source));
            Emit.Ldarg(nameof(itemCount));
            Emit.Sizeof<T>();
            Emit.Mul();
            Emit.Cpblk();
        }
    }
}