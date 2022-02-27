using System.Reflection;
using Jay.Reflection;
using Jay.Text;

namespace Jay.Dumping;

public class MemberDumper : Dumper<MemberInfo>
{
    public override bool CanDump(Type objType)
    {
        return objType.Implements<MemberInfo>() && objType != typeof(Type);
    }

    private static void DumpField(TextBuilder text, FieldInfo field, DumpOptions options)
    {
        if (options.Detailed)
        {
            text.Append(field.Visibility()).Append(' ');
        }

        text.AppendDump(field.FieldType)
            .Write(' ');

        if (options.Detailed)
        {
            text.AppendDump(field.OwnerType())
                .Write('.');
        }
        text.Write(field.Name);
    }

    private static void DumpProperty(TextBuilder text, PropertyInfo property, DumpOptions options)
    {
        if (options.Detailed)
        {
            var getVis = property.GetGetter().Visibility();
            var setVis = property.GetSetter().Visibility();
            Visibility highVis = getVis >= setVis ? getVis : setVis;
            text.Append(highVis)
                .Append(' ')
                .AppendDump(property.PropertyType)
                .Write(' ');

            if (options.Detailed)
            {
                text.AppendDump(property.OwnerType())
                    .Write('.');
            }

            text.Append(property.Name)
                .Write(" { ");
            if (getVis != Visibility.None)
            {
                if (getVis != highVis)
                    text.Write(getVis);
                text.Write(" get; ");
            }

            if (setVis != Visibility.None)
            {
                if (setVis != highVis)
                    text.Append(setVis);
                text.Write(" set; ");
            }
            text.Write('}');
        }
        else
        {
            if (options.Detailed)
            {
                text.AppendDump(property.OwnerType())
                    .Write('.');
            }

            text.Write(property.Name);
        }
    }

    private static void DumpEvent(TextBuilder text, EventInfo eventInfo, DumpOptions options)
    {
        if (options.Detailed)
        {
            text.Append(eventInfo.Visibility()).Append(' ');
        }

        text.Append("event ")
            .AppendDump(eventInfo.EventHandlerType)
            .Write(' ');

        if (options.Detailed)
        {
            text.AppendDump(eventInfo.OwnerType())
                .Write('.');
        }

        text.Write(eventInfo.Name);
    }

    private static void DumpConstructor(TextBuilder text, ConstructorInfo constructor, DumpOptions options)
    {
        if (options.Detailed)
        {
            text.Append(constructor.Visibility()).Append(' ');
        }
        text.AppendDump(constructor.DeclaringType!)
            .Append('(')
            .AppendDelimit(",", constructor.GetParameters(), (tb, param) => tb.AppendDump(param, options))
            .Append(')');
    }

    private static void DumpMethod(TextBuilder text, MethodInfo method, DumpOptions options)
    {
        if (options.Detailed)
        {
            text.Append(method.Visibility()).Append(' ');
        }


        if (options.Detailed)
        {
            text.AppendDump(method.OwnerType())
                .Write('.');
        }

        text.Append(method.Name)
            .Append('(')
            .AppendDelimit(",", method.GetParameters(), (tb, param) => tb.AppendDump(param, options))
            .Append(')');
    }

    public override void DumpValue(TextBuilder text, MemberInfo? value, DumpOptions options = default)
    {
        if (DumpNull(text, value, options)) return;
        switch (value)
        {
            case Type type:
                Dump.GetDumper<Type>().DumpValue(text, type, options);
                return;
            case FieldInfo field:
                DumpField(text, field, options);
                return;
            case PropertyInfo property:
                DumpProperty(text, property, options);
                return;
            case EventInfo eventInfo:
                DumpEvent(text, eventInfo, options);
                return;
            case ConstructorInfo ctor:
                DumpConstructor(text, ctor, options);
                return;
            case MethodInfo method:
                DumpMethod(text, method, options);
                return;
            default:
                throw new NotImplementedException();
        }
    }
}