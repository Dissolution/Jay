﻿using System.Reflection;
using Jay.Reflection;
using Jay.Reflection.Extensions;
using Jay.Text;

namespace Jay.Dumping;

public static partial class Dumper
{
    private static void DumpFieldTo(FieldInfo? field, TextBuilder text)
    {
        if (TryDumpNull(field, text)) return;
        DumpEnumTo(field.Visibility(), text);
        text.Write(' ');
        DumpTypeTo(field.FieldType, text);
        text.Write(' ');
        DumpTypeTo(field.OwnerType(), text);
        text.Append('.').Write(field.Name);
    }

    private static void DumpPropertyTo(PropertyInfo? property, TextBuilder text)
    {
        if (TryDumpNull(property, text)) return;
        var getVis = property.GetGetter().Visibility();
        var setVis = property.GetSetter().Visibility();
        Visibility highVis = getVis >= setVis ? getVis : setVis;
        DumpEnumTo(highVis, text);
        text.Write(' ');
        DumpTypeTo(property.PropertyType, text);
        text.Write(' ');
        DumpTypeTo(property.OwnerType(), text);
        text.Append('.').Append(property.Name).Write(" {");
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

    private static void DumpEventTo(EventInfo? @event, TextBuilder text)
    {
        if (TryDumpNull(@event, text)) return;
        DumpEnumTo(@event.Visibility(), text);
        text.Write(' ');
        DumpTypeTo(@event.EventHandlerType, text);
        text.Write(' ');
        DumpTypeTo(@event.OwnerType(), text);
        text.Append('.').Write(@event.Name);
    }

    private static void DumpConstructorTo(ConstructorInfo? ctor, TextBuilder text)
    {
        if (TryDumpNull(ctor, text)) return;
        DumpEnumTo(ctor.Visibility(), text);
        text.Write(' ');
        DumpTypeTo(ctor.DeclaringType, text);
        text.Append('(')
            .AppendDelimit(", ", ctor.GetParameters(), (tb, param) => DumpParameterTo(param, tb))
            .Write(')');
    }

    private static void DumpMethodTo(MethodInfo? method, TextBuilder text)
    {
        if (TryDumpNull(method, text)) return;
        DumpEnumTo(method.Visibility(), text);
        text.Write(' ');
        DumpTypeTo(method.OwnerType(), text);
        text.Append('.').Append(method.Name)
            .Append('(')
            .AppendDelimit(", ", method.GetParameters(), (tb, param) => DumpParameterTo(param, tb))
            .Write(')');
    }

    private static void DumpParameterTo(ParameterInfo? parameter, TextBuilder text)
    {
        if (TryDumpNull(parameter, text)) return;
        DumpTypeTo(parameter.ParameterType, text);
        text.Append(' ').Append(parameter.Name);
        if (parameter.HasDefaultValue)
        {
            text.Write(" = ");
            DumpObjectTo(parameter.DefaultValue, text);
        }
    }
}