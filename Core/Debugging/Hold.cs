using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using Jay.Debugging.Dumping;
using Jay.Reflection;
using Jay.Reflection.Emission;
using Jay.Text;

namespace Jay.Debugging
{
    /// <summary>
    /// A utility for 'holding' onto references so they can be examined during debugging without the compiler optimizing them out.
    /// </summary>
    public static class Hold
    {
        [Conditional("DEBUG")]
        public static void Debug() { }
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
        
        public static string Dump(object? value, MemberDumpOptions? options = default)
        {
            if (value is null) return "null";
            Type valueType = value.GetType();
            using (var text = TextBuilder.Rent())
            {
                text.AppendDump(valueType, options)
                    .Append(" \"")
                    .Append(value.ToString())
                    .Append('"')
                    .AppendNewLine();
                foreach (var property in valueType.GetProperties(Reflect.InstanceFlags))
                {
                    text.Append(" .")
                        .Append(property.Name)
                        .Append(": ")
                        .AppendDump(property.GetValue(value), options)
                        .AppendNewLine();
                }
                return text;
            }
        }
    }
}