using System.Collections.Generic;
using System.Reflection;
using Jay.Comparison;
using Jay.Reflection;
using ParameterModifier = Jay.Reflection.Emission.ParameterModifier;

namespace Jay.Reflection.Comparison
{
    public sealed class ParameterComplexityComparer : Comparer<ParameterInfo>
    {
        /// <inheritdoc />
        public override int Compare(ParameterInfo? left, ParameterInfo? right)
        {
            var lMod = left.GetParameterModifier();
            var rMod = right.GetParameterModifier();
            if (lMod != rMod)
            {
                if (lMod == ParameterModifier.Default)
                    return -1;
                return 1;
            }

            var lType = left.ParameterType;
            var rType = right.ParameterType;
            if (lType != rType)
            {
                if (lType.IsValueType != rType.IsValueType)
                {
                    if (lType.IsValueType)
                        return -1;
                    return 1;
                }

                if (lType == typeof(object))
                    return -1;
                if (rType == typeof(object))
                    return 1;

                if (lType.IsInterface != rType.IsInterface)
                {
                    if (lType.IsInterface)
                        return 1;
                    return -1;
                }

                int c = lType.GetInterfaces().Length.CompareTo(rType.GetInterfaces().Length);
                if (c != 0)
                    return c;
            }
            return 0;
        }
    }
}