using System;
using System.Reflection;

namespace Jay.Reflection;

public readonly struct MemberSearch
{
    public static implicit operator MemberSearch(Visibility visibility) => new MemberSearch(visibility, NameMatch.Any, MemberTypes.All);
    public static implicit operator MemberSearch(string? name) => new MemberSearch(Visibility.Any, new NameMatch(name), MemberTypes.All);
    public static implicit operator MemberSearch(MemberTypes memberTypes) => new MemberSearch(Visibility.Any, NameMatch.Any, memberTypes);
        
    public static readonly MemberSearch All = new MemberSearch(Visibility.Any, NameMatch.Any, MemberTypes.All);
        
    public readonly Visibility Visibility;
    public readonly NameMatch NameMatch;
    public readonly MemberTypes MemberTypes;

    public MemberSearch(Visibility visibility, NameMatch nameMatch, MemberTypes memberTypes)
    {
        Visibility = visibility;
        NameMatch = nameMatch;
        MemberTypes = memberTypes;
    }

    public bool Matches(MemberInfo member)
    {
        ArgumentNullException.ThrowIfNull(member);
        if (!this.Visibility.HasAnyFlags<Visibility>(member.Access()))
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
            return search.Visibility == this.Visibility &&
                   search.NameMatch == this.NameMatch &&
                   search.MemberTypes == this.MemberTypes;
        if (obj is MemberInfo member)
            return Matches(member);
        return false;
    }

    /// <inheritdoc />
    public override int GetHashCode()
    {
        return HashCode.Combine(Visibility, NameMatch, MemberTypes);
    }

    /// <inheritdoc />
    public override string ToString()
    {
        return $"{Visibility} {MemberTypes} {NameMatch}";
    }
}