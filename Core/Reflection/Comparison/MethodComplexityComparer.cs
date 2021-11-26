using System.Collections.Generic;
using System.Reflection;
using Jay.Comparison;

namespace Jay.Reflection.Comparison
{
    public sealed class MethodComplexityComparer : Comparer<MethodBase>
    {
        /// <inheritdoc />
        public override int Compare(MethodBase? x, MethodBase? y)
        {
            if (x.IsPublic != y.IsPublic)
            {
                if (x.IsPublic)
                {
                    return -1;
                }
                return 1;
            }

            var xParams = x.GetParameters();
            var yParams = y.GetParameters();
            int c = xParams.Length.CompareTo(yParams.Length);
            if (c != 0) return c;

            for (var i = 0; i < xParams.Length; i++)
            {
                c = ParameterComplexityComparer.Default.Compare(xParams[i], yParams[i]);
                if (c != 0) return c;
            }

            return 0;
        }
    }
}