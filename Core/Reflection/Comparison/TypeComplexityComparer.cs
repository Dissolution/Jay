using System;
using Jay.Comparison;

namespace Jay.Reflection.Comparison
{
    public sealed class TypeComplexityComparer : ComparerBase<Type, TypeComplexityComparer>
    {
        private static int Count(Type? type)
        {
            int count = 0;
            if (type is null) return count;
            count += type.GetInterfaces().Length;
            do
            {
                count++;
                type = type.BaseType;
            } while (type != null && type != typeof(object));
            return count;
        }
        
        /// <inheritdoc />
        protected override int CompareImpl(Type left, Type right)
        {
            return Count(left).CompareTo(Count(right));
        }
    }
}