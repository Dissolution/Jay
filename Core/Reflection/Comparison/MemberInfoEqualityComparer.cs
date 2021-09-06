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
        protected override bool EqualsImpl(MemberInfo x, MemberInfo y)
        {
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