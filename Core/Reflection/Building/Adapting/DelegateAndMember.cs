using System.Reflection;

namespace Jay.Reflection.Building.Adapting;

public sealed class DelegateAndMember : IEquatable<DelegateAndMember>
{
    public static DelegateAndMember Create(DelegateSig sig, MemberInfo member)
    {
        ArgumentNullException.ThrowIfNull(sig);
        ArgumentNullException.ThrowIfNull(member);
        return new DelegateAndMember(sig, member);
    }

    public static DelegateAndMember Create<TDelegate>(MemberInfo member)
        where TDelegate : Delegate
    {
        ArgumentNullException.ThrowIfNull(member);
        return new DelegateAndMember(DelegateSig.Of<TDelegate>(), member);
    }

    public static DelegateAndMember Create<TMember>(DelegateSig sig, TMember member)
        where TMember : MemberInfo
    {
        ArgumentNullException.ThrowIfNull(sig);
        ArgumentNullException.ThrowIfNull(member);
        return new DelegateAndMember(sig, member);
    }

    public static DelegateAndMember Create<TDelegate, TMember>(TMember member)
        where TDelegate : Delegate
        where TMember : MemberInfo
    {
        ArgumentNullException.ThrowIfNull(member);
        return new DelegateAndMember(DelegateSig.Of<TDelegate>(), member);
    }

    public DelegateSig DelegateSig { get; }
    public MemberInfo Member { get; }

    private DelegateAndMember(DelegateSig sig, MemberInfo member)
    {
        this.DelegateSig = sig;
        this.Member = member;
    }
    public void Deconstruct(out DelegateSig delegateSig, out MemberInfo member)
    {
        delegateSig = DelegateSig;
        member = Member;
    }
    public bool Equals(DelegateAndMember? delMem)
    {
        return delMem is not null &&
               delMem.DelegateSig == this.DelegateSig &&
               delMem.Member == this.Member;
    }

    public override bool Equals(object? obj)
    {
        return obj is DelegateAndMember delMem && Equals(delMem);
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