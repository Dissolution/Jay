/*using System.Diagnostics;
using Jay.Collections;
using Jay.Text;

namespace Jay.Dumping.Refactor;

internal sealed class TypeDumper : ValueDumper<Type>, IDumper<Type>
{
    private static readonly HashSet<string> _ignoredNamespaces = new(StringComparer.OrdinalIgnoreCase)
    {
        "System",
        "Microsoft",
    };

    private static readonly ConcurrentTypeDictionary<DumpValue<Type>> _cache = new();
    private static readonly DumpValue<Type> _cacheDumpValue;

    static TypeDumper()
    {
        _cacheDumpValue = (Type? type, TextBuilder text, DumpOptions? options) =>
        {
            if (type is null) return;
            var dumper = _cache.GetOrAdd(type, CreateDumpDelegate);
            dumper(type, text, options);
        };
    }
    
    public static void Dump(Type? type, TextBuilder text, DumpOptions? options = default)
    {
        _cacheDumpValue(type, text, options ?? DumpOptions.Default);
    }

    private static DumpValue<Type> CreateDumpDelegate(Type valueType)
    {
        if (valueType == typeof(bool)) return DumpConst("bool");
        if (valueType == typeof(char)) return DumpConst("char");
        if (valueType == typeof(sbyte)) return DumpConst("sbyte");
        if (valueType == typeof(byte)) return DumpConst("byte");
        if (valueType == typeof(short)) return DumpConst("short");
        if (valueType == typeof(ushort)) return DumpConst("ushort");
        if (valueType == typeof(int)) return DumpConst("int");
        if (valueType == typeof(uint)) return DumpConst("uint");
        if (valueType == typeof(long)) return DumpConst("long");
        if (valueType == typeof(ulong)) return DumpConst("ulong");
        if (valueType == typeof(float)) return DumpConst("float");
        if (valueType == typeof(double)) return DumpConst("double");
        if (valueType == typeof(decimal)) return DumpConst("decimal");
        if (valueType == typeof(string)) return DumpConst("string");
        if (valueType == typeof(object)) return DumpConst("object");

        // TODO: deep print namespace

        Type? underlyingType;

        // Enums
        if (valueType.IsEnum)
        {
            return DumpConst(valueType.Name);
        }

        // Everything else is implementation
        return (Type? type, TextBuilder text, DumpOptions? options) =>
        {
            // Print a Declaring Type for Nested Types
            if (type!.IsNested && !type.IsGenericParameter)
            {
                Dump(type.DeclaringType, text, options);
                text.Write('.');
            }

            underlyingType = Nullable.GetUnderlyingType(type);
            if (underlyingType is not null)
            {
                Dump(underlyingType, text, options);
                text.Write('?');
                return;
            }

            if (type.IsPointer)
            {
                underlyingType = type.GetElementType();
                Debug.Assert(underlyingType != null);
                Dump(underlyingType, text, options);
                text.Write('*');
                return;
            }

            if (type.IsByRef)
            {
                underlyingType = type.GetElementType();
                Debug.Assert(underlyingType != null);
                text.Write("ref ");
                Dump(underlyingType, text, options);
                return;
            }

            if (type.IsArray)
            {
                underlyingType = type.GetElementType();
                Debug.Assert(underlyingType != null);
                Dump(underlyingType, text, options);
                text.Write("[]");
                return;
            }

            string name = type.Name;

            if (type.IsGenericType)
            {
                if (type.IsGenericParameter)
                {
                    text.Write(name);
                    var constraints = type.GetGenericParameterConstraints();
                    if (constraints.Length > 0 && options?.Verbose == true)
                    {
                        text.Write(" : ");
                        Debugger.Break();
                    }

                    return;
                }

                var genericTypes = type.GetGenericArguments();
                var i = name.IndexOf('`');
                Debug.Assert(i >= 0);
                text.Append(name[..i])
                    .Append('<')
                    .AppendDelimit(",", 
                        genericTypes, 
                        (tb, gt) => Dump(gt, tb, options))
                    .Append('>');
            }
            else
            {
                text.Write(name);
            }
        };
    }
}*/