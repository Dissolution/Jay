using System.Collections.Generic;
using System.Reflection;

namespace Jay.Reflection
{
    /// <summary>
    /// An <see cref="IEqualityComparer{T}"/> for <see cref="MemberInfo"/>
    /// </summary>
    public class MemberInfoEqualityComparer : IEqualityComparer<MemberInfo>
    {
        public static MemberInfoEqualityComparer Instance { get; } = new MemberInfoEqualityComparer();
        
        /// <inheritdoc />
        public bool Equals(MemberInfo? x, MemberInfo? y)
        {
            if (ReferenceEquals(x, y))
                return true;
            if (x is null || y is null)
                return false;
            return x.MetadataToken == y.MetadataToken &&
                   x.Module == y.Module;
        }

        /// <inheritdoc />
        public int GetHashCode(MemberInfo? member)
        {
            return member is null ? 0 : Hasher.Create(member.MetadataToken, member.Module);
        }
    }
}