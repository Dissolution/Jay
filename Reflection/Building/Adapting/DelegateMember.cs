namespace Jay.Reflection.Adapting;

public sealed class DelegateMember : IEquatable<DelegateMember>
{
    public static DelegateMember Create(DelegateSig sig, MemberInfo member)
    {
        ArgumentNullException.ThrowIfNull(sig);
        ArgumentNullException.ThrowIfNull(member);
        return new DelegateMember(sig, member);
    }

    public static DelegateMember Create<TDelegate>(MemberInfo member)
        where TDelegate : Delegate
    {
        ArgumentNullException.ThrowIfNull(member);
        return new DelegateMember(DelegateSig.Of<TDelegate>(), member);
    }

    public static DelegateMember Create<TMember>(DelegateSig sig, TMember member)
        where TMember : MemberInfo
    {
        ArgumentNullException.ThrowIfNull(sig);
        ArgumentNullException.ThrowIfNull(member);
        return new DelegateMember(sig, member);
    }

    public static DelegateMember Create<TDelegate, TMember>(TMember member)
        where TDelegate : Delegate
        where TMember : MemberInfo
    {
        ArgumentNullException.ThrowIfNull(member);
        return new DelegateMember(DelegateSig.Of<TDelegate>(), member);
    }

    public DelegateSig DelegateSig { get; }
    public MemberInfo Member { get; }

    private DelegateMember(DelegateSig sig, MemberInfo member)
    {
        this.DelegateSig = sig;
        this.Member = member;
    }
    public void Deconstruct(out DelegateSig delegateSig, out MemberInfo member)
    {
        delegateSig = DelegateSig;
        member = Member;
    }
    public bool Equals(DelegateMember? delMem)
    {
        return delMem is not null &&
               delMem.DelegateSig == this.DelegateSig &&
               delMem.Member == this.Member;
    }

    public override bool Equals(object? obj)
    {
        return obj is DelegateMember delMem && Equals(delMem);
    }

    public override int GetHashCode()
    {
        return HashCode.Combine(DelegateSig, Member);
    }

    public override string ToString()
    {
        return $"({DelegateSig}){Member}";
    }
}