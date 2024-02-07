// ReSharper disable EntityNameCapturedOnly.Local
// ReSharper disable InvokeAsExtensionMethod
// ^ I want to be sure I'm calling the very specific version of a method

#if !(NET48 || NETSTANDARD2_0)
using System.Runtime.InteropServices;
#endif
using Jay.Text.Building;
using static InlineIL.IL;
// ReSharper disable EntityNameCapturedOnly.Global

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
            throw new InvalidOperationException(Interpolate($"Cannot copy source char[{source.Length}] \"{source}\" to destination char[{dest?.Length}]"));
        }
    }

    /// <summary>
    /// Copies the <paramref name="source"/> text to <paramref name="dest"/>
    /// </summary>
    public static void CopyTo(ReadOnlySpan<char> source, char[]? dest)
    {
        if (!TryCopyTo(source, dest))
        {
            throw new InvalidOperationException(Interpolate($"Cannot copy source ReadOnlySpan<char> \"{source}\" [{source.Length}] to destination char[{dest?.Length}]"));
        }
    }

    /// <summary>
    /// Copies the <paramref name="source"/> text to <paramref name="dest"/>
    /// </summary>
    public static void CopyTo(string? source, char[]? dest)
    {
        if (!TryCopyTo(source, dest))
        {
            throw new InvalidOperationException(Interpolate($"Cannot copy source string \"{source}\" [{source.Length}] to destination char[{dest?.Length}]"));
        }
    }

    /// <summary>
    /// Copies the <paramref name="source"/> text to <paramref name="dest"/>
    /// </summary>
    public static void CopyTo(char[]? source, Span<char> dest)
    {
        if (!TryCopyTo(source, dest))
        {
            throw new InvalidOperationException(Interpolate($"Cannot copy source char[{source!.Length}] \"{source}\" to destination Span<char>[{dest.Length}]"));
        }
    }

    /// <summary>
    /// Copies the <paramref name="source"/> text to <paramref name="dest"/>
    /// </summary>
    public static void CopyTo(ReadOnlySpan<char> source, Span<char> dest)
    {
        if (!TryCopyTo(source, dest))
        {
            throw new InvalidOperationException(Interpolate($"Cannot copy source ReadOnlySpan<char> \"{source}\" [{source.Length}] to destination Span<char>[{dest.Length}]"));
        }
    }

    /// <summary>
    /// Copies the <paramref name="source"/> text to <paramref name="dest"/>
    /// </summary>
    public static void CopyTo(string? source, Span<char> dest)
    {
        if (!TryCopyTo(source, dest))
        {
            throw new InvalidOperationException(Interpolate($"Cannot copy source string \"{source}\" [{source?.Length}] to destination Span<char>[{dest.Length}]"));
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
            source,
            dest,
            sourceLen);
        return true;
    }

    /// <summary>
    /// Try to copy the text in <paramref name="source"/> to <paramref name="dest"/>
    /// </summary>
    public static bool TryCopyTo(scoped ReadOnlySpan<char> source, char[]? dest)
    {
        var sourceLen = source.Length;
        if (sourceLen == 0)
            return true;
        if (dest is null)
            return false;
        if (sourceLen > dest.Length)
            return false;

        Unsafe.CopyBlock(
            source,
            dest,
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
            source,
            dest,
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
            source,
            dest,
            sourceLen);
        return true;
    }

    /// <summary>
    /// Try to copy the text in <paramref name="source"/> to <paramref name="dest"/>
    /// </summary>
    public static bool TryCopyTo(scoped ReadOnlySpan<char> source, Span<char> dest)
    {
        var sourceLen = source.Length;
        if (sourceLen == 0)
            return true;
        if (sourceLen > dest.Length)
            return false;

        Unsafe.CopyBlock(
            source,
            dest,
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
            source,
            dest,
            sourceLen);
        return true;
    }
