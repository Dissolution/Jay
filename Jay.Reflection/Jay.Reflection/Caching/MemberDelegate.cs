using Jay.Reflection.Info;
using Jay.Utilities;

namespace Jay.Reflection.Caching;

public class MemberDelegate : IEquatable<MemberDelegate>
{
    public static MemberDelegate Create(MemberInfo memberInfo, DelegateInfo delegateInfo) 
        => new MemberDelegate(memberInfo, delegateInfo);
    public static MemberDelegate<TDelegate> Create<TDelegate>(MemberInfo memberInfo)
        where TDelegate : Delegate
        => new MemberDelegate<TDelegate>(memberInfo);
    public static MemberDelegate<TMember, TDelegate> Create<TMember, TDelegate>(TMember member)
        where TMember : MemberInfo
        where TDelegate : Delegate
        => new MemberDelegate<TMember, TDelegate>(member);



    public MemberInfo MemberInfo { get; }
    public DelegateInfo DelegateInfo { get; }

    public MemberDelegate(MemberInfo memberInfo, DelegateInfo delegateInfo)
    {
        MemberInfo = memberInfo;
        DelegateInfo = delegateInfo;
    }
    
    public bool Equals(MemberDelegate? memberDelegate)
    {
        return memberDelegate is not null &&
            memberDelegate.MemberInfo == this.MemberInfo &&
            memberDelegate.DelegateInfo == this.DelegateInfo;
    }
    public override bool Equals(object? obj)
    {
        if (obj is MemberDelegate memberDelegate)
            return Equals(memberDelegate);
        return false;
    }
    public override int GetHashCode()
    {
        return Hasher.Combine(MemberInfo, DelegateInfo);
    }
    public override string ToString()
    {
        return $"{this.MemberInfo} - {this.DelegateInfo}";
    }
}

public class MemberDelegate<TDelegate> : MemberDelegate
    where TDelegate : Delegate
{
    public MemberDelegate(MemberInfo memberInfo) 
        : base(memberInfo, DelegateInfo.For<TDelegate>())
    {

    }
}

public class MemberDelegate<TMember, TDelegate> : MemberDelegate<TDelegate>
    where TMember : MemberInfo
    where TDelegate : Delegate
{
    public TMember Member { get; }
    
    public MemberDelegate(TMember member) 
        : base(member)
    {
        this.Member = member;
    }
}