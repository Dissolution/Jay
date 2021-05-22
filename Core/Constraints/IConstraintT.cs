using System;
using System.Diagnostics.CodeAnalysis;
using System.Linq;

namespace Jay.Constraints
{
    public static class Constraint
    {
        public static IConstraint<T> Between<T>(T? inclusiveMinimum, T? inclusiveMaximum)
            where T : IComparable<T>
        {
            return new FuncConstraint<T>(value => value.IsBetween(inclusiveMinimum, inclusiveMaximum));
        }

        public static IConstraint<T> EqualsAny<T>(params T?[] values)
        {
            return new FuncConstraint<T>(value => values.Contains(value));
        }

        public static IConstraint<T> NotEqualsAny<T>(params T?[] values)
        {
            return new FuncConstraint<T>(value => !values.Contains(value));
        }
    }

    internal sealed class FuncConstraint<T> : IConstraint<T>
    {
        private readonly Func<T?, bool> _match;


        public FuncConstraint(Func<T?, bool> match)
        {
            _match = match;
        }

        public bool Match(T? value)
        {
            return _match(value);
        }
    }
    
    public interface IConstraint<in T>
    {
        bool Match(T? value);
    }
}