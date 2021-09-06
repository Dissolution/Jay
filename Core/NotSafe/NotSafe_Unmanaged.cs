using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using InlineIL;
using static InlineIL.IL;

namespace Jay
{
    public static partial class NotSafe
    {
            
        public static class Unmanaged
        {
            [DllImport("msvcrt.dll", CallingConvention = CallingConvention.Cdecl)]
            // ReSharper disable once IdentifierTypo
            internal static extern unsafe int memcmp(void* first, void* b2, nuint count);

            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public static unsafe int MemCompare<T>(in T first, in T second, int count)
                where T : unmanaged
            {
                Emit.Ldarg(nameof(first));
                Emit.Ldarg(nameof(second));
                Emit.Sizeof<T>();
                Emit.Ldarg(nameof(count));
                Emit.Mul();
                Emit.Call(MethodRef.Method(typeof(Unmanaged), nameof(memcmp)));
                return Return<int>();
            }
        
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public static ref T GetRef<T>(Span<T> span)
            {
                return ref span.GetPinnableReference();
            }
            
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public static ref readonly T GetRef<T>(ReadOnlySpan<T> span)
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
            
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public static unsafe void BlockCopy<T>(T* source, T* dest, int itemCount)
                where T : unmanaged
            {
                Emit.Ldarg(nameof(dest));
                Emit.Ldarg(nameof(source));
                Emit.Ldarg(nameof(itemCount));
                Emit.Sizeof<T>();
                Emit.Mul();
                Emit.Cpblk();
            }

            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public static TOut As<TIn, TOut>(TIn input)
            {
                Emit.Ldarg(nameof(input));
                return Return<TOut>();
            }

            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public static T Add<T>(T a, T b)
            {
                Emit.Ldarg(nameof(a));
                Emit.Ldarg(nameof(b));
                Emit.Add();
                return Return<T>();
            }
            
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public static T Subtract<T>(T a, T b)
            {
                Emit.Ldarg(nameof(a));
                Emit.Ldarg(nameof(b));
                Emit.Sub();
                return Return<T>();
            }
            
               
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public static T Divide<T>(T a, T b)
            {
                Emit.Ldarg(nameof(a));
                Emit.Ldarg(nameof(b));
                Emit.Div();
                return Return<T>();
            }
            
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public static T And<T>(T a, T b)
                where T : unmanaged
            {
                Emit.Ldarg(nameof(a));
                Emit.Ldarg(nameof(b));
                Emit.And();
                return Return<T>();
            }
            
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public static T Or<T>(T a, T b)
                where T : unmanaged
            {
                Emit.Ldarg(nameof(a));
                Emit.Ldarg(nameof(b));
                Emit.Or();
                return Return<T>();
            }
            
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public static T Xor<T>(T a, T b)
                where T : unmanaged
            {
                Emit.Ldarg(nameof(a));
                Emit.Ldarg(nameof(b));
                Emit.Xor();
                return Return<T>();
            }
            
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public static T Neg<T>(T value)
                where T : unmanaged
            {
                Emit.Ldarg(nameof(value));
                Emit.Neg();
                return Return<T>();
            }
            
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public static T Not<T>(T value)
                where T : unmanaged
            {
                Emit.Ldarg(nameof(value));
                Emit.Not();
                return Return<T>();
            }
            
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public static bool Equal<T>(T a, T b)
                where T : unmanaged
            {
                Emit.Ldarg(nameof(a));
                Emit.Ldarg(nameof(b));
                Emit.Ceq();
                return Return<bool>();
            }
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public static bool NotEqual<T>(T a, T b)
                where T : unmanaged
            {
                Emit.Ldarg(nameof(a));
                Emit.Ldarg(nameof(b));
                Emit.Ceq();
                Emit.Not();
                return Return<bool>();
            }
        }
    }

}