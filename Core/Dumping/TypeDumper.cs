using System.Diagnostics;
using Jay.Text;

namespace Jay.Dumping;

public class TypeDumper : Dumper<Type>
{
    public override void DumpValue(TextBuilder text, Type? type, DumpOptions options = default)
    {
        switch (Type.GetTypeCode(type))
        {
            case TypeCode.Empty:
                DumpNull<Type>(text, null, options);
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
                        DumpValue(text, et, options);
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
                DumpValue(text, elementType, options);
            }
            else
            {
                Debug.Assert(type.IsPointer, "Only array, by-ref, and pointer types have an element type.");

                DumpValue(text, elementType, options);
                text.Write('*');
            }

            return;
        }

        // if (type.IsGenericType && !type.IsGenericTypeDefinition)
        // {
        //
        // }

        if (type.IsGenericType)
        {
            var argTypes = type.GenericTypeArguments;
            ReadOnlySpan<char> name = type.Name;
            // Remove the trailing `# bit
            var idx = name.LastIndexOf('`');
            if (idx == -1)
            {
                name = type.FullName;
                idx = name.LastIndexOf('`');
            }
            Debug.Assert(idx >= 0);
            text.Append(name[..idx])
                .Append('<')
                .AppendDelimit(",", argTypes, (tb, argType) => DumpValue(tb, argType))
                .Append('>');
            return;
        }

        Debugger.Break();

        text.Write(type.Name);
    }
}