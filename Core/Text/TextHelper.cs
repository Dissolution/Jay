using Jay.Collections.Pools;
using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Text;
using InlineIL;
using Jay.Comparison;

namespace Jay.Text
{
    public static class TextHelper
    {
        private const string _digits = "0123456789";
        private const string _upperCase = "ABCDEFGHIJKLMNOPQRSTUVWXYZ";
        private const string _lowerCase = "abcdefghijklmnopqrstuvwxyz";
        private const string _letters = _upperCase + _lowerCase;
        private const string _alphanumeric = _digits + _upperCase + _lowerCase;
        private const string _blocks = "▖▗▘▙▚▛▜▝▞▟";

        public static ReadOnlySpan<char> Digits => _digits;
        public static ReadOnlySpan<char> UpperCase => _upperCase;
        public static ReadOnlySpan<char> LowerCase => _lowerCase;
        public static ReadOnlySpan<char> Letters => _letters;
        public static ReadOnlySpan<char> Alphanumeric => _alphanumeric;
        public static ReadOnlySpan<char> Blocks => _blocks;

        static TextHelper()
        {
            var thing = "▀▄▌▐";
            var thang = "▖▗▘▙▚▛▜▝▞▟";
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void Copy(ReadOnlySpan<char> source, Span<char> dest)
        {
            NotSafe.Unmanaged.BlockCopy<char>(in source.GetPinnableReference(),
                                              ref dest.GetPinnableReference(),
                                              source.Length);
        }
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void Copy(char[] source, Span<char> dest)
        {
            NotSafe.Unmanaged.BlockCopy<char>(in MemoryMarshal.GetArrayDataReference(source),
                                              ref dest.GetPinnableReference(),
                                              source.Length);
        }
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void Copy(string source, Span<char> dest)
        {
            NotSafe.Unmanaged.BlockCopy<char>(in source.GetPinnableReference(),
                                              ref dest.GetPinnableReference(),
                                              source.Length);
        }
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void Copy(ReadOnlySpan<char> source, char[] dest)
        {
            NotSafe.Unmanaged.BlockCopy<char>(in source.GetPinnableReference(),
                                              ref MemoryMarshal.GetArrayDataReference(dest),
                                              source.Length);
        }
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void Copy(char[] source, char[] dest)
        {
            NotSafe.Unmanaged.BlockCopy<char>(in MemoryMarshal.GetArrayDataReference(source),
                                              ref MemoryMarshal.GetArrayDataReference(dest),
                                              source.Length);
        }
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void Copy(char[] source, char[] dest, int count)
        {
            NotSafe.Unmanaged.BlockCopy<char>(in MemoryMarshal.GetArrayDataReference(source),
                                              ref MemoryMarshal.GetArrayDataReference(dest),
                                              count);
        }
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void Copy(string source, char[] dest)
        {
            NotSafe.Unmanaged.BlockCopy<char>(in source.GetPinnableReference(),
                                              ref MemoryMarshal.GetArrayDataReference(dest),
                                              source.Length);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool Equals(ReadOnlySpan<char> first, ReadOnlySpan<char> second) 
            => MemoryExtensions.SequenceEqual(first, second);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool Equals(ReadOnlySpan<char> first, ReadOnlySpan<char> second, StringComparison comparison)
            => MemoryExtensions.Equals(first, second, comparison);
    }
}