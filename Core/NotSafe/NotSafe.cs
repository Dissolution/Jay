using System;
using System.Runtime.CompilerServices;

using static InlineIL.IL;

namespace Jay
{
    /// <summary>
    /// 
    /// </summary>
    /// <see cref="https://github.com/ltrzesniewski/InlineIL.Fody/blob/master/src/InlineIL.Examples/Unsafe.cs"/>
    public static unsafe partial class NotSafe
    {
#region Read

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static T Read<T>(void* source)
        {
            Emit.Ldarg(nameof(source));
            Emit.Ldobj<T>();
            return Return<T>();
        }
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static T Read<T>(in byte source)
        {
            Emit.Ldarg(nameof(source));
            Emit.Ldobj<T>();
            return Return<T>();
        }
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static T ReadUnaligned<T>(void* source)
        {
            Emit.Ldarg(nameof(source));
            Emit.Unaligned(1);
            Emit.Ldobj<T>();
            return Return<T>();
        }
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static T ReadUnaligned<T>(in byte source)
        {
            Emit.Ldarg(nameof(source));
            Emit.Unaligned(1);
            Emit.Ldobj<T>();
            return Return<T>();
        }
   #endregion
        #region Write
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void Write<T>(void* destination, T value)
        {
            Emit.Ldarg(nameof(destination));
            Emit.Ldarg(nameof(value));
            Emit.Stobj<T>();
        }
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void WriteUnaligned<T>(void* destination, T value)
        {
            Emit.Ldarg(nameof(destination));
            Emit.Ldarg(nameof(value));
            Emit.Unaligned(1);
            Emit.Stobj<T>();
        }
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void Write<T>(ref byte destination, T value)
        {
            Emit.Ldarg(nameof(destination));
            Emit.Ldarg(nameof(value));
            Emit.Stobj<T>();
        }
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void WriteUnaligned<T>(ref byte destination, T value)
        {
            Emit.Ldarg(nameof(destination));
            Emit.Ldarg(nameof(value));
            Emit.Unaligned(1);
            Emit.Stobj<T>();
        }
#endregion
        
        #region Copy
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void Copy<T>(void* destination, void* source)
        {
            Emit.Ldarg(nameof(destination));
            Emit.Ldarg(nameof(source));
            Emit.Ldobj<T>();
            Emit.Stobj<T>();
        }
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void Copy<T>(void* destination, in T source)
        {
            Emit.Ldarg(nameof(destination));
            Emit.Ldarg(nameof(source));
            Emit.Ldobj<T>();
            Emit.Stobj<T>();
        }
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void Copy<T>(ref T destination, void* source)
        {
            Emit.Ldarg(nameof(destination));
            Emit.Ldarg(nameof(source));
            Emit.Ldobj<T>();
            Emit.Stobj<T>();
        }
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void Copy<T>(ref T destination, in T source)
        {
            Emit.Ldarg(nameof(destination));
            Emit.Ldarg(nameof(source));
            Emit.Ldobj<T>();
            Emit.Stobj<T>();
        }
        #endregion
        #region Copy Block
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void CopyBlock(void* destination, void* source, uint byteCount)
        {
            Emit.Ldarg(nameof(destination));
            Emit.Ldarg(nameof(source));
            Emit.Ldarg(nameof(byteCount));
            Emit.Cpblk();
        }
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void CopyBlock(void* destination, in byte source, uint byteCount)
        {
            Emit.Ldarg(nameof(destination));
            Emit.Ldarg(nameof(source));
            Emit.Ldarg(nameof(byteCount));
            Emit.Cpblk();
        }
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void CopyBlock(ref byte destination, void* source, uint byteCount)
        {
            Emit.Ldarg(nameof(destination));
            Emit.Ldarg(nameof(source));
            Emit.Ldarg(nameof(byteCount));
            Emit.Cpblk();
        }
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void CopyBlock(ref byte destination, in byte source, uint byteCount)
        {
            Emit.Ldarg(nameof(destination));
            Emit.Ldarg(nameof(source));
            Emit.Ldarg(nameof(byteCount));
            Emit.Cpblk();
        }
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void CopyBlockUnaligned(void* destination, void* source, uint byteCount)
        {
            Emit.Ldarg(nameof(destination));
            Emit.Ldarg(nameof(source));
            Emit.Ldarg(nameof(byteCount));
            Emit.Unaligned(1);
            Emit.Cpblk();
        }
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void CopyBlockUnaligned(void* destination, in byte source, uint byteCount)
        {
            Emit.Ldarg(nameof(destination));
            Emit.Ldarg(nameof(source));
            Emit.Ldarg(nameof(byteCount));
            Emit.Unaligned(1);
            Emit.Cpblk();
        }
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void CopyBlockUnaligned(ref byte destination, void* source, uint byteCount)
        {
            Emit.Ldarg(nameof(destination));
            Emit.Ldarg(nameof(source));
            Emit.Ldarg(nameof(byteCount));
            Emit.Unaligned(1);
            Emit.Cpblk();
        }
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void CopyBlockUnaligned(ref byte destination, ref byte source, uint byteCount)
        {
            Emit.Ldarg(nameof(destination));
            Emit.Ldarg(nameof(source));
            Emit.Ldarg(nameof(byteCount));
            Emit.Unaligned(1);
            Emit.Cpblk();
        }

