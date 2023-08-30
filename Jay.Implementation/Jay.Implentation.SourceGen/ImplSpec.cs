namespace IMPL.SourceGen;

public sealed class ImplSpec
{
    public required TypeSig ImplType { get; init; }
    public required IReadOnlyList<TypeSig> InterfaceTypes { get; init; }
    public required IReadOnlyList<MemberSig> Members { get; init; }
}




