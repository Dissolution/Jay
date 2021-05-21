using Jay.Text;
using System;
using System.Diagnostics;

namespace Jay.Debugging.Dumping
{
     public static partial class Dumper
    {
        public static TextBuilder AppendDump(this TextBuilder builder, Type? type, DumpOptions options = default)
        {
            if (type is null)
            {
                if (options.Verbose)
                {
                    return builder.Append("(Type)null");
                }
                return builder;
            }

            var underlyingType = Nullable.GetUnderlyingType(type);
            if (underlyingType != null)
            {
                return AppendDump(builder, underlyingType, options).Append('?');
            }

            if (type == typeof(bool))
                return builder.Append("bool");
            if (type == typeof(byte))
                return builder.Append("byte");
            if (type == typeof(sbyte))
                return builder.Append("sbyte");
            if (type == typeof(short))
                return builder.Append("short");
            if (type == typeof(ushort))
                return builder.Append("ushort");
            if (type == typeof(int))
                return builder.Append("int");
            if (type == typeof(uint))
                return builder.Append("uint");
            if (type == typeof(long))
                return builder.Append("long");
            if (type == typeof(ulong))
                return builder.Append("ulong");
            if (type == typeof(float))
                return builder.Append("float");
            if (type == typeof(double))
                return builder.Append("double");
            if (type == typeof(decimal))
                return builder.Append("decimal");
            if (type == typeof(char))
                return builder.Append("char");
            if (type == typeof(string))
                return builder.Append("string");
            if (type == typeof(object))
                return builder.Append("object");
            if (type == typeof(void))
                return builder.Append("void");

            if (type.IsArray)
            {
                underlyingType = type.GetElementType();
                Debug.Assert(underlyingType != null);
                return AppendDump(builder, underlyingType, options).Append("[]");
            }
            
            Hold.Debug(type,
                       type.IsGenericType,
                       type.IsGenericTypeDefinition,
                       type.IsConstructedGenericType);
            return builder.Append(type.Name);
        }
    }
}