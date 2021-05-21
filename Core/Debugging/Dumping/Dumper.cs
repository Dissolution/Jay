using Jay.Collections;
using Jay.Text;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;

namespace Jay.Debugging.Dumping
{
    public static partial class Dumper
    {
        public static TextBuilder AppendDump(this TextBuilder textBuilder, object? obj, DumpOptions options = default)
        {
            // Big ol' switch statement
            if (obj is null)
            {
                if (options.Verbose)
                    return textBuilder.Append("null");
                return textBuilder;
            }
            if (obj is Type type)
            {
                return AppendDump(textBuilder, type, options);
            }
            if (obj is Array array)
            {
                return AppendDump(textBuilder, array, options);
            }
            if (obj is IEnumerable enumerable)
            {
                return AppendDump(textBuilder, enumerable, options);
            }
            if (obj is string str)
            {
                return textBuilder.Append(str);
            }
            if (obj is TimeSpan timeSpan)
            {
                return textBuilder.AppendDump(timeSpan, options);
            }
            if (obj is DateTime dateTime)
            {
                return textBuilder.AppendDump(dateTime, options);
            }
            if (obj is Guid guid)
            {
                return textBuilder.AppendDump(guid, options);
            }

            // Default
            return textBuilder.Append(obj);
        }
    }


   


}