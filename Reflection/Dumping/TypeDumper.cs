using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Threading.Tasks.Sources;
using Jay.Collections;
using Jay.Text;
using Jay.Text.Dumping;

namespace Jay.Reflection.Dumping;

public abstract class Dumper : IDumper
{
    protected static bool DumpNull<T>(TextBuilder text, [NotNullWhen(false)] T? value, DumpLevel level)
    {
        if (value is null)
        {
            if (level.HasFlag<DumpLevel>(DumpLevel.Surroundings))
            {
                text.Write('(');
                Dumpers.Dump(text, typeof(T), DumpLevel.Self);
                text.Write(')');
            }
            text.Write("null");
            return true;
        }
        return false;
    }

    public abstract bool CanDump(Type type);

    public abstract void Dump(TextBuilder text, object? value, DumpLevel level = DumpLevel.Self);
}

public abstract class Dumper<T> : Dumper, IDumper<T>
{
    public sealed override bool CanDump(Type type) => type.IsAssignableTo(typeof(T));

    public sealed override void Dump(TextBuilder text, object? value, DumpLevel level = DumpLevel.Self)
    {
        if (value is T typed)
        {
            Dump(text, typed, level);
        }
        else
        {
            Dumpers.Dump(text, value, level);
        }
    }

    public abstract void Dump(TextBuilder text, T? value, DumpLevel level = DumpLevel.Self);
}

public class TypeDumper : Dumper<Type>
{
    public override void Dump(TextBuilder text, Type? type, DumpLevel level = DumpLevel.Self)
    {
        if (DumpNull(text, type, level)) return;

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
}

public class MemberDumper : Dumper<MemberInfo>
{
    private void Dump(TextBuilder text, FieldInfo field, DumpLevel level)
    {
        if (level.HasFlag(DumpLevel.Details))
        {
            text.Append(field.Visibility()).Append(' ');
        }

        Dumpers.Dump(text, field.FieldType, DumpLevel.Self);
        text.Write(' ');

        if (level.HasFlag(DumpLevel.Surroundings))
        {
            Dumpers.Dump(text, field.OwnerType(), DumpLevel.Self);
            text.Write('.');
        }

        text.Write(field.Name);
    }

    private void Dump(TextBuilder text, PropertyInfo property, DumpLevel level)
    {
        if (level.HasFlag(DumpLevel.Details))
        {
            var getVis = property.GetGetter().Visibility();
            var setVis = property.GetSetter().Visibility();
            Visibility highVis = getVis >= setVis ? getVis : setVis;
            text.Append(highVis).Write(' ');
            Dumpers.Dump(text, property.PropertyType, DumpLevel.Self);
            text.Write(' ');

            if (level.HasFlag(DumpLevel.Surroundings))
            {
                Dumpers.Dump(text, property.OwnerType(), DumpLevel.Self);
                text.Write('.');
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
                Dumpers.Dump(text, property.OwnerType(), DumpLevel.Self);
                text.Write('.');
            }

            text.Write(property.Name);
        }
    }

    public override void Dump(TextBuilder text, MemberInfo? value, DumpLevel level = DumpLevel.Self)
    {
        if (DumpNull(text, value, level)) return;
        switch (value)
        {
            case FieldInfo field:
                Dump(text, field, level);
                return;
            case PropertyInfo property:
                Dump(text, property, level);
                return;
        }
    }
}