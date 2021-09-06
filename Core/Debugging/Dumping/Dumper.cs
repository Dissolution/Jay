using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Reflection;
using Jay.Constraints;
using Jay.Reflection.Emission;
using Jay.Text;

namespace Jay.Debugging.Dumping
{
    public static partial class Dumper
    {
        private static string FormatImpl(string format, object?[] args)
        {
            using (var builder = TextBuilder.Rent())
            {
                builder.WriteFormat(null, format, args, true);
                return builder.ToString();
            }
        }
        
        internal static bool CheckNull<T>(TextBuilder builder,
                                          [AllowNull, NotNullWhen(true)] T value)
        {
            if (value is null)
            {
                builder.Append('(')
                       .AppendDump(typeof(T), MemberDumpOptions.Default)
                       .Append(")null");
                return false;
            }
            return true;
        }

        internal static string DumpObject(object? obj) => Dump(obj, MemberDumpOptions.Default);
        
        public static string Dump(object? obj, DumpOptions? options = default)
        {
            using (var builder = TextBuilder.Rent())
            {
                builder.AppendDump(obj, options);
                return builder.ToString();
            }
        }

        public static string DumpProperties<T>([AllowNull] T instance, bool onlyNonDefault = false)
        {
            if (instance is null) return string.Empty;
            using (var builder = TextBuilder.Rent())
            {
                builder.Append<T>(instance).AppendNewLine()
                       .Append('{').AppendNewLine();
                var properties = typeof(T).GetProperties(BindingFlags.Public | BindingFlags.Instance);
                var count = properties.Length;
                var types = new List<string>(count);
                var names = new List<string>(count);
                var values = new List<object?>(count);
                foreach (var property in properties)
                {
                    object? value = property.GetValue<T, object?>(ref instance!);
                    if (onlyNonDefault && value.IsDefault()) continue;
                    types.Add(Dumper.Dump(property.PropertyType));
                    names.Add(property.Name);
                    values.Add(value);
                }

                var typesWidth = types.Select(s => s.Length).Max();
                var namesWidth = names.Select(s => s.Length).Max();
                for (var i = 0; i < values.Count; i++)
                {
                    builder.Append('\t')
                           .AppendAlign(types[i], Alignment.Right, typesWidth)
                           .Append(' ')
                           .AppendAlign(names[i], Alignment.Right, namesWidth)
                           .Append(": ")
                           .AppendDump(values[0])
                           .AppendNewLine();
                }
                builder.Append('}');
                return builder.ToString();
            }
        }

        public static string Format(this FormattableString formattableString)
        {
            return FormatImpl(formattableString.Format, formattableString.GetArguments());
        }

        public static string Format(NonFormattableString format, params object?[] args)
        {
            return FormatImpl(format.Value, args);
        }
    }
}