namespace Jay.Reflection.Searching;

public sealed record class MemberSearchOptions : ICodePart
{
    public Visibility Visibility { get; init; } = Visibility.Any;

    public string? Name { get; init; } = null;

    public NameMatchOptions NameMatch { get; init; } = NameMatchOptions.Exact;

    public Type? ReturnType { get; init; } = null;

    public Type[]? ParameterTypes { get; init; } = null;

    public int? GenericTypeCount { get; init; } = null;
    //public bool ConvertableTypeMatch { get; init; } = false;

    public BindingFlags BindingFlags
    {
        get
        {
            BindingFlags flags = this.Visibility.ToBindingFlags();
            if (NameMatch.HasFlag<NameMatchOptions>(NameMatchOptions.IgnoreCase))
                flags.AddFlag(BindingFlags.IgnoreCase);
            return flags;
        }
    }

    public MemberSearchOptions()
    {
    }

    public MemberSearchOptions(string? name, NameMatchOptions nameMatchOptions = NameMatchOptions.Exact)
    {
        this.Name = name;
        this.NameMatch = nameMatchOptions;
    }

    public MemberSearchOptions(string? name, Visibility visibility)
    {
        this.Name = name;
        this.Visibility = visibility;
    }

    public MemberSearchOptions(
        string? name, Visibility visibility,
        Type returnType)
    {
        this.Name = name;
        this.Visibility = visibility;
        this.ReturnType = returnType;
    }

    public MemberSearchOptions(
        string? name, Type? returnType,
        params Type[]? parameterTypes)
    {
        this.Name = name;
        this.ReturnType = returnType;
        this.ParameterTypes = parameterTypes;
    }

    private bool MatchesName(string memberName)
    {
        if (this.Name is null)
            return true; // match any name
        if (memberName.Length < Name.Length)
            return false;
        if (NameMatch == NameMatchOptions.Exact)
            return string.Equals(memberName, Name);
        if (NameMatch.HasFlag(NameMatchOptions.IgnoreCase))
        {
            if (NameMatch.HasFlag(NameMatchOptions.Contains))
                return memberName.Contains(Name, StringComparison.OrdinalIgnoreCase);
            if (NameMatch.HasFlag(NameMatchOptions.StartsWith) && memberName.StartsWith(Name, StringComparison.OrdinalIgnoreCase))
                return true;
            if (NameMatch.HasFlag(NameMatchOptions.EndsWith) && memberName.EndsWith(Name, StringComparison.OrdinalIgnoreCase))
                return true;
            return string.Equals(
                memberName,
                Name,
                StringComparison.OrdinalIgnoreCase);
        }
        if (NameMatch.HasFlag(NameMatchOptions.Contains))
            return memberName.Contains(Name);
        if (NameMatch.HasFlag(NameMatchOptions.StartsWith) && memberName.StartsWith(Name))
            return true;
        if (NameMatch.HasFlag(NameMatchOptions.EndsWith) && memberName.EndsWith(Name))
            return true;
        return string.Equals(memberName, Name);
    }

    private bool MatchesReturnType(Type returnType)
    {
        if (ReturnType is null)
            return true;
        // if (ConvertableTypeMatch)
        // {
        //     if (!RuntimeMethodAdapter.CanAdaptType(returnType, ReturnType))
        //         return false;
        // }
        // else
        {
            if (returnType != ReturnType)
                return false;
        }
        return true;
    }

    private bool MatchesParameterType(Type sourceType, Type destType)
    {
        // if (ConvertableTypeMatch)
        // {
        //     if (!RuntimeMethodAdapter.CanAdaptType(sourceType, destType))
        //         return false;
        // }
        // else
        {
            if (sourceType != destType)
                return false;
        }
        return true;
    }

    private bool MatchesParameterTypes(Type[] argTypes)
    {
        if (ParameterTypes is null)
            return true;
        if (ParameterTypes.Length != argTypes.Length)
            return false;
        for (var i = 0; i < argTypes.Length; i++)
        {
            if (!MatchesParameterType(ParameterTypes[i], argTypes[i]))
                return false;
        }
        return true;
    }

    public bool Matches(MemberInfo member)
    {
        // We always know visibility matches, as we passed it to get this member

        if (!MatchesName(member.Name))
            return false;

        if (GenericTypeCount.TryGetValue(out var genericTypeCount) &&
            member.GenericTypeCount() != genericTypeCount)
            return false;

        if (member is FieldInfo field)
        {
            if (!MatchesReturnType(field.FieldType))
                return false;
            if (!MatchesParameterTypes(Type.EmptyTypes))
                return false;
            return true;
        }

        if (member is PropertyInfo property)
        {
            if (!MatchesReturnType(property.PropertyType))
                return false;
            if (!MatchesParameterTypes(property.GetIndexParameterTypes()))
                return false;
            return true;
        }

        if (member is EventInfo eventInfo)
        {
            if (!MatchesReturnType(eventInfo.EventHandlerType!))
                return false;
            if (!MatchesParameterTypes(Type.EmptyTypes))
                return false;
            return true;
        }

        if (member is ConstructorInfo ctor)
        {
            if (!MatchesReturnType(ctor.DeclaringType!))
                return false;
            if (!MatchesParameterTypes(ctor.GetParameterTypes()))
                return false;
            return true;
        }

        if (member is MethodInfo method)
        {
            if (!MatchesReturnType(method.ReturnType!))
                return false;
            if (!MatchesParameterTypes(method.GetParameterTypes()))
                return false;
            return true;
        }

        throw new NotImplementedException();
    }

    public void DeclareTo(CodeBuilder codeBuilder)
    {
        if (Visibility != Visibility.None)
        {
            codeBuilder.Append(Visibility);
        }
        if (ReturnType != null)
        {
            codeBuilder.Append(' ').Append(ReturnType);
        }
        if (Name is not null)
        {
            codeBuilder.Append($" \"{Name}\"");
            if (NameMatch != NameMatchOptions.Exact)
            {
                codeBuilder.Append($"({NameMatch})");
            }
        }
        if (ParameterTypes is not null)
        {
            codeBuilder.Append(" (").Delimit(static c => c.Write(", "), ParameterTypes, static (c,p) => c.Write(p));
        }
    }

    public override string ToString()
    {
        return CodePart.ToDeclaration(this);
    }
}