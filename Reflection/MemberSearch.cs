using System;
using System.Reflection;

namespace Jay.Reflection;

public readonly struct MemberSearch
{
    public static implicit operator MemberSearch(Access access) => new MemberSearch(access, NameMatch.Any, MemberTypes.All);
    public static implicit operator MemberSearch(string? name) => new MemberSearch(Access.Any, new NameMatch(name), MemberTypes.All);
    public static implicit operator MemberSearch(MemberTypes memberTypes) => new MemberSearch(Access.Any, NameMatch.Any, memberTypes);
        
    public static readonly MemberSearch All = new MemberSearch(Access.Any, NameMatch.Any, MemberTypes.All);
        
    public readonly Access Access;
    public readonly NameMatch NameMatch;
    public readonly MemberTypes MemberTypes;

    public MemberSearch(Access access, NameMatch nameMatch, MemberTypes memberTypes)
    {
        Access = access;
        NameMatch = nameMatch;
        MemberTypes = memberTypes;
    }

    public bool Matches(MemberInfo member)
    {
        ArgumentNullException.ThrowIfNull(member);
        if (!this.Access.HasAnyFlags<Access>(member.Access()))
            return false;
        if (!this.NameMatch.Matches(member.Name))
            return false;
        if (!this.MemberTypes.HasFlag<MemberTypes>(member.MemberType))
            return false;
        return true;
    }

    /// <inheritdoc />
    public override bool Equals(object? obj)
    {
        if (obj is MemberSearch search)
            return search.Access == this.Access &&
                   search.NameMatch == this.NameMatch &&
                   search.MemberTypes == this.MemberTypes;
        if (obj is MemberInfo member)
            return Matches(member);
        return false;
    }

    /// <inheritdoc />
    public override int GetHashCode()
    {
        return HashCode.Combine(Access, NameMatch, MemberTypes);
    }

    /// <inheritdoc />
    public override string ToString()
    {
        return $"{Access} {MemberTypes} {NameMatch}";
    }
}