using System;
using System.Diagnostics.CodeAnalysis;
using Jay.Constraints;
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