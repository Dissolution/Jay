using System.Diagnostics;
using System.Reflection;
using Jay.Reflection;
using Jay.Text;

namespace Jay.Dumping;

public class MemberDumper : Dumper<MemberInfo>
{
    private static void Dump(TextBuilder text, Type type, DumpLevel level = DumpLevel.Self)
    {
        switch (Type.GetTypeCode(type))
        {
            case TypeCode.Empty:
                DumpNull<Type>(text, null, level);
                return;
            case TypeCode.DBNull:
                text.Write("DBNull");
                return;
            case TypeCode.Boolean:
                text.Write("bool");
                return;
            case TypeCode.Char:
                text.Write("char");
                return;
            case TypeCode.SByte:
                text.Write("sbyte");
                return;
            case TypeCode.Byte:
                text.Write("byte");
                return;
            case TypeCode.Int16:
                text.Write("short");
                return;
            case TypeCode.UInt16:
                text.Write("ushort");
                return;
            case TypeCode.Int32:
                text.Write("int");
                return;
            case TypeCode.UInt32:
                text.Write("uint");
                return;
            case TypeCode.Int64:
                text.Write("long");
                return;
            case TypeCode.UInt64:
                text.Write("ulong");
                return;
            case TypeCode.Single:
                text.Write("float");
                return;
            case TypeCode.Double:
                text.Write("double");
                return;
            case TypeCode.Decimal:
                text.Write("decimal");
                return;
            case TypeCode.DateTime:
                text.Write("DateTime");
                return;
            case TypeCode.String:
                text.Write("string");
                return;
            case TypeCode.Object:
            //text.Write("object");
            //return;
            default:
                break;
        }

        ReadOnlySpan<char> name = type!.Name;
        if (type.IsGenericType)
        {
            var argTypes = type.GenericTypeArguments;
            // Remove the trailing `# bit
            var idx = name.LastIndexOf('`');
            Debug.Assert(idx >= 0);
            text.Append(name.Slice(0, idx)).Append('<')
                .AppendDelimit(",", argTypes, (tb, argType) => Dump(tb, argType, DumpLevel.Self))
                .Append('>');
            return;
        }

        Debugger.Break();

        text.Write(name);
    }

    private static void Dump(TextBuilder text, FieldInfo field, DumpLevel level)
    {
        if (level.HasFlag(DumpLevel.Details))
        {
            text.Append(field.Visibility()).Append(' ');
        }

        text.AppendDump(field.FieldType, DumpLevel.Self)
            .Write(' ');

        if (level.HasFlag(DumpLevel.Surroundings))
        {
            text.AppendDump(field.OwnerType(), DumpLevel.Self)
                .Write('.');
        }
        text.Write(field.Name);
    }

    private static void Dump(TextBuilder text, PropertyInfo property, DumpLevel level)
    {
        if (level.HasFlag(DumpLevel.Details))
        {
            var getVis = property.GetGetter().Visibility();
            var setVis = property.GetSetter().Visibility();
            Visibility highVis = getVis >= setVis ? getVis : setVis;
            text.Append(highVis)
                .Append(' ')
                .AppendDump(property.PropertyType, DumpLevel.Self)
                .Write(' ');

            if (level.HasFlag(DumpLevel.Surroundings))
            {
                text.AppendDump(property.OwnerType(), DumpLevel.Self)
                    .Write('.');
            }

            text.Append(property.Name)
                .Write(" { ");
            if (getVis != Visibility.None)
            {
                text.Append(getVis)
                    .Write(" get; ");
            }

            if (setVis != Visibility.None)
            {
                text.Append(setVis)
                    .Write(" set; ");
            }
            text.Write('}');
        }
        else
        {
            if (level.HasFlag(DumpLevel.Surroundings))
            {
                text.AppendDump(property.OwnerType(), DumpLevel.Self)
                    .Write('.');
            }

            text.Write(property.Name);
        }
    }

    private static void Dump(TextBuilder text, EventInfo eventInfo, DumpLevel level)
    {
        if (level.HasFlag(DumpLevel.Details))
        {
            text.Append(eventInfo.Visibility()).Append(' ');
        }

        text.Append("event ")
            .AppendDump(eventInfo.EventHandlerType, DumpLevel.Self)
            .Write(' ');

        if (level.HasFlag(DumpLevel.Surroundings))
        {
            text.AppendDump(eventInfo.OwnerType(), DumpLevel.Self)
                .Write('.');
        }

        text.Write(eventInfo.Name);
    }

    private static void Dump(TextBuilder text, ConstructorInfo constructor, DumpLevel level)
    {
        if (level.HasFlag(DumpLevel.Details))
        {
            text.Append(constructor.Visibility()).Append(' ');
        }
        text.AppendDump(constructor.DeclaringType!, DumpLevel.Self)
            .Append('(')
            .AppendDelimit(",", constructor.GetParameters(), (tb, param) => tb.AppendDump(param, level))
            .Append(')');
    }

    private static void Dump(TextBuilder text, MethodInfo method, DumpLevel level)
    {
        if (level.HasFlag(DumpLevel.Details))
        {
            text.Append(method.Visibility()).Append(' ');
        }


        if (level.HasFlag(DumpLevel.Surroundings))
        {
            text.AppendDump(method.OwnerType(), DumpLevel.Self)
                .Write('.');
        }

        text.Append(method.Name)
            .Append('(')
            .AppendDelimit(",", method.GetParameters(), (tb, param) => tb.AppendDump(param, level))
            .Append(')');
    }

    public override void Dump(TextBuilder text, MemberInfo? value, DumpLevel level = DumpLevel.Self)
    {
        if (DumpNull(text, value, level)) return;
        switch (value)
        {
            case Type type:
                Dump(text, type, level);
                return;
            case FieldInfo field:
                Dump(text, field, level);
                return;
            case PropertyInfo property:
                Dump(text, property, level);
                return;
            case EventInfo eventInfo:
                Dump(text, eventInfo, level);
                return;
            case ConstructorInfo ctor:
                Dump(text, ctor, level);
                return;
            case MethodInfo method:
                Dump(text, method, level);
                return;
            default:
                Dumpers.Default.Dump(text, value, level);
                return;
        }
    }
}