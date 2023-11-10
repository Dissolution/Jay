namespace Jay.SourceGen.Reflection;

public sealed class Attributes : List<AttributeSignature>, IToCode
{
    public static Attributes From(ISymbol? symbol)
    {
        var attributes = new Attributes();
        if (symbol is not null)
        {
            attributes.AddRange(
                symbol.GetAttributes()
                    .Select(static a => AttributeSignature.Create(a))
                    .Where(static a => a is not null)!);
        }

        return attributes;
    }

    public static Attributes From(MemberInfo? memberInfo)
    {
        var attributes = new Attributes();
        if (memberInfo is not null)
        {
            attributes.AddRange(
                memberInfo.GetCustomAttributesData()
                    .Select(static a => AttributeSignature.Create(a))
                    .Where(static a => a is not null)!);
        }
        return attributes;
    }
    
    public static Attributes From(ParameterInfo? parameterInfo)
    {
        var attributes = new Attributes();
        if (parameterInfo is not null)
        {
            attributes.AddRange(
                parameterInfo.GetCustomAttributesData()
                    .Select(static a => AttributeSignature.Create(a))
                    .Where(static a => a is not null)!);
        }
        return attributes;
    }
    
    public Attributes() : base(0){ }

    public bool WriteTo(CodeBuilder codeBuilder)
    {
        if (Count == 0) return false;
        codeBuilder.Append('[')
            .Delimit(", ", this, static (cb, attr) => cb.Append(attr))
            .Append(']');
        return true;
    }

    public override string ToString()
    {
        if (Count == 0) return string.Empty;
        return CodeBuilder.New
            .Append('[')
            .Delimit(", ", this, static (cb, attr) => cb.Append(attr))
            .Append(']')
            .ToStringAndDispose();
    }
}