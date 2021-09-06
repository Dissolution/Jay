using System;
using System.Diagnostics;
using Jay.Debugging.Dumping.Temp;
using Jay.Reflection;
using Jay.Text;

namespace Jay.Debugging.Dumping.Temp
{
    public class TypeDumper : IDumper<Type>
    {
        public TextBuilder Dump(TextBuilder builder,
                                       Type type,
                                       DumpOptions options = DumpOptions.Surface)
        {
            // Nullable?
            var underlyingType = Nullable.GetUnderlyingType(type);
            if (underlyingType != null)
            {
                return builder.AppendDump(underlyingType, options)
                              .Append('?');
            }
            // Pointer?
            if (type.IsPointer)
            {
                return builder.AppendDump(type.GetElementType(), options)
                              .Append('*');
            }
            // ByRef?
            if (type.IsByRef)// || type.IsByRefLike)
            {
                return builder.Append("ref ")
                              .AppendDump(type.GetElementType(), options);
            }
            
            // Common system types
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
            
            // Array
            if (type.IsArray)
            {
                underlyingType = type.GetElementType();
                Debug.Assert(underlyingType != null);
                return builder.AppendDump(underlyingType, options)
                              .Append("[]");
            }

            if (options.HasFlag<DumpOptions>(DumpOptions.Type))
            {
                var instanceType = type.InstanceType();
                if (instanceType != typeof(void))
                {
                    builder.AppendDump(instanceType, options)
                           .Append('.');
                }
            }
            
            // Generic Types?
            if (type.IsGenericType)
            {
                // Cut off the `##### part of the name
                var index = type.Name.IndexOf('`');
                Debug.Assert(index >= 0);
                return builder.Append(type.Name.Slice(..index))
                       .Append('<')
                       .AppendDelimit(", ", type.GetGenericArguments(), (tb, gaType) => tb.AppendDump(gaType, options))
                       .Append('>');
            }
            
            // Default is just the name
            return builder.Append(type.Name);
        }
    }
}