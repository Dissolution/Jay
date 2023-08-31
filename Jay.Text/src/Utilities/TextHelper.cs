// ReSharper disable InvokeAsExtensionMethod
// ^ I want to be sure I'm calling the very specific version of a method

#if !(NET48 || NETSTANDARD2_0)
using System.Runtime.InteropServices;
#endif
using Jay.Text.Building;
using Jay.Text.Comparision;
using Jay.Utilities;
using static InlineIL.IL;

namespace Jay.Text.Utilities;

/// <summary>
/// A utility class for working with text,
/// including <see cref="char"/>,
/// <see cref="string"/>,
/// <see cref="Array">char[]</see>,
/// and <see cref="ReadOnlySpan{T}">ReadOnlySpan&lt;char&gt;</see>
/// </summary>
public static class TextHelper
{
    /// <summary>
    /// Use an <see cref="InterpolatedTextBuilder"/> to render a formattable string
    /// </summary>
    public static string Interpolate(this ref InterpolatedTextBuilder text)
    {
        return text.ToStringAndDispose();
    }

#region CopyTo

    /// <summary>
    /// Copies the <paramref name="source"/> text to <paramref name="dest"/>
    /// </summary>
    public static void CopyTo(char[]? source, char[]? dest)
    {
        if (!TryCopyTo(source, dest))
        {
            throw new InvalidOperationException(
                Interpolate($"Cannot copy source char[{source.Length}] \"{source}\" to destination char[{dest?.Length}]"));
        }
    }
    
    /// <summary>
    /// Copies the <paramref name="source"/> text to <paramref name="dest"/>
    /// </summary>
    public static void CopyTo(ReadOnlySpan<char> source, char[]? dest)
    {
        if (!TryCopyTo(source, dest))
        {
            throw new InvalidOperationException(
                Interpolate($"Cannot copy source ReadOnlySpan<char> \"{source}\" [{source.Length}] to destination char[{dest?.Length}]"));
        }
    }

    /// <summary>
    /// Copies the <paramref name="source"/> text to <paramref name="dest"/>
    /// </summary>
    public static void CopyTo(string? source, char[]? dest)
    {
        if (!TryCopyTo(source, dest))
        {
            throw new InvalidOperationException(
                Interpolate($"Cannot copy source string \"{source}\" [{source.Length}] to destination char[{dest?.Length}]"));
        }
    }
    
    /// <summary>
    /// Copies the <paramref name="source"/> text to <paramref name="dest"/>
    /// </summary>
    public static void CopyTo(char[]? source, Span<char> dest)
    {
        if (!TryCopyTo(source, dest))
        {
            throw new InvalidOperationException(
                Interpolate($"Cannot copy source char[{source!.Length}] \"{source}\" to destination Span<char>[{dest.Length}]"));
        }
    }
    
    /// <summary>
    /// Copies the <paramref name="source"/> text to <paramref name="dest"/>
    /// </summary>
    public static void CopyTo(ReadOnlySpan<char> source, Span<char> dest)
    {
        if (!TryCopyTo(source, dest))
        {
            throw new InvalidOperationException(
                Interpolate($"Cannot copy source ReadOnlySpan<char> \"{source}\" [{source.Length}] to destination Span<char>[{dest.Length}]"));
        }
    }

    /// <summary>
    /// Copies the <paramref name="source"/> text to <paramref name="dest"/>
    /// </summary>
    public static void CopyTo(string? source, Span<char> dest)
    {
        if (!TryCopyTo(source, dest))
        {
            throw new InvalidOperationException(
                Interpolate($"Cannot copy source string \"{source}\" [{source?.Length}] to destination Span<char>[{dest.Length}]"));
        }
    }