#endregion
    
    /// <summary>
    /// WARNING: All methods in <see cref="Unsafe"/> lack bounds checking!
    /// </summary>
    public static class Unsafe
    {
        /* Input:<br/>
         * <c>in char</c>, <c>char[]</c>, <c>ReadOnlySpan&lt;char&gt;</c>, <c>string</c><br/>
         * Output:<br/>
         * <c>ref char</c>, <c>char[]</c>, <c>Span&lt;char&gt;<c/>
         */

        /// <summary>
        /// Copies the block of <paramref name="count"/>  <see cref="char">characters</see>
        /// from <paramref name="source"/> to <paramref name="destination"/>
        /// </summary>
        /// <param name="source">The source readable text</param>
        /// <param name="destination">The destination writable text</param>
        /// <param name="count">The number of characters to copy</param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void CopyBlock(
            in char source,
            ref char destination,
            int count)
        {
            Emit.Ldarg(nameof(destination));
            Emit.Ldarg(nameof(source));
            Emit.Ldarg(nameof(count));
            Emit.Sizeof<char>();
            Emit.Mul();
            Emit.Cpblk();
        }

        /// <summary>
        /// Copies the block of <paramref name="count"/>  <see cref="char">characters</see>
        /// from <paramref name="source"/> to <paramref name="destination"/>
        /// </summary>
        /// <param name="source">The source readable text</param>
        /// <param name="destination">The destination writable text</param>
        /// <param name="count">The number of characters to copy</param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void CopyBlock(
            in char source,
            char[] destination,
            int count)
        {
            CopyBlock(in source, ref destination.GetPinnableReference(), count);
        }

        /// <summary>
        /// Copies the block of <paramref name="count"/>  <see cref="char">characters</see>
        /// from <paramref name="source"/> to <paramref name="destination"/>
        /// </summary>
        /// <param name="source">The source readable text</param>
        /// <param name="destination">The destination writable text</param>
        /// <param name="count">The number of characters to copy</param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void CopyBlock(
            in char source,
            Span<char> destination,
            int count)
        {
            CopyBlock(in source, ref destination.GetPinnableReference(), count);
        }

        /// <summary>
        /// Copies the block of <paramref name="count"/>  <see cref="char">characters</see>
        /// from <paramref name="source"/> to <paramref name="destination"/>
        /// </summary>
        /// <param name="source">The source readable text</param>
        /// <param name="destination">The destination writable text</param>
        /// <param name="count">The number of characters to copy</param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void CopyBlock(
            char[] source,
            ref char destination,
            int count)
        {
            CopyBlock(in source.GetPinnableReference(), ref destination, count);
        }

        /// <summary>
        /// Copies the block of <paramref name="count"/>  <see cref="char">characters</see>
        /// from <paramref name="source"/> to <paramref name="destination"/>
        /// </summary>
        /// <param name="source">The source readable text</param>
        /// <param name="destination">The destination writable text</param>
        /// <param name="count">The number of characters to copy</param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void CopyBlock(
            char[] source,
            char[] destination,
            int count)
        {
            CopyBlock(in source.GetPinnableReference(), ref destination.GetPinnableReference(), count);
        }

        /// <summary>
        /// Copies the block of <paramref name="count"/>  <see cref="char">characters</see>
        /// from <paramref name="source"/> to <paramref name="destination"/>
        /// </summary>
        /// <param name="source">The source readable text</param>
        /// <param name="destination">The destination writable text</param>
        /// <param name="count">The number of characters to copy</param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void CopyBlock(
            char[] source,
            Span<char> destination,
            int count)
        {
            CopyBlock(in source.GetPinnableReference(), ref destination.GetPinnableReference(), count);
        }

        /// <summary>
        /// Copies the block of <paramref name="count"/>  <see cref="char">characters</see>
        /// from <paramref name="source"/> to <paramref name="destination"/>
        /// </summary>
        /// <param name="source">The source readable text</param>
        /// <param name="destination">The destination writable text</param>
        /// <param name="count">The number of characters to copy</param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void CopyBlock(
            ReadOnlySpan<char> source,
            ref char destination,
            int count)
        {
            CopyBlock(in source.GetPinnableReference(), ref destination, count);
        }

        /// <summary>
        /// Copies the block of <paramref name="count"/>  <see cref="char">characters</see>
        /// from <paramref name="source"/> to <paramref name="destination"/>
        /// </summary>
        /// <param name="source">The source readable text</param>
        /// <param name="destination">The destination writable text</param>
        /// <param name="count">The number of characters to copy</param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void CopyBlock(
            ReadOnlySpan<char> source,
            char[] destination,
            int count)
        {
            CopyBlock(in source.GetPinnableReference(), ref destination.GetPinnableReference(), count);
        }

        /// <summary>
        /// Copies the block of <paramref name="count"/>  <see cref="char">characters</see>
        /// from <paramref name="source"/> to <paramref name="destination"/>
        /// </summary>
        /// <param name="source">The source readable text</param>
        /// <param name="destination">The destination writable text</param>
        /// <param name="count">The number of characters to copy</param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void CopyBlock(
            ReadOnlySpan<char> source,
            Span<char> destination,
            int count)
        {
            CopyBlock(in source.GetPinnableReference(), ref destination.GetPinnableReference(), count);
        }

        /// <summary>
        /// Copies the block of <paramref name="count"/>  <see cref="char">characters</see>
        /// from <paramref name="source"/> to <paramref name="destination"/>
        /// </summary>
        /// <param name="source">The source readable text</param>
        /// <param name="destination">The destination writable text</param>
        /// <param name="count">The number of characters to copy</param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void CopyBlock(
            string source,
            ref char destination,
            int count)
        {
            CopyBlock(in source.GetPinnableReference(), ref destination, count);
        }

        /// <summary>
        /// Copies the block of <paramref name="count"/>  <see cref="char">characters</see>
        /// from <paramref name="source"/> to <paramref name="destination"/>
        /// </summary>
        /// <param name="source">The source readable text</param>
        /// <param name="destination">The destination writable text</param>
        /// <param name="count">The number of characters to copy</param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void CopyBlock(
            string source,
            char[] destination,
            int count)
        {
            CopyBlock(in source.GetPinnableReference(), ref destination.GetPinnableReference(), count);
        }

        /// <summary>
        /// Copies the block of <paramref name="count"/>  <see cref="char">characters</see>
        /// from <paramref name="source"/> to <paramref name="destination"/>
        /// </summary>
        /// <param name="source">The source readable text</param>
        /// <param name="destination">The destination writable text</param>
        /// <param name="count">The number of characters to copy</param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void CopyBlock(
            string source,
            Span<char> destination,
            int count)
        {
            CopyBlock(in source.GetPinnableReference(), ref destination.GetPinnableReference(), count);
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
}