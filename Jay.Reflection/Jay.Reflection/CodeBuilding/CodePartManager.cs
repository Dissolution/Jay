using System.Diagnostics;
using Jay.Collections;

namespace Jay.Reflection.CodeBuilding;

public static class CodePartManager
{
   
    
    
    private static readonly ConcurrentTypeMap<string> _cachedTypeRenders = new()
    {
        [typeof(DBNull)] = "DBNull",
        [typeof(bool)] = "bool",
        [typeof(char)] = "char",
        [typeof(sbyte)] = "sbyte",
        [typeof(byte)] = "byte",
        [typeof(short)] = "short",
        [typeof(ushort)] = "ushort",
        [typeof(int)] = "int",
        [typeof(uint)] = "uint",
        [typeof(long)] = "long",
        [typeof(ulong)] = "ulong",
        [typeof(float)] = "float",
        [typeof(double)] = "double",
        [typeof(decimal)] = "decimal",
        [typeof(string)] = "string",
        [typeof(object)] = "object",
        [typeof(void)] = "void",
        [typeof(nint)] = "nint",
        [typeof(nuint)] = "nuint",
    };

    private static string GetCodePartString(Type type)
    {
        using CodeBuilder code = new();
        Type? underType;

        // ref
        if (type.IsByRef)
        {
            underType = type.GetElementType()
                .ThrowIfNull();
            return code.Write("ref ")
                .Code(underType)
                .ToString();
        }

        // Array
        if (type.IsArray)
        {
            underType = type.GetElementType()
                .ThrowIfNull();
            return code.Code(underType)
                .Write("[]")
                .ToString();
        }

        // Pointer
        if (type.IsPointer)
        {
            underType = type.GetElementType()
                .ThrowIfNull();
            return code.Code(underType)
                .Write('*')
                .ToString();
        }

        // Nullable
        underType = Nullable.GetUnderlyingType(type);
        if (underType is not null)
        {
            return code
                .Code(underType)
                .Write('?')
                .ToString();
        }

        // Nested Type?
        if (type.IsNested && !type.IsGenericParameter)
        {
            underType = type.DeclaringType.ThrowIfNull();
            code.Write($"{underType}.");
        }

        // Fast non-generic return
        if (!type.IsGenericType)
        {
            // just the name
            return code.Write(type.Name)
                .ToString();
        }

        // Process complex name
        ReadOnlySpan<char> typeName = type.Name.AsSpan();

        // A parameter?
        if (type.IsGenericParameter)
        {
            var constraints = type.GetGenericParameterConstraints();
            Debugger.Break();
        }

        // Name is often something like NAME`2 for NAME<,>, so we want to strip that off
        var i = typeName.IndexOf('`');
        if (i >= 0)
        {
            code.Write(typeName[..i]);
        }
        else
        {
            code.Write(typeName);
        }

        // Add our generic types
        return code
            .Write('<')
            .DelimitCode(", ", type.GetGenericArguments())
            .Write('>')
            .ToString();
    }

    internal static bool DeclareNull<T>([AllowNull, NotNullWhen(false)] T? value, CodeBuilder code)
    {
        if (value is null)
        {
            code.Write($"({typeof(T)})null");
            return true;
        }
        return false;
    }
    
    public static void DeclareTo(this Type? type, CodeBuilder code)
    {
        if (DeclareNull(type, code)) return;
        string name = _cachedTypeRenders.GetOrAdd(type, GetCodePartString);
        code.Write(name);
    }

    public static void DeclareTo(this FieldInfo? field, CodeBuilder code)
    {
        if (DeclareNull(field, code)) return;

        code.Write($"{field.Visibility()} {field.Name};");
    }

    public static void WriteCodeTo(this PropertyInfo? property, CodeBuilder code)
    {
        if (DeclareNull(property, code)) return;
        code.Write($"{property.PropertyType} {property.OwnerType()}.{property.Name}");
        /*
         *    else // format >= DumpFormat.Inspect)
        {
            var getVis = property.GetGetter().Visibility();
            var setVis = property.GetSetter().Visibility();
            Visibility highVis = getVis >= setVis ? getVis : setVis;
            stringHandler.Value(highVis);
            stringHandler.Write(' ');

            stringHandler.Dump(property.PropertyType, format);
            stringHandler.Write(' ');
            stringHandler.Dump(property.OwnerType(), format);
            stringHandler.Write('.');
            stringHandler.Value(property.Name);
            stringHandler.Write(" {");
            if (getVis != Visibility.None)
            {
                if (getVis != highVis)
                    stringHandler.Value(getVis);
                stringHandler.Write(" get; ");
            }

            if (setVis != Visibility.None)
            {
                if (setVis != highVis)
                    stringHandler.Value(setVis);
                stringHandler.Write(" set; ");
            }
            stringHandler.Write('}');
        }
         */
    }

    public static void WriteCodeTo(this EventInfo? eventInfo, CodeBuilder code)
    {
        if (DeclareNull(eventInfo, code)) return;
        code.Write($"{eventInfo.EventHandlerType} {eventInfo.OwnerType()}.{eventInfo.Name}");
    }

    public static void WriteCodeTo(this ConstructorInfo? ctor, CodeBuilder code)
    {
        if (DeclareNull(ctor, code)) return;
        code
            .Write($"{ctor.DeclaringType}.ctor(")
            .DelimitCode(", ", ctor.GetParameters())
            .Write(')');
    }

    public static void WriteCodeTo(this MethodInfo? method, CodeBuilder code)
    {
        if (DeclareNull(method, code)) return;
        code.Write($"{method.ReturnType} {method.OwnerType()}.{method.Name}");
        if (method.IsGenericMethod)
        {
            code
                .Write('<')
                .DelimitCode(",", method.GetGenericArguments())
                .Write('>');
        }
        code
            .Write('(')
            .DelimitCode(", ", method.GetParameters())
            .Write(')');
    }

    public static void WriteCodeTo(this ParameterInfo? parameter, CodeBuilder code)
    {
        if (DeclareNull(parameter, code)) return;
        ParameterAccess access = parameter.GetAccess(out var parameterType);
        switch (access)
        {
            case ParameterAccess.In:
                code.Write("in ");
                break;
            case ParameterAccess.Ref:
                code.Write("ref ");
                break;
            case ParameterAccess.Out:
                code.Write("out ");
                break;
            case ParameterAccess.Default:
            default:
                break;
        }
        code
            .Code(parameterType)
            .Write(' ')
            .Write(parameter.Name ?? "???");
        if (parameter.HasDefaultValue)
        {
            code
                .Write(" = ")
                .Code(parameter.DefaultValue);
        }
    }
}