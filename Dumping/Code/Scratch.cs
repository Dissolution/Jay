
using System.Diagnostics;
using System.Reflection;
using System.Reflection.Metadata;

using Jay.Dumping;
using Jay.Dumping.Interpolated;
using Jay.Text;

namespace Jay.ToCode;


internal static class TypeFormatter
{
    // Simple type aliases
    private static readonly (Type Type, string Code)[] _systemTypeCodes =
    {
        (typeof(bool), "bool"),
        (typeof(char), "char"),
        (typeof(sbyte), "sbyte"),
        (typeof(byte), "byte"),
        (typeof(short), "short"),
        (typeof(ushort), "ushort"),
        (typeof(int), "int"),
        (typeof(uint), "uint"),
        (typeof(long), "long"),
        (typeof(ulong), "ulong"),
        (typeof(float), "float"),
        (typeof(double), "double"),
        (typeof(decimal), "decimal"),
        (typeof(string), "string"),
        (typeof(object), "object"),
        (typeof(void), "void"),
    };

    public static TextBuilder AppendTypeCode(this TextBuilder textBuilder, Type type)
    {
        Type? underType;

        // Enum is always just Name
        if (type.IsEnum)
        {
            return textBuilder.Append(type.Name);
        }

        // Nullable<T>:  "{type}?"
        underType = Nullable.GetUnderlyingType(type);
        if (underType is not null)
        {
            AppendTypeCode(textBuilder, underType);
            return textBuilder.Append('?');
        }

        // Pointer:  "{type}*"
        if (type.IsPointer)
        {
            underType = type.GetElementType();
            Debug.Assert(underType != null);
            AppendTypeCode(textBuilder, underType);
            return textBuilder.Append('*');
        }

        // ByRef:  "ref {type}"
        if (type.IsByRef)
        {
            underType = type.GetElementType();
            Debug.Assert(underType != null);
            textBuilder.Write("ref ");
            return AppendTypeCode(textBuilder, underType);
        }

        // Array:  "{type}[]"
        if (type.IsArray)
        {
            underType = type.GetElementType();
            Debug.Assert(underType != null);
            AppendTypeCode(textBuilder, underType);
            return textBuilder.Append("[]");
        }

        // un-detailed fast cache check
        foreach (var pair in _systemTypeCodes)
        {
            if (pair.Type == type)
            {
                return textBuilder.Append(pair.Code);
            }
        }

        // Nested:  "{declarer}.{type}"
        if (type.IsNested && !type.IsGenericParameter)
        {
            AppendTypeCode(textBuilder, type.DeclaringType!);
            textBuilder.Write('.');
        }

        // If non-generic
        if (!type.IsGenericType)
        {
            // Just write the type name and we're done
            return textBuilder.Append(type.Name);
        }

        // Start processing type name
        ReadOnlySpan<char> typeName = type.Name;

        // I'm a parameter?
        if (type.IsGenericParameter)
        {
            var constraints = type.GetGenericParameterConstraints();
            if (constraints.Length > 0)
            {
                textBuilder.Write(" : ");
                Debugger.Break();
            }

            Debugger.Break();
        }

        // Name is often something like NAME`2 for NAME<,>, so we want to strip that off
        var i = typeName.IndexOf('`');
        textBuilder.Write(i >= 0 ? typeName[..i] : typeName);

        // Add our generic types
        textBuilder.Write('<');
        var argTypes = type.GetGenericArguments();
        for (i = 0; i < argTypes.Length; i++)
        {
            if (i > 0) textBuilder.Write(", ");
            AppendTypeCode(textBuilder, argTypes[i]);
        }
        textBuilder.Write('>');

        return textBuilder;
    }
}


