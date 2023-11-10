namespace Jay.SourceGen.Utilities;

public static class Naming
{
    public static string GetImplementationName(string interfaceName)
    {
        if (interfaceName.StartsWith("I"))
            return interfaceName.Substring(1);
        else
            return "impl_" + interfaceName;
    }

    public static string GetFieldName(IPropertySymbol property)
    {
        string propertyName = property.Name;
        if (string.IsNullOrWhiteSpace(propertyName))
        {
            return $"_field_{Guid.NewGuid():N}";
        }
        Span<char> fieldName = stackalloc char[propertyName.Length + 1];
        fieldName[0] = '_';
        fieldName[1] = char.ToLower(propertyName[0]);
        propertyName.AsSpan(1).CopyTo(fieldName.Slice(2));
        return fieldName.ToString();
    }

    private static long _counter = 0L;
    private static readonly ImmutableHashSet<string> _keywords =
        SyntaxFacts.GetKeywordKinds().Select(SyntaxFacts.GetText).ToImmutableHashSet();
    
    public static string GetVariableName(string? otherName)
    {
        if (otherName is null)
            return $"___{Interlocked.Increment(ref _counter)}";
        int otherNameLength = otherName.Length;
        if (otherNameLength == 0)
            return $"___{Interlocked.Increment(ref _counter)}";
        
        int i = 0;
        if (otherName[i] == '_')
            i++;
        if (otherName.Length > 2 && otherName[i] == 'I' && char.IsUpper(otherName[i+1]))
            i++;
        char firstChar = otherName[i];
        if (i == 0 && char.IsLower(firstChar))
        {
            return $"{otherName}{Interlocked.Increment(ref _counter)}";
        }

        Span<char> name = stackalloc char[otherName.Length - i];
        name[0] = char.ToLower(otherName[i]);
        i++;
        otherName.AsSpan(i).CopyTo(name.Slice(1));
        string varName = name.ToString();
        if (!_keywords.Contains(varName)) return varName;
        return $"@{varName}";
    }
}