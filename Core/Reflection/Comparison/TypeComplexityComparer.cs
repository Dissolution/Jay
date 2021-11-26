using System;
using System.Collections.Generic;
using Jay.Comparison;

namespace Jay.Reflection.Comparison
{
    public sealed class TypeComplexityComparer : Comparer<Type>
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
        public override int Compare(Type? left, Type? right)
        {
            return Count(left).CompareTo(Count(right));
        }
    }
}