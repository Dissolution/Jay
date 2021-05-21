using Jay.Reflection;
using Jay.Text;
using System;
using System.Reflection;

namespace Jay.Debugging.Dumping
{
    public static partial class Dumper
    {
        public static TextBuilder AppendDump(this TextBuilder textBuilder, MemberInfo? member, DumpOptions options = default)
        {
            if (member is null)
            {
                if (options.Verbose)
                    return textBuilder.Append("(MemberInfo)null");
                return textBuilder;
            }

            if (member is FieldInfo fieldInfo)
            {
                return AppendDump(textBuilder, fieldInfo, options);
            }

            // Fallback
            return textBuilder.Append(member);
        }
        
        public static TextBuilder AppendDump(this TextBuilder textBuilder, FieldInfo? field, DumpOptions options = default)
        {
            if (field is null)
            {
                if (options.Verbose)
                    return textBuilder.Append("(FieldInfo)null");
                return textBuilder;
            }

            if (options.Verbose)
            {
                var fieldAttributes = Attribute.GetCustomAttributes(field, true);
                if (fieldAttributes.Length > 0)
                {
                    textBuilder.AppendDelimit(Environment.NewLine,
                                              fieldAttributes,
                                              (tb, attr) => tb.Append('[').Append(attr).Append(']'))
                               .AppendLine();
                }

                var visibility = field.GetVisibility();
                if (visibility.HasFlag<Visibility>(Visibility.Private))
                    textBuilder.Write("private ");
                if (visibility.HasFlag<Visibility>(Visibility.Protected))
                    textBuilder.Write("protected ");
                if (visibility.HasFlag<Visibility>(Visibility.Internal))
                    textBuilder.Write("internal ");
                if (visibility.HasFlag<Visibility>(Visibility.Public))
                    textBuilder.Write("public ");
                if (field.IsStatic)
                {
                    textBuilder.Write("static ");
                }
            }
            
            return textBuilder.AppendDump(field.FieldType, options)
                              .Append(' ')
                              .Append(field.Name);
        }
        
        public static TextBuilder AppendDump(this TextBuilder textBuilder, PropertyInfo? property, DumpOptions options = default)
        {
            if (property is null)
            {
                if (options.Verbose)
                    return textBuilder.Append("(PropertyInfo)null");
                return textBuilder;
            }

            if (options.Verbose)
            {
                var fieldAttributes = Attribute.GetCustomAttributes(property, true);
                if (fieldAttributes.Length > 0)
                {
                    textBuilder.AppendDelimit(Environment.NewLine,
                                              fieldAttributes,
                                              (tb, attr) => tb.Append('[').Append(attr).Append(']'))
                               .AppendLine();
                }

                var visibility = property.GetVisibility();
                if (visibility.HasFlag<Visibility>(Visibility.Private))
                    textBuilder.Write("private ");
                if (visibility.HasFlag<Visibility>(Visibility.Protected))
                    textBuilder.Write("protected ");
                if (visibility.HasFlag<Visibility>(Visibility.Internal))
                    textBuilder.Write("internal ");
                if (visibility.HasFlag<Visibility>(Visibility.Public))
                    textBuilder.Write("public ");
                if (property.IsStatic())
                {
                    textBuilder.Write("static ");
                }
            }

            textBuilder.AppendDump(property.PropertyType, options)
                       .Append(' ')
                       .Append(property.Name);

            var indexParams = property.GetIndexParameters();
            if (indexParams.Length > 0)
            {
                textBuilder.Append('[')
                           .AppendDelimit(", ", indexParams, (tb, pi) => tb.AppendDump(pi, options))
                           .Append(']');
            }

            // Getter + Setter
            textBuilder.Append(" { ");
            var getter = property.GetGetter();
            if (getter != null)
            {
                if (options.Verbose)
                {
                    
                }
                else
                {
                    textBuilder.Append("get; ");
                }
            }
            var setter = property.GetSetter();
            if (setter != null)
            {
                if (options.Verbose)
                {
                    
                }
                else
                {
                    textBuilder.Append("set; ");
                }
            }
            return textBuilder.Append(" }");
        }
        
        // TODO: ParameterInfo
    }
}