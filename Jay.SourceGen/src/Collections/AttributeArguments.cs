namespace Jay.SourceGen.Collections;

public sealed class AttributeArguments : Dictionary<string, object?>
{
    public AttributeArguments()
        : base(0, StringComparer.OrdinalIgnoreCase)
    {

    }

    public AttributeArguments(AttributeData attributeData)
        : base(StringComparer.OrdinalIgnoreCase)
    {
        var ctorArgs = attributeData.ConstructorArguments;
        if (ctorArgs.Length > 0)
        {
            var ctorParams = attributeData.AttributeConstructor?.Parameters ?? ImmutableArray<IParameterSymbol>.Empty;
            Debug.Assert(ctorArgs.Length == ctorParams.Length);
            int count = ctorArgs.Length;
            for (var i = 0; i < count; i++)
            {
                string name = ctorParams[i].Name;
                Debug.Assert(ContainsKey(name) == false);
                object? value = ctorArgs[i].GetObjectValue();
                this[name] = value;
            }
        }

        var namedArgs = attributeData.NamedArguments;
        if (namedArgs.Length > 0)
        {
            int count = namedArgs.Length;
            for (var i = 0; i < count; i++)
            {
                var arg = namedArgs[i];
                string name = arg.Key;
                Debug.Assert(ContainsKey(name) == false);
                object? value = arg.Value.GetObjectValue();
                this[name] = value;
            }
        }
    }

    public AttributeArguments(CustomAttributeData attrData)
    {
        var ctorArgs = attrData.ConstructorArguments;
        var ctorArgsLen = ctorArgs.Count;
        if (ctorArgsLen > 0)
        {
            var ctorParams = attrData.Constructor.GetParameters();
            Debug.Assert(ctorArgsLen == ctorParams.Length);
            for (var i = 0; i < ctorArgsLen; i++)
            {
                string name = ctorParams[i].Name;
                Debug.Assert(ContainsKey(name) == false);
                object? value = ctorArgs[i].Value;
                this[name] = value;
            }
        }

        var namedArgs = attrData.NamedArguments;
        if (namedArgs is not null)
        {
            var namedArgsLen = namedArgs.Count;
            if (namedArgsLen > 0)
            {
                for (var i = 0; i < namedArgsLen; i++)
                {
                    var arg = namedArgs[i];
                    string name = arg.MemberName;
                    Debug.Assert(ContainsKey(name) == false);
                    object? value = arg.TypedValue.Value;
                    this[name] = value;
                }
            }
        }
    }

    public bool TryGetValue<TValue>(
        [AllowNull, NotNullWhen(true)] string? name,
        [MaybeNullWhen(false)] out TValue? value)
    {
        if (name is not null 
            && base.TryGetValue(name, out object? objValue) 
            && objValue.Is(out value))
        {
            return true;
        }
        value = default;
        return false;
    }

    public override string ToString()
    {
        return TextBuilder
            .New
            .Append($"{Count} Attribute Arguments:")
            .NewLine()
            .Delimit(static cb => cb.NewLine(), this, (tb, pair) => tb.Append(pair.Key).Append(": ").Append(pair.Value))
            .ToStringAndDispose();
    }
}
