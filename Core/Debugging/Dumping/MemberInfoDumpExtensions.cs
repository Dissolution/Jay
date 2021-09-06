using System;
using System.Diagnostics.CodeAnalysis;
using System.Reflection;
using Jay.Reflection;
using Jay.Text;

namespace Jay.Debugging.Dumping
{
    public static class MemberInfoDumpExtensions
    {
         private static bool CheckNull<TMember>(TextBuilder builder, [NotNullWhen(true)] TMember? member, MemberDumpOptions options)
         {
             if (member is null)
             {
                 if (options.InstanceType)
                 {
                     builder.Append('(')
                            .AppendDump(typeof(TMember), options)
                            .Append(')')
                            .Append("null");
                 }
                 return false;
             }
             return true;
         }

         private static void WriteAttributes<TMember>(TextBuilder builder, TMember member)
            where TMember : MemberInfo
         {
             var attributes = Attribute.GetCustomAttributes(member, true);
             if (attributes.Length > 0)
             {
                 builder.AppendDelimit(Environment.NewLine,
                                       attributes,
                                       (tb, attr) => tb.Append('[').Append(attr).Append(']'))
                        .AppendNewLine();
             }
         }
         
         private static void WritePreamble<TMember>(TextBuilder builder, TMember member)
             where TMember : MemberInfo
         {
             var visibility = member.GetVisibility();
             if (visibility.HasFlag<Visibility>(Visibility.Private))
                 builder.Write("private ");
             if (visibility.HasFlag<Visibility>(Visibility.Protected))
                 builder.Write("protected ");
             if (visibility.HasFlag<Visibility>(Visibility.Internal))
                 builder.Write("internal ");
             if (visibility.HasFlag<Visibility>(Visibility.Public))
                 builder.Write("public ");
             if (member.IsStatic())
             {
                 builder.Write("static ");
             }
         }
        
         public static TextBuilder AppendDump(this TextBuilder textBuilder, MemberInfo? member, MemberDumpOptions? options = default)
         {
             options ??= MemberDumpOptions.Default;
             if (member is null)
             {
                 if (options.InstanceType)
                     return textBuilder.Append("(MemberInfo)null");
                 return textBuilder;
             }
        
             if (member is FieldInfo fieldInfo)
             {
                 return AppendDump(textBuilder, fieldInfo, options);
             }
        
             if (member is PropertyInfo propertyInfo)
             {
                 return AppendDump(textBuilder, propertyInfo, options);
             }
        
             if (member is EventInfo eventInfo)
             {
                 return AppendDump(textBuilder, eventInfo, options);
             }
        
             if (member is ConstructorInfo constructorInfo)
             {
                 return AppendDump(textBuilder, constructorInfo, options);
             }
        
             if (member is MethodInfo methodInfo)
             {
                 return AppendDump(textBuilder, methodInfo, options);
             }
        
             // Fallback
             return textBuilder.Append(member);
         }
        
      
         
         public static TextBuilder AppendDump(this TextBuilder textBuilder, FieldInfo? field, MemberDumpOptions? options = default)
         {
             options ??= MemberDumpOptions.Default;
             if (!CheckNull(textBuilder, field, options))
                 return textBuilder;
             if (options.Attributes)
                 WriteAttributes(textBuilder, field);
             if (options.Preamble)
                WritePreamble(textBuilder, field);
             textBuilder.AppendDump(field.FieldType, options)
                        .Append(' ');
             if (options.InstanceType)
             {
                 textBuilder.AppendDump(field.InstanceType())
                            .Append('.');
             }
             return textBuilder.Append(field.Name);
         }
         
         public static TextBuilder AppendDump(this TextBuilder textBuilder, PropertyInfo? property, MemberDumpOptions? options = default)
         {
             options ??= MemberDumpOptions.Default;
             if (!CheckNull(textBuilder, property, options))
                 return textBuilder;
             if (options.Attributes)
                 WriteAttributes(textBuilder, property);
             if (options.Preamble)
                 WritePreamble(textBuilder, property);
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
                 if (options.Attributes)
                     WriteAttributes(textBuilder, getter);
                 if (options.Preamble)
                     WritePreamble(textBuilder, getter);
                 textBuilder.Append("get; ");
             }
             var setter = property.GetSetter();
             if (setter != null)
             {
                 if (options.Attributes)
                     WriteAttributes(textBuilder, setter);
                 if (options.Preamble)
                     WritePreamble(textBuilder, setter);
                 textBuilder.Append("set; ");
             }
             return textBuilder.Append(" }");
         }
        
         public static TextBuilder AppendDump(this TextBuilder textBuilder, EventInfo? eventInfo, MemberDumpOptions? options = default)
         {
             options ??= MemberDumpOptions.Default;
             throw new NotImplementedException();
         }
         
         public static TextBuilder AppendDump(this TextBuilder textBuilder, ConstructorInfo? constructor, MemberDumpOptions?  options = default)
         {
             options ??= MemberDumpOptions.Default;
             if (!CheckNull(textBuilder, constructor, options))
                 return textBuilder;
             if (options.Attributes)
                 WriteAttributes(textBuilder, constructor);
             if (options.Preamble)
                 WritePreamble(textBuilder, constructor);
             return textBuilder.Append(" new ")
                        .AppendDump(constructor.DeclaringType, options)
                        .Append('(')
                        .AppendDelimit(", ", constructor.GetParameters(), (tb, param) => tb.AppendDump(param, options))
                        .Append(')');
         }
         
         public static TextBuilder AppendDump(this TextBuilder textBuilder, MethodInfo? method, MemberDumpOptions?  options = default)
         {
             options ??= MemberDumpOptions.Default;
             if (!CheckNull(textBuilder, method, options))
                 return textBuilder;
             if (options.Attributes)
                 WriteAttributes(textBuilder, method);
             if (options.Preamble)
                 WritePreamble(textBuilder, method);
             textBuilder.AppendDump(method.ReturnType).Append(' ');
             if (options.InstanceType)
             {
                 textBuilder.AppendDump(method.InstanceType())
                            .Append('.');
             }
             return textBuilder.Append(method.Name)
                               .Append('(')
                               .AppendDelimit(", ", method.GetParameters(), (tb, param) => tb.AppendDump(param, options))
                               .Append(')');
         }
         
         public static TextBuilder AppendDump(this TextBuilder textBuilder, ParameterInfo? parameter, MemberDumpOptions?  options = default)
         {
             options ??= MemberDumpOptions.Default;
             if (!CheckNull(textBuilder, parameter, options))
                 return textBuilder;
             if (parameter.ParameterType.IsByRef)
             {
                 if (parameter.IsIn)
                 {
                     textBuilder.Append("in ");
                 }
                 else if (parameter.IsOut)
                 {
                     textBuilder.Append("out ");
                 }
                 else
                 {
                     textBuilder.Append("ref ");
                 }
             }
             return textBuilder.AppendDump(parameter.ParameterType, options)
                               .Append(' ')
                               .Append(parameter.Name);
         }
    }
}