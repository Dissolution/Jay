using Jay.Collections;
using Jay.Text;
using System;
using System.Collections;
using System.Collections.Generic;

namespace Jay.Debugging.Dumping
{
    public static partial class Dumper
    {
        public static TextBuilder AppendDump(this TextBuilder textBuilder, IEnumerable? enumerable, DumpOptions options = default)
        {
            textBuilder.Write('(');
            if (enumerable != null)
            {
                textBuilder.AppendDelimit<object?>(", ", enumerable.AsObjectEnumerable(), (tb, value) => tb.AppendDump(value, options));
            }
            return textBuilder.Append(')');
        }
        
        public static TextBuilder AppendDump(this TextBuilder textBuilder, Array? array, DumpOptions options = default)
        {
            textBuilder.Write('[');
            if (array != null)
            {
                textBuilder.AppendDelimit<object?>(", ", array.AsArrayEnumerable(), (tb, value) => tb.AppendDump(value, options));
            }
            return textBuilder.Append(']');
        }
        
        public static TextBuilder AppendDump<T>(this TextBuilder textBuilder, params T[]? array)
        {
            return AppendDump<T>(textBuilder, array, default);
        }
        
        public static TextBuilder AppendDump<T>(this TextBuilder textBuilder, T[]? array, DumpOptions options)
        {
            return textBuilder.Append('[')
                              .AppendDelimit(", ", array, (tb, value) => tb.AppendDump((object?) value, options))
                              .Append(']');
        }
        
        public static TextBuilder AppendDump<T>(this TextBuilder textBuilder, IEnumerable<T> values, DumpOptions options = default)
        {
            return textBuilder.Append('(')
                              .AppendDelimit(", ", values, (tb, value) => tb.AppendDump((object?) value, options))
                              .Append(')');
        }
        
        public static TextBuilder AppendDump<T>(this TextBuilder textBuilder, ReadOnlySpan<T> values, DumpOptions options = default)
        {
            return textBuilder.Append('|')
                              .AppendDelimit<T>(", ", values, (tb, value) => tb.AppendDump((object?) value, options))
                              .Append('|');
        }
    }
}