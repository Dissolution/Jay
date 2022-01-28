using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;

using static InlineIL.IL;

// ReSharper disable UnusedMember.Global

namespace Jay.Reflection;

public static unsafe class Danger
{
    #region Read / Write
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static T Read<T>(void* source)
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
    public static void WriteUnaligned<T>(ref byte destination, T value)
    {
        Emit.Ldarg(nameof(destination));
        Emit.Ldarg(nameof(value));
        Emit.Unaligned(1);
        Emit.Stobj<T>();
    }
    #endregion

    #region CopyTo
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void CopyTo<T>(in T source, void* destination)
    {
        Emit.Ldarg(nameof(destination));
        Emit.Ldarg(nameof(source));
        Emit.Ldobj<T>();
        Emit.Stobj<T>();
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void CopyTo<T>(void* source, ref T destination)
    {
        Emit.Ldarg(nameof(destination));
        Emit.Ldarg(nameof(source));
        Emit.Ldobj<T>();
        Emit.Stobj<T>();
    }


    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void CopyTo<TIn, TOut>(in TIn source, ref TOut destination)
    {
        Emit.Ldarg(nameof(destination));
        Emit.Ldarg(nameof(source));
        Emit.Ldobj<TIn>();
        Emit.Stobj<TOut>();
    }
    #endregion

    #region As / Cast / Box / Unbox

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void* AsVoidPointer<T>(in T value)
    {
        Emit.Ldarg(nameof(value));
        Emit.Conv_U();
        return ReturnPointer();
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static T* AsPointer<T>(in T value)
        where T : unmanaged
    {
        Emit.Ldarg(nameof(value));
        return ReturnPointer<T>();
    }
    
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static ref T AsRef<T>(void* source)
    {
        Push(source);
        return ref ReturnRef<T>();
    }
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static ref T AsRef<T>(T* source)
        where T : unmanaged
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

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    [return: NotNullIfNotNull("obj")]
    public static T? As<T>(object? obj)
        where T : class
    {
        Emit.Ldarg(nameof(obj));
        // Emit.Castclass<T>();
        return Return<T>();
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static TOut As<TIn, TOut>(TIn input)
    {
        Emit.Ldarg(nameof(input));
        return Return<TOut>();
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static ref TOut As<TIn, TOut>(ref TIn source)
    {
        Emit.Ldarg(nameof(source));
        return ref ReturnRef<TOut>();
    }


    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static ref T Unbox<T>(object box)
        where T : struct
    {
        //Push(box);
        Emit.Ldarg(nameof(box));
        Emit.Unbox<T>();
        return ref ReturnRef<T>();
    }
    #endregion


    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void SkipInit<T>(out T value)
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

    #region Blocks (byte[])
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void CopyToBlock(void* source, void* destination, uint byteCount)
    {
        Emit.Ldarg(nameof(destination));
        Emit.Ldarg(nameof(source));
        Emit.Ldarg(nameof(byteCount));
        Emit.Cpblk();
    }
    
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void CopyToBlock(in byte source, ref byte destination, uint byteCount)
    {
        Emit.Ldarg(nameof(destination));
        Emit.Ldarg(nameof(source));
        Emit.Ldarg(nameof(byteCount));
        Emit.Cpblk();
    }


    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void CopyToBlockUnaligned(void* source, void* destination, uint byteCount)
    {
        Emit.Ldarg(nameof(destination));
        Emit.Ldarg(nameof(source));
        Emit.Ldarg(nameof(byteCount));
        Emit.Unaligned(1);
        Emit.Cpblk();
    }


    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void CopyToBlockUnaligned(in byte source, ref byte destination, uint byteCount)
    {
        Emit.Ldarg(nameof(destination));
        Emit.Ldarg(nameof(source));
        Emit.Ldarg(nameof(byteCount));
        Emit.Unaligned(1);
        Emit.Cpblk();
    }


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

    #region Offsets
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void* OffsetBy<T>(void* source, int elementOffset)
    {
        Emit.Ldarg(nameof(source));
        Emit.Ldarg(nameof(elementOffset));
        Emit.Sizeof<T>();
        Emit.Conv_I();
        Emit.Mul();
        Emit.Add();
        return ReturnPointer();
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void* OffsetBy<T>(T* source, int elementOffset)
        where T : unmanaged
    {
        Emit.Ldarg(nameof(source));
        Emit.Ldarg(nameof(elementOffset));
        Emit.Sizeof<T>();
        Emit.Conv_I();
        Emit.Mul();
        Emit.Add();
        return ReturnPointer();
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static ref T OffsetBy<T>(ref T source, int elementOffset)
    {
        Emit.Ldarg(nameof(source));
        Emit.Ldarg(nameof(elementOffset));
        Emit.Sizeof<T>();
        Emit.Conv_I();
        Emit.Mul();
        Emit.Add();
        return ref ReturnRef<T>();
    }
    
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static ref T OffsetBy<T>(ref T source, nint elementOffset)
    {
        Emit.Ldarg(nameof(source));
        Emit.Ldarg(nameof(elementOffset));
        Emit.Sizeof<T>();
        Emit.Mul();
        Emit.Add();
        return ref ReturnRef<T>();
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void* OffsetByBytes(void* source, int byteOffset)
    {
        Emit.Ldarg(nameof(source));
        Emit.Ldarg(nameof(byteOffset));
        Emit.Add();
        return ReturnPointer();
    }
#endregion

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
