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
        protected override bool EqualsImpl(ParameterInfo left, ParameterInfo right)
        {
            return left.Position == right.Position &&
                   left.ParameterType == right.ParameterType &&
                   left.IsIn == right.IsIn &&
                   left.IsOut == right.IsOut &&
                   left.IsOptional == right.IsOptional;
        }
    }
}