using Jay.Collections;
using Jay.Text;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Reflection;
using System.Runtime.CompilerServices;

namespace Jay.Debugging.Dumping
{
    public static partial class Dumper
    {
        public static TextBuilder AppendDump(this TextBuilder textBuilder, object? obj, DumpOptions options = default)
        {
            return obj switch
            {
                // Big ol' switch statement
                null when options.Verbose => textBuilder.Append("null"),
                null => textBuilder,
                Type type => AppendDump(textBuilder, type, options),
                Array array => AppendDump(textBuilder, array, options),
                string str => textBuilder.Append(str),
                IEnumerable enumerable => AppendDump(textBuilder, enumerable, options),
                TimeSpan timeSpan => textBuilder.AppendDump(timeSpan, options),
                DateTime dateTime => textBuilder.AppendDump(dateTime, options),
                Guid guid => textBuilder.AppendDump(guid, options),
                _ => textBuilder.Append(obj)
            };

            // Default
        }
    }

    internal static class Dumper<T>
    {
        private delegate void Dump(TextBuilder textBuilder, T? value, DumpOptions options);

        private static readonly Dump _dump;
        
        static Dumper()
        {
            var type = typeof(T);
            var attr = type.GetCustomAttribute<DumpAsAttribute>();
            if (attr != null && attr.HasRepresentation(out string rep))
            {
                _dump = (builder, _, _) => builder.Append(rep);
            }
            else
            {
                _dump = (builder, value, options) => builder.Append<T>(value);
            }
        }


        public static TextBuilder AppendDump(TextBuilder textBuilder,
                                             T? value,
                                             DumpOptions options = default)
        {
            _dump(textBuilder, value, options);
            return textBuilder;
        }
    }


   


}