public static class Formatter
{
    private static TextBuilder AppendMemberCode(this TextBuilder textBuilder, MemberInfo member, bool declaration)
    {
        if (member is Type type)
        {
            return textBuilder.AppendTypeCode(type);
        }
        else if (member is FieldInfo field)
        {

        }
        else if (member is PropertyInfo property)
        {
            if (!declaration)
            {
                return textBuilder
                    .AppendTypeCode(property.PropertyType)
                    .Append(' ')
                    .Append(property.Name);
            }

            var getVis = property.GetGetter().Visibility();
            var setVis = property.GetSetter().Visibility();
            Visibility highVis = getVis >= setVis ? getVis : setVis;
            stringHandler.Write(highVis);
            stringHandler.Write(' ');

            stringHandler.Dump(property.PropertyType, format);
            stringHandler.Write(' ');
            stringHandler.Dump(property.OwnerType(), format);
            stringHandler.Write('.');
            stringHandler.Write(property.Name);
            stringHandler.Write(" {");
            if (getVis != Visibility.None)
            {
                if (getVis != highVis)
                    stringHandler.Write(getVis);
                stringHandler.Write(" get; ");
            }

            if (setVis != Visibility.None)
            {
                if (setVis != highVis)
                    stringHandler.Write(setVis);
                stringHandler.Write(" set; ");
            }

            stringHandler.Write('}');
        }
        else if (member is EventInfo eventInfo)
        {
            TypeFormatter.AppendTypeCode(textBuilder, eventInfo.EventHandlerType);
            textBuilder.Write(' ');
           
            if (declaration)
            {
                TypeFormatter.AppendTypeCode(textBuilder, eventInfo.ReflectedType ?? eventInfo.DeclaringType);
                textBuilder.Write('.');
            }
            textBuilder.Write(eventInfo.Name);
        }
        else if (member is ConstructorInfo constructor)
        {
            
        }
        else if (member is MethodInfo method)
        {

        }
        else if (m)
    }
    
    
    
    // declaration, as apposed to reference
    public static TextBuilder AppendCode<T>(this TextBuilder textBuilder, T? value, bool declaration = false)
    {
        if (value is null)
        {
            if (declaration)
            {
                textBuilder.Append('(').AppendCode(typeof(T)).Write(')');
            }
            return textBuilder.Append("null");
        }

        switch (value)
        {
            case bool boolean:
                return textBuilder.Append(boolean ? "true" : "false");
            case byte or sbyte or short or ushort or int or uint:
            {
                if (declaration)
                {
                    textBuilder.Append('(').AppendCode(value.GetType()).Write(')');
                }
                return textBuilder.Append<T>(value);
            }
            case long l:
                return textBuilder.Append<long>(l).Append('L');
            case ulong ul:
                return textBuilder.Append<ulong>(ul).Append("UL");
            case float f:
                return textBuilder.Append<float>(f).Append('f');
            case double d:
                return textBuilder.Append<double>(d).Append('d');
            case decimal m:
                return textBuilder.Append<decimal>(m).Append('m');
            case char ch:
                return textBuilder.Append('\'').Append(ch).Append('\'');
            case string str:
                return textBuilder.Append('"').Append(str).Append('"');
            case Guid guid:
                return textBuilder.Append('"').AppendFormat(guid, "D").Append('"');
            case TimeSpan timeSpan:
            {
                if (declaration)
                {
                    return textBuilder.Append("new TimeSpan(")
                        .Append(timeSpan.Days).Append(',')
                        .Append(timeSpan.Hours).Append(',')
                        .Append(timeSpan.Minutes).Append(',')
                        .Append(timeSpan.Seconds).Append(',')
                        .Append(timeSpan.Milliseconds).Append(",")
                        .Append(timeSpan.Microseconds).Append(")");
                }
                else
                {
                    return textBuilder.AppendFormat(timeSpan, "c");
                }
            }
            case DateTime dateTime:
            {
                if (declaration)
                {
                    return textBuilder.Append("new DateTime(")
                        .Append(dateTime.Year).Append(',')
                        .Append(dateTime.Month).Append(',')
                        .Append(dateTime.Day).Append(',')
                        .Append(dateTime.Hour).Append(',')
                        .Append(dateTime.Minute).Append(',')
                        .Append(dateTime.Second).Append(',')
                        .Append(dateTime.Millisecond).Append(',')
                        .Append(dateTime.Microsecond).Append(')');
                }
                else
                {
                    return textBuilder.AppendFormat(dateTime, "yyyyMMdd HH\\:mm\\:ss\\.ffffff");
                }
            }
            case MemberInfo member:
            {
                AppendMemberCode(textBuilder, member, declaration);
                return textBuilder;
            }
            default:
                break;
        }
    }
}

public static class Extensions
{
    public static string ToCode<T>(this T? value, FormatOptions options = default)
    {
        using var textBuilder = new TextBuilder();
        Formatter.WriteCode(textBuilder, value, options);
        return textBuilder.ToString();
    }
}