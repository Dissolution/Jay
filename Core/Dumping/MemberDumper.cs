using System.Diagnostics;
using System.Reflection;
using System.Text;
using Jay.Reflection;
using Jay.Text;

namespace Jay.Dumping;

public class TypeDumper : IDumper<Type>
{
    public void DumpValue(TextBuilder text, Type? type, DumpLevel level = DumpLevel.Default)
    {
        switch (Type.GetTypeCode(type))
        {
            case TypeCode.Empty:
                Dump.DumpNull<Type>(text, null, level);
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
    https://github.com/stakx/TypeNameFormatter/blob/2ab6b20e7eb2a2477371769d8109c705a0043e2b/src/TypeNameFormatter/TypeNameFormatter.Nullable.cs#L67
        if (type == typeof(object))
        {
            text.Write("object");
            return;
        }

        if (type == typeof(void))
        {
            text.Write("void");
            return;
        }

        // Array, ref, pointer
        if (type!.HasElementType)
        {
            var elementType = type.GetElementType()!;

            if (type.IsArray)
            {
                var ranks = new Queue<int>();
                ranks.Enqueue(type.GetArrayRank());
                HandleArrayElementType(elementType, ranks);

                void HandleArrayElementType(Type et, Queue<int> r)
                {
                    if (et.IsArray)
                    {
                        r.Enqueue(et.GetArrayRank());
                        HandleArrayElementType(et.GetElementType()!, r);
                    }
                    else
                    {
                        DumpValue(text, et, level);
                        while (r.Count > 0)
                        {
                            text.Append('[')
                                .AppendRepeat(r.Dequeue() - 1, ',')
                                .Write(']');
                        }
                    }
                }
            }
            else if (type.IsByRef)
            {
                text.Write("ref ");
                DumpValue(text, elementType, level);
            }
            else
            {
                Debug.Assert(type.IsPointer, "Only array, by-ref, and pointer types have an element type.");

                DumpValue(text, elementType, level);
                text.Write('*');
            }

            return;
        }

        // if (type.IsGenericType && !type.IsGenericTypeDefinition)
        // {
        //
        // }


        ReadOnlySpan<char> name = type!.Name;
        if (type.IsGenericType)
        {
            var argTypes = type.GenericTypeArguments;
            // Remove the trailing `# bit
            var idx = name.LastIndexOf('`');
            Debug.Assert(idx >= 0);
            text.Append(name[..idx])
                .Append('<')
                .AppendDelimit(",", argTypes, (tb, argType) => DumpValue(tb, argType))
                .Append('>');
            return;
        }

        Debugger.Break();

        text.Write(name);
    }
}

public class MemberDumper : IDumper<MemberInfo>
{
    public bool CanDump(Type objType)
    {
        return objType.Implements<MemberInfo>() && objType != typeof(Type);
    }

    private static void DumpField(TextBuilder text, FieldInfo field, DumpLevel level)
    {
        if (level.HasFlag(DumpLevel.Detailed))
        {
            text.Append(field.Visibility()).Append(' ');
        }

        text.AppendDump(field.FieldType)
            .Write(' ');

        if (level.HasFlag(DumpLevel.Detailed))
        {
            text.AppendDump(field.OwnerType())
                .Write('.');
        }
        text.Write(field.Name);
    }

    private static void DumpProperty(TextBuilder text, PropertyInfo property, DumpLevel level)
    {
        if (level.HasFlag(DumpLevel.Detailed))
        {
            var getVis = property.GetGetter().Visibility();
            var setVis = property.GetSetter().Visibility();
            Visibility highVis = getVis >= setVis ? getVis : setVis;
            text.Append(highVis)
                .Append(' ')
                .AppendDump(property.PropertyType)
                .Write(' ');

            if (level.HasFlag(DumpLevel.Detailed))
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
            if (level.HasFlag(DumpLevel.Detailed))
            {
                text.AppendDump(property.OwnerType())
                    .Write('.');
            }

            text.Write(property.Name);
        }
    }

    private static void DumpEvent(TextBuilder text, EventInfo eventInfo, DumpLevel level)
    {
        if (level.HasFlag(DumpLevel.Detailed))
        {
            text.Append(eventInfo.Visibility()).Append(' ');
        }

        text.Append("event ")
            .AppendDump(eventInfo.EventHandlerType)
            .Write(' ');

        if (level.HasFlag(DumpLevel.Detailed))
        {
            text.AppendDump(eventInfo.OwnerType())
                .Write('.');
        }

        text.Write(eventInfo.Name);
    }

    private static void DumpConstructor(TextBuilder text, ConstructorInfo constructor, DumpLevel level)
    {
        if (level.HasFlag(DumpLevel.Detailed))
        {
            text.Append(constructor.Visibility()).Append(' ');
        }
        text.AppendDump(constructor.DeclaringType!)
            .Append('(')
            .AppendDelimit(",", constructor.GetParameters(), (tb, param) => tb.AppendDump(param, level))
            .Append(')');
    }

    private static void DumpMethod(TextBuilder text, MethodInfo method, DumpLevel level)
    {
        if (level.HasFlag(DumpLevel.Detailed))
        {
            text.Append(method.Visibility()).Append(' ');
        }


        if (level.HasFlag(DumpLevel.Detailed))
        {
            text.AppendDump(method.OwnerType())
                .Write('.');
        }

        text.Append(method.Name)
            .Append('(')
            .AppendDelimit(",", method.GetParameters(), (tb, param) => tb.AppendDump(param, level))
            .Append(')');
    }

    public void DumpValue(TextBuilder text, MemberInfo? value, DumpLevel level = DumpLevel.Default)
    {
        if (Dump.DumpNull(text, value, level)) return;
        switch (value)
        {
            case Type type:
                Dump.GetDumper<Type>().DumpValue(text, type, level);
                return;
            case FieldInfo field:
                DumpField(text, field, level);
                return;
            case PropertyInfo property:
                DumpProperty(text, property, level);
                return;
            case EventInfo eventInfo:
                DumpEvent(text, eventInfo, level);
                return;
            case ConstructorInfo ctor:
                DumpConstructor(text, ctor, level);
                return;
            case MethodInfo method:
                DumpMethod(text, method, level);
                return;
            default:
                throw new NotImplementedException();
        }
    }
}