    /// <summary>
    /// Try to copy the text in <paramref name="source"/> to <paramref name="dest"/>
    /// </summary>
    public static bool TryCopyTo([NotNullWhen(false)] char[]? source, char[]? dest)
    {
        if (source is null)
            return true;
        var sourceLen = source.Length;
        if (sourceLen == 0)
            return true;
        if (dest is null)
            return false;
        if (sourceLen > dest.Length)
            return false;
        Unsafe.CopyBlock(
            in source.GetPinnableReference(),
            ref dest.GetPinnableReference(),
            sourceLen);
        return true;
    }

    /// <summary>
    /// Try to copy the text in <paramref name="source"/> to <paramref name="dest"/>
    /// </summary>
    public static bool TryCopyTo(ReadOnlySpan<char> source, char[]? dest)
    {
        var sourceLen = source.Length;
        if (sourceLen == 0)
            return true;
        if (dest is null)
            return false;
        if (sourceLen > dest.Length)
            return false;
        Unsafe.CopyBlock(
            in source.GetPinnableReference(),
            ref dest.GetPinnableReference(),
            sourceLen);
        return true;
    }

    /// <summary>
    /// Try to copy the text in <paramref name="source"/> to <paramref name="dest"/>
    /// </summary>
    public static bool TryCopyTo([NotNullWhen(false)] string? source, char[]? dest)
    {
        if (source is null)
            return true;
        var sourceLen = source.Length;
        if (sourceLen == 0)
            return true;
        if (dest is null)
            return false;
        if (sourceLen > dest.Length)
            return false;
        Unsafe.CopyBlock(
            in source.GetPinnableReference(),
            ref dest.GetPinnableReference(),
            sourceLen);
        return true;
    }
    
    /// <summary>
    /// Try to copy the text in <paramref name="source"/> to <paramref name="dest"/>
    /// </summary>
    public static bool TryCopyTo([NotNullWhen(false)] char[]? source, Span<char> dest)
    {
        if (source is null)
            return true;
        var sourceLen = source.Length;
        if (sourceLen == 0)
            return true;
        if (sourceLen > dest.Length)
            return false;
        Unsafe.CopyBlock(
            in source.GetPinnableReference(),
            ref dest.GetPinnableReference(),
            sourceLen);
        return true;
    }

    /// <summary>
    /// Try to copy the text in <paramref name="source"/> to <paramref name="dest"/>
    /// </summary>
    public static bool TryCopyTo(ReadOnlySpan<char> source, Span<char> dest)
    {
        var sourceLen = source.Length;
        if (sourceLen == 0)
            return true;
        if (sourceLen > dest.Length)
            return false;
        Unsafe.CopyBlock(
            in source.GetPinnableReference(),
            ref dest.GetPinnableReference(),
            sourceLen);
        return true;
    }

    /// <summary>
    /// Try to copy the text in <paramref name="source"/> to <paramref name="dest"/>
    /// </summary>
    public static bool TryCopyTo(string? source, Span<char> dest)
    {
        if (source is null)
            return true;
        var sourceLen = source.Length;
        if (sourceLen == 0)
            return true;
        if (sourceLen > dest.Length)
            return false;
        Unsafe.CopyBlock(
            in source.GetPinnableReference(),
            ref dest.GetPinnableReference(),
            sourceLen);
        return true;
    }

#endregion
    
    #region Unsafe

    public static class Unsafe
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal static void CopyBlock(
            in char sourcePtr, ref char destPtr,
            int charCount)
        {
            Emit.Ldarg(nameof(destPtr));
            Emit.Ldarg(nameof(sourcePtr));
            Emit.Ldarg(nameof(charCount));
            Emit.Sizeof<char>();
            Emit.Mul();
            Emit.Cpblk();
        }
        
        /// <summary>
        /// This is dangerous!
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Span<char> AsSpan(in string text)
        {
            ref readonly char ch = ref text.GetPinnableReference();
#if NET48 || NETSTANDARD2_0
            unsafe
            {
                return new Span<char>(Scary.InToVoidPointer(in ch), text.Length);
            }
#else
            return MemoryMarshal.CreateSpan<char>(ref Scary.InToRef(in ch), text.Length);
#endif
        }
    }
    #endregion
}