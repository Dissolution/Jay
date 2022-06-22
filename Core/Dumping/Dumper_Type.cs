using System.Diagnostics;
using Jay.Collections;
using Jay.Text;

namespace Jay.Dumping;

public static partial class Dumper
{
    private static readonly ConcurrentTypeDictionary<string> _typeDumpCache = new();
    
        private static void DumpTypeTo(Type? type, TextBuilder text)
    {
        if (type is null)
        {
            text.Write("null");
        }
        else
        {
            if (_typeDumpCache.TryGetValue(type, out var str))
            {
                text.Write(str);
            }
            // This writes + creates
            str = CreateTypeString(type, text);
            _typeDumpCache.TryAdd(type, str);
        }
        
    }

    private static string CreateTypeString(Type type, TextBuilder textBuilder)
    {
        var start = textBuilder.Length;
        if (type == typeof(bool))
            textBuilder.Write("bool");
        else if (type == typeof(char))
            textBuilder.Write("char");
        else if (type == typeof(sbyte))
            textBuilder.Write("sbyte");
        else if (type == typeof(byte))
            textBuilder.Write("byte");
        else if (type == typeof(short))
            textBuilder.Write("short");
        else if (type == typeof(ushort))
            textBuilder.Write("ushort");
        else if (type == typeof(int))
            textBuilder.Write("int");
        else if (type == typeof(uint))
            textBuilder.Write("uint");
        else if (type == typeof(long))
            textBuilder.Write("long");
        else if (type == typeof(ulong))
            textBuilder.Write("ulong");
        else if (type == typeof(float))
            textBuilder.Write("float");
        else if (type == typeof(double))
            textBuilder.Write("double");
        else if (type == typeof(decimal))
            textBuilder.Write("decimal");
        else if (type == typeof(string))
            textBuilder.Write("string");
        else if (type == typeof(object))
            textBuilder.Write("object");
        else if (type == typeof(void))
            textBuilder.Write("void");
        else if (type.IsEnum)
            textBuilder.Write(type.Name);
        else
        {
            // Print a Declaring Type for Nested Types
            if (type!.IsNested && !type.IsGenericParameter)
            {
                DumpTypeTo(type.DeclaringType, textBuilder);
                textBuilder.Write('.');
            }

            var underlyingType = Nullable.GetUnderlyingType(type);
            if (underlyingType is not null)
            {
                DumpTypeTo(underlyingType, textBuilder);
                textBuilder.Write('?');
            }
            else if (type.IsPointer)
            {
                underlyingType = type.GetElementType();
                Debug.Assert(underlyingType != null);
                DumpTypeTo(underlyingType, textBuilder);
                textBuilder.Write('*');
            }
            else if (type.IsByRef)
            {
                underlyingType = type.GetElementType();
                Debug.Assert(underlyingType != null);
                textBuilder.Write("ref ");
                DumpTypeTo(underlyingType, textBuilder);
            }
            else if (type.IsArray)
            {
                underlyingType = type.GetElementType();
                Debug.Assert(underlyingType != null);
                DumpTypeTo(underlyingType, textBuilder);
                textBuilder.Write("[]");
            }
            
            string name = type.Name;

            if (type.IsGenericType)
            {
                if (type.IsGenericParameter)
                {
                    textBuilder.Write(name);
                    var constraints = type.GetGenericParameterConstraints();
                    if (constraints.Length > 0)
                    {
                        textBuilder.Write(" : ");
                        Debugger.Break();
                    }
                    Debugger.Break();
                }

                var genericTypes = type.GetGenericArguments();
                var i = name.IndexOf('`');
                Debug.Assert(i >= 0);
                textBuilder.Append(name[..i])
                    .Append('<')
                    .AppendDelimit(",", 
                        genericTypes, 
                        (tb, gt) => DumpTypeTo(gt, tb))
                    .Write('>');
            }
            else
            {
                textBuilder.Write(name);
            }
        }

        var end = textBuilder.Length;
        int length = end - start;
        Debug.Assert(length > 0);
        return new string(textBuilder.Written.Slice(0, length));
    }
}