namespace Jay.CodeGen.Scratch;


public interface IMember //: IToCode
{
    string Name { get; }
    Visibility Visibility { get; }
}

public interface IType : IMember
{
    bool IsNested();
    bool IsNested([NotNullWhen(true)] out IType? declaringType);
}

internal class MemberInfoMember : IMember
{
    protected readonly MemberInfo _memberInfo;

    public string Name => _memberInfo.Name;

    public Visibility Visibility => _memberInfo.GetVisibility();

    protected MemberInfoMember(MemberInfo memberInfo)
    {
        _memberInfo = memberInfo;
    }
}

internal class SymbolMember : IMember
{
    protected readonly ISymbol _symbol;

    public string Name => _symbol.Name;
    
    public Visibility Visibility => _symbol.GetVisibility();
}

internal class TypeMember : MemberInfoMember, IType
{
    protected readonly Type _type;

    public TypeMember(Type type) : base(type)
    {
        _type = type;
    }
    
    public bool IsNested() => _type.IsNested;
    public bool IsNested([NotNullWhen(true)] out IType? declaringType)
    {
        if (_type.IsNested)
        {
            declaringType = new TypeMember(_type.DeclaringType!);
            return true;
        }
        declaringType = null;
        return false;
    }
}

internal class TypeSymbolMember : SymbolMember, IType
{
    protected readonly ITypeSymbol _typeSymbol;

    public TypeSymbolMember(ITypeSymbol typeSymbol)
    {
        _typeSymbol = typeSymbol;
    }

    public bool IsNested() => _typeSymbol.ContainingType != null;
    public bool IsNested(out IType? declaringType)
    {
        var ctype = _typeSymbol.ContainingType;
        if (ctype is not null)
        {
            declaringType = new TypeSymbolMember(ctype);
            return true;
        }
        declaringType = null;
        return false;
    }
}