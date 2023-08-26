using System.Diagnostics;
using Jay.Collections;

namespace Jay.Reflection.CodeBuilding;

public static class MemberInfoToCode
{
    private static readonly ConcurrentTypeMap<string> _cachedTypeRenders = new()
    {
        (typeof(DBNull), nameof(DBNull)),
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
        (typeof(nint), "nint"),
        (typeof(nuint), "nuint"),
        (typeof(DateTime), nameof(DateTime)),
        (typeof(DateTimeOffset), nameof(DateTimeOffset)),
        (typeof(TimeSpan), nameof(TimeSpan)),
        (typeof(Guid), nameof(Guid)),
#if NET6_0_OR_GREATER
        (typeof(TimeOnly), nameof(TimeOnly)),
        (typeof(DateOnly), nameof(DateOnly)),
#endif
    };

    private static string GetTypeCodeString(Type type)
    {
        using CodeBuilder code = new();
        Type? underType;

        // ref
        if (type.IsByRef)
        {
            underType = type.GetElementType().ThrowIfNull();
            return code.Append("ref ")
                .Append(underType)
                .ToString();
        }

        // Array
        if (type.IsArray)
        {
            underType = type.GetElementType().ThrowIfNull();
            return code.Append(underType)
                .Append("[]")
                .ToString();
        }

        // Pointer
        if (type.IsPointer)
        {
            underType = type.GetElementType().ThrowIfNull();
            return code.Append(underType)
                .Append('*')
                .ToString();
        }

        // Nullable
        underType = Nullable.GetUnderlyingType(type);
        if (underType is not null)
        {
            return code
                .Append(underType)
                .Append('?')
                .ToString();
        }

        // Nested Type?
        if (type.IsNested && !type.IsGenericParameter)
        {
            underType = type.DeclaringType.ThrowIfNull();
            code.Append($"{underType}.");
        }

        // Fast non-generic return
        if (!type.IsGenericType)
        {
            // just the name
            return code.Append(type.Name)
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
            code.Append(typeName[..i]);
        }
        else
        {
            code.Append(typeName);
        }

        // Add our generic types
        return code
            .Append('<')
            .DelimitAppend(", ", type.GetGenericArguments())
            .Append('>')
            .ToString();
    }

   

    public static void WriteCodeTo(this MemberInfo? member, CodeBuilder code)
    {
        if (code.CheckNull(member))
            return;
        switch (member)
        {
            case EventInfo eventInfo:
                WriteCodeTo(eventInfo, code);
                break;
            case ConstructorInfo constructorInfo:
                WriteCodeTo(constructorInfo, code);
                break;
            case Type type:
                WriteCodeTo(type, code);
                break;
            case FieldInfo field:
                WriteCodeTo(field, code);
                break;
            case MethodInfo methodInfo:
                WriteCodeTo(methodInfo, code);
                break;
            case PropertyInfo propertyInfo:
                WriteCodeTo(propertyInfo, code);
                break;
            default:
                throw new ArgumentException(nameof(member));
        }
    }
    
    
    public static void WriteCodeTo(this Type? type, CodeBuilder code)
    {
        if (code.CheckNull(type))
            return;
        string name = _cachedTypeRenders.GetOrAdd(type, static t => GetTypeCodeString(t));
        code.Append(name);
    }

    public static void WriteCodeTo(this FieldInfo? field, CodeBuilder code)
    {
        if (code.CheckNull(field))
            return;
        code.Append($"{field.FieldType} {field.OwnerType()}.{field.Name}");
    }

    public static void WriteCodeTo(this PropertyInfo? property, CodeBuilder code)
    {
        if (code.CheckNull(property))
            return;
        code.Append($"{property.PropertyType} {property.OwnerType()}.{property.Name}");
        /*
         *    else // format >= DumpFormat.Inspect)
        {
            var getVis = property.GetGetter().Visibility();
            var setVis = property.GetSetter().Visibility();
            Visibility highVis = getVis >= setVis ? getVis : setVis;
            stringHandler.Value(highVis);
            stringHandler.Append(' ');

            stringHandler.Dump(property.PropertyType, format);
            stringHandler.Append(' ');
            stringHandler.Dump(property.OwnerType(), format);
            stringHandler.Append('.');
            stringHandler.Value(property.Name);
            stringHandler.Append(" {");
            if (getVis != Visibility.None)
            {
                if (getVis != highVis)
                    stringHandler.Value(getVis);
                stringHandler.Append(" get; ");
            }

            if (setVis != Visibility.None)
            {
                if (setVis != highVis)
                    stringHandler.Value(setVis);
                stringHandler.Append(" set; ");
            }
            stringHandler.Append('}');
        }
         */

    }

    public static void WriteCodeTo(this EventInfo? eventInfo, CodeBuilder code)
    {
        if (code.CheckNull(eventInfo))
            return;
        code.Append($"{eventInfo.EventHandlerType} {eventInfo.OwnerType()}.{eventInfo.Name}");
    }

    public static void WriteCodeTo(this ConstructorInfo? ctor, CodeBuilder code)
    {
        if (code.CheckNull(ctor))
            return;
        code
            .Append($"{ctor.DeclaringType}.ctor(")
            .DelimitAppend(", ", ctor.GetParameters())
            .Append(')');
    }

    public static void WriteCodeTo(this MethodInfo? method, CodeBuilder code)
    {
        if (code.CheckNull(method))
            return;
        code.Append($"{method.ReturnType} {method.OwnerType()}.{method.Name}");
        if (method.IsGenericMethod)
        {
            code
                .Append('<')
                .DelimitAppend(",", method.GetGenericArguments())
                .Append('>');
        }
        code
            .Append('(')
            .DelimitAppend(", ", method.GetParameters())
            .Append(')');
    }

    public static void WriteCodeTo(this ParameterInfo? parameter, CodeBuilder code)
    {
        if (code.CheckNull(parameter))
            return;
        ParameterAccess access = parameter.GetAccess(out var parameterType);
        switch (access)
        {
            case ParameterAccess.In:
                code.Append("in ");
                break;
            case ParameterAccess.Ref:
                code.Append("ref ");
                break;
            case ParameterAccess.Out:
                code.Append("out ");
                break;
            case ParameterAccess.Default:
            default:
                break;
        }
        code
            .Append(parameterType)
            .Append(' ')
            .Append(parameter.Name ?? "???");
        if (parameter.HasDefaultValue)
        {
            code
                .Append(" = ")
                .Append(parameter.DefaultValue);
        }
    }
}