        #endregion
        #region Init Block
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void InitBlock(void* startAddress, byte value, uint byteCount)
        {
            Emit.Ldarg(nameof(startAddress));
            Emit.Ldarg(nameof(value));
            Emit.Ldarg(nameof(byteCount));
            Emit.Initblk();
        }
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void InitBlock(ref byte startAddress, byte value, uint byteCount)
        {
            Emit.Ldarg(nameof(startAddress));
            Emit.Ldarg(nameof(value));
            Emit.Ldarg(nameof(byteCount));
            Emit.Initblk();
        }
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void InitBlockUnaligned(void* startAddress, byte value, uint byteCount)
        {
            Emit.Ldarg(nameof(startAddress));
            Emit.Ldarg(nameof(value));
            Emit.Ldarg(nameof(byteCount));
            Emit.Unaligned(1);
            Emit.Initblk();
        }
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void InitBlockUnaligned(ref byte startAddress, byte value, uint byteCount)
        {
            Emit.Ldarg(nameof(startAddress));
            Emit.Ldarg(nameof(value));
            Emit.Ldarg(nameof(byteCount));
            Emit.Unaligned(1);
            Emit.Initblk();
        }
        #endregion
        
        #region As
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static TClass As<TClass>(object obj)
            where TClass : class
        {
            Emit.Ldarg(nameof(obj));
            return Return<TClass>();
        }
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static ref TTo As<TFrom, TTo>(ref TFrom source)
        {
            Emit.Ldarg(nameof(source));
            return ref ReturnRef<TTo>();
        }
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static TTo As<TFrom, TTo>(TFrom source)
        {
            Emit.Ldarg(nameof(source));
            return Return<TTo>();
        }
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static ref T AsRef<T>(void* source)
        {
            Push(source);
            return ref ReturnRef<T>();
        }
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static ref T AsRef<T>(in byte source)
        {
            Emit.Ldarg(nameof(source));
            return ref ReturnRef<T>();
        }
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static ref T AsRef<T>(in T source)
        {
            Emit.Ldarg(nameof(source));
            return ref ReturnRef<T>();
        }

#region AsPointer
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void* AsVoidPointer<T>(ref T value)
        {
            Emit.Ldarg(nameof(value));
            Emit.Conv_U();
            return ReturnPointer();
        }
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void* AsVoidPointer<T>(T* value)
            where T : unmanaged
        {
            Emit.Ldarg(nameof(value));
            Emit.Conv_U();
            return ReturnPointer();
        }
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static T* AsPointer<T>(ref T value)
            where T : unmanaged
        {
            Emit.Ldarg(nameof(value));
            return ReturnPointer<T>();
        }
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static TOut* AsPointer<TIn, TOut>(ref TIn value)
            where TIn : unmanaged
            where TOut : unmanaged
        {
            Emit.Ldarg(nameof(value));
            return ReturnPointer<TOut>();
        }
        #endregion
        #endregion

        #region Box / Unbox

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static object Box<T>(T value)
        {
            Emit.Ldarg(nameof(value));
            Emit.Box<T>();
            return Return<object>();
        }
        
       [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static ref T Unbox<T>(object box)
            where T : struct
        {
            Push(box);
            Emit.Unbox<T>();
            return ref ReturnRef<T>();
        }
        #endregion
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void SkipInit<T>(out T? value)
        {
            Emit.Ret();
            throw Unreachable();
        }
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int SizeOf<T>()
        {
            Emit.Sizeof<T>();
            return Return<int>();
        }
        
        #region Add
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void* Add<T>(void* source, int elementOffset)
        {
            Emit.Ldarg(nameof(source));
            Emit.Ldarg(nameof(elementOffset));
            Emit.Sizeof(typeof(T));
            Emit.Conv_I();
            Emit.Mul();
            Emit.Add();
            return ReturnPointer();
        }


        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static ref T Add<T>(ref T source, IntPtr elementOffset)
        {
            Emit.Ldarg(nameof(source));
            Emit.Ldarg(nameof(elementOffset));
            Emit.Sizeof(typeof(T));
            Emit.Mul();
            Emit.Add();
            return ref ReturnRef<T>();
        }
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static ref T Add<T>(ref T source, nuint elementOffset)
        {
            Emit.Ldarg(nameof(source));
            Emit.Ldarg(nameof(elementOffset));
            Emit.Sizeof(typeof(T));
            Emit.Mul();
            Emit.Add();
            return ref ReturnRef<T>();
        }
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static ref T AddByteOffset<T>(ref T source, IntPtr byteOffset)
        {
            Emit.Ldarg(nameof(source));
            Emit.Ldarg(nameof(byteOffset));
            Emit.Add();
            return ref ReturnRef<T>();
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static ref T AddByteOffset<T>(ref T source, nuint byteOffset)
        {
            Emit.Ldarg(nameof(source));
            Emit.Ldarg(nameof(byteOffset));
            Emit.Add();
            return ref ReturnRef<T>();
        }
        #endregion
        #region Subtract
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static ref T Subtract<T>(ref T source, int elementOffset)
        {
            Emit.Ldarg(nameof(source));
            Emit.Ldarg(nameof(elementOffset));
            Emit.Sizeof(typeof(T));
            Emit.Conv_I();
            Emit.Mul();
            Emit.Sub();
            return ref ReturnRef<T>();
        }
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void* Subtract<T>(void* source, int elementOffset)
        {
            Emit.Ldarg(nameof(source));
            Emit.Ldarg(nameof(elementOffset));
            Emit.Sizeof(typeof(T));
            Emit.Conv_I();
            Emit.Mul();
            Emit.Sub();
            return ReturnPointer();
        }
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static ref T Subtract<T>(ref T source, IntPtr elementOffset)
        {
            Emit.Ldarg(nameof(source));
            Emit.Ldarg(nameof(elementOffset));
            Emit.Sizeof(typeof(T));
            Emit.Mul();
            Emit.Sub();
            return ref ReturnRef<T>();
        }
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static ref T Subtract<T>(ref T source, nuint elementOffset)
        {
            Emit.Ldarg(nameof(source));
            Emit.Ldarg(nameof(elementOffset));
            Emit.Sizeof(typeof(T));
            Emit.Mul();
            Emit.Sub();
            return ref ReturnRef<T>();
        }
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static ref T SubtractByteOffset<T>(ref T source, IntPtr byteOffset)
        {
            Emit.Ldarg(nameof(source));
            Emit.Ldarg(nameof(byteOffset));
            Emit.Sub();
            return ref ReturnRef<T>();
        }
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static ref T SubtractByteOffset<T>(ref T source, nuint byteOffset)
        {
            Emit.Ldarg(nameof(source));
            Emit.Ldarg(nameof(byteOffset));
            Emit.Sub();
            return ref ReturnRef<T>();
        }
        #endregion
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static IntPtr ByteOffset<T>(ref T origin, ref T target)
        {
            Emit.Ldarg(nameof(target));
            Emit.Ldarg(nameof(origin));
            Emit.Sub();
            return Return<IntPtr>();
        }
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool AreSame<T>(ref T left, ref T right)
        {
            Emit.Ldarg(nameof(left));
            Emit.Ldarg(nameof(right));
            Emit.Ceq();
            return Return<bool>();
        }
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool IsAddressGreaterThan<T>(ref T left, ref T right)
        {
            Emit.Ldarg(nameof(left));
            Emit.Ldarg(nameof(right));
            Emit.Cgt_Un();
            return Return<bool>();
        }
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool IsAddressLessThan<T>(ref T left, ref T right)
        {
            Emit.Ldarg(nameof(left));
            Emit.Ldarg(nameof(right));
            Emit.Clt_Un();
            return Return<bool>();
        }
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool IsNullRef<T>(ref T source)
        {
            Emit.Ldarg(nameof(source));
            Emit.Ldc_I4_0();
            Emit.Conv_U();
            Emit.Ceq();
            return Return<bool>();
        }
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static ref T NullRef<T>()
        {
            Emit.Ldc_I4_0();
            Emit.Conv_U();
            return ref ReturnRef<T>();
        }
    }
}