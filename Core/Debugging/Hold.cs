using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace Jay.Debugging
{
    /// <summary>
    /// A utility for 'holding' onto references so they can be examined during debugging without the compiler optimizing them out.
    /// </summary>
    public static class Hold
    {
        [Conditional("DEBUG")]
        public static void Debug<T1>(T1? value1) { }
        [Conditional("DEBUG")]
        public static void Debug<T1, T2>(T1? value1, T2? value2) { }
        [Conditional("DEBUG")]
        public static void Debug<T1, T2, T3>(T1? value1, T2? value2, T3? value3) { }
        [Conditional("DEBUG")]
        public static void Debug<T1, T2, T3, T4>(T1? value1, T2? value2, T3? value3, T4? value4) { }
        [Conditional("DEBUG")]
        public static void Debug<T1, T2, T3, T4, T5>(T1? value1, T2? value2, T3? value3, T4? value4, T5? value5) { }
        [Conditional("DEBUG")]
        public static void Debug<T1, T2, T3, T4, T5, T6>(T1? value1, T2? value2, T3? value3, T4? value4, T5? value5, T6? value6) { }
        [Conditional("DEBUG")]
        public static void Debug<T1, T2, T3, T4, T5, T6, T7>(T1? value1, T2? value2, T3? value3, T4? value4, T5? value5, T6? value6, T7? value7) { }
        [Conditional("DEBUG")]
        public static void Debug<T1, T2, T3, T4, T5, T6, T7, T8>(T1? value1, T2? value2, T3? value3, T4? value4, T5? value5, T6? value6, T7? value7, T8? value8) { }

        [Conditional("DEBUG")]
        public static void Debug<T>(params T?[]? values) { }
        [Conditional("DEBUG")]
        public static void Debug<T>(IEnumerable<T?>? values) { }
        [Conditional("DEBUG")]
        public static void Debug<T>(Span<T> span) { }
        [Conditional("DEBUG")]
        public static void Debug<T>(ReadOnlySpan<T> readOnlySpan) { }
    }
}