using Jay.Collections.Pools;
using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Text;
using InlineIL;

namespace Jay.Text
{
    public static class TextHelper
    {
        private const string _digits = "0123456789";
        private const string _upperCase = "ABCDEFGHIJKLMNOPQRSTUVWXYZ";
        private const string _lowerCase = "abcdefghijklmnopqrstuvwxyz";

        public static ReadOnlySpan<char> Digits => _digits;
        public static ReadOnlySpan<char> UpperCase => _upperCase;
        public static ReadOnlySpan<char> LowerCase => _lowerCase;
        
        static TextHelper()
        {
           
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void Copy(ReadOnlySpan<char> source,
                                Span<char> dest)
        {
            if (source.Length > dest.Length)
                throw new ArgumentOutOfRangeException(nameof(dest));
            // unsafe
            // {
            //     fixed (char* sourcePtr = source)
            //     fixed (char* destPtr = dest)
            //     {
            //         Buffer.MemoryCopy(sourcePtr, destPtr, dest.Length * sizeof(char), source.Length * sizeof(char));
            //     }
            // }
            NotSafe.Unmanaged.BlockCopy<char>(in source.GetPinnableReference(),
                                              ref dest.GetPinnableReference(),
                                              source.Length);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool Equals(ReadOnlySpan<char> first, ReadOnlySpan<char> second) 
            => MemoryExtensions.SequenceEqual(first, second);
    }
}