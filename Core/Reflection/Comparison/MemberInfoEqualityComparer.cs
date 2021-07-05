using System.Collections.Generic;
using System.Reflection;
using Jay.Comparison;

namespace Jay.Reflection.Comparison
{
    /// <summary>
    /// An <see cref="IEqualityComparer{T}"/> for <see cref="MemberInfo"/>
    /// </summary>
    public sealed class MemberInfoEqualityComparer : EqualityComparerBase<MemberInfo, MemberInfoEqualityComparer>
    {
        /// <inheritdoc />
        public override bool Equals(MemberInfo? x, MemberInfo? y)
        {
            if (ReferenceEquals(x, y))
                return true;
            if (x is null || y is null)
                return false;
            return x.MetadataToken == y.MetadataToken &&
                   x.Module == y.Module;
        }

        /// <inheritdoc />
        public override int GetHashCode(MemberInfo? member)
        {
            return member is null ? 0 : Hasher.Create(member.MetadataToken, member.Module);
        }
    }
}