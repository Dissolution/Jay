using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;

namespace Jay.Debugging
{
    internal static class Hold
    {
        [Conditional("DEBUG")]
        public static void Debug<T1>([AllowNull] T1 value1) { }
        [Conditional("DEBUG")]
        public static void Debug<T1, T2>([AllowNull] T1 value1, [AllowNull] T2 value2) { }
        [Conditional("DEBUG")]
        public static void Debug<T1, T2, T3>([AllowNull] T1 value1, [AllowNull] T2 value2, [AllowNull] T3 value3) { }
        
        [Conditional("DEBUG")]
        public static void Debug<T>(ReadOnlySpan<T?> values) { }
        [Conditional("DEBUG")]
        public static void Debug<T>(Span<T?> values) { }
        [Conditional("DEBUG")]
        public static void Debug<T>(params T?[] values) { }
        [Conditional("DEBUG")]
        public static void Debug<T>(IEnumerable<T?> values) { }

    }
}