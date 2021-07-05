using System.Reflection;
using Jay.Comparison;

namespace Jay.Reflection.Comparison
{
    public sealed class ParameterInfoEqualityComparer : EqualityComparerBase<ParameterInfo, ParameterInfoEqualityComparer>
    {
        public ParameterInfoEqualityComparer()
        {
            
        }
        
        /// <inheritdoc />
        public override bool Equals(ParameterInfo? left, ParameterInfo? right)
        {
            if (ReferenceEquals(left, right)) return true;
            if (left is null || right is null) return false;
            return left.Position == right.Position &&
                   left.ParameterType == right.ParameterType &&
                   left.IsIn == right.IsIn &&
                   left.IsOut == right.IsOut &&
                   left.IsOptional == right.IsOptional;
        }
    }
}