using System.Linq.Expressions;

namespace Jay;

public static class PredicateBuilder
{
    private static class Predicate<T>
    {
        public static readonly Expression<Func<T, bool>> True = (item => true);

        public static readonly Expression<Func<T, bool>> False = (item => false);
    }

    private sealed class ExpressionReplacer : ExpressionVisitor
    {
        private readonly Expression _original;
        private readonly Expression _replacement;

        public ExpressionReplacer(Expression original, Expression replacement)
        {
            _original = original;
            _replacement = replacement;
        }

        public override Expression Visit(Expression? node)
        {
            if (Equals(_original, node))
                return _replacement;
            return base.Visit(node)!;
        }
    }

    private sealed class ParameterReplacer : ExpressionVisitor
    {
        private readonly ParameterExpression _original;
        private readonly ParameterExpression _replacement;

        public ParameterReplacer(ParameterExpression original, ParameterExpression replacement)
        {
            _original = original;
            _replacement = replacement;
        }

        protected override Expression VisitParameter(ParameterExpression node)
        {
            if (Equals(_original, node))
            {
                node = _replacement;
            }
            return base.VisitParameter(node);
        }
    }



    private static Expression Replace(this Expression expression, Expression source, Expression target)
    {
        return new ExpressionReplacer(source, target).Visit(expression);
    }

    private static Expression ReplaceParameter(this Expression expression,
                                               ParameterExpression original,
                                               ParameterExpression replacement)
    {
        return new ParameterReplacer(original, replacement).Visit(expression);
    }
    /// <summary>
    /// Creates a predicate that evaluates to true.
    /// </summary>
    public static Expression<Func<T, bool>> True<T>() => Predicate<T>.True;

    /// <summary>
    /// Creates a predicate that evaluates to false.
    /// </summary>
    public static Expression<Func<T, bool>> False<T>() => Predicate<T>.False;

    /// <summary>
    /// Creates a predicate expression from the specified lambda expression.
    /// </summary>
    public static Expression<Func<T, bool>> Create<T>(Expression<Func<T, bool>> predicate) => predicate;

    public static Expression<Func<T, bool>> Create<T>(Func<T, bool> predicate) => (t => predicate(t));

    public static Expression<Func<T, bool>> And<T>(this Expression<Func<T, bool>> left, 
                                                   Expression<Func<T, bool>> right)
    {
        if (Equals(left, right)) return left;
        if (Equals(left, Predicate<T>.True)) return right;
        if (Equals(right, Predicate<T>.True)) return left;
        if (Equals(left, Predicate<T>.False) || Equals(right, Predicate<T>.False))
            return Predicate<T>.False;

        var body = Expression.AndAlso(left.Body, 
                                      right.Body.ReplaceParameter(right.Parameters[0], left.Parameters[0]));
        return Expression.Lambda<Func<T, bool>>(body, left.Parameters);
    }

    public static Expression<Func<T, bool>> Or<T>(this Expression<Func<T, bool>> left, Expression<Func<T, bool>> right)
    {
        if (Equals(left, right)) return left;
        if (Equals(left, Predicate<T>.False)) return right;
        if (Equals(right, Predicate<T>.False)) return left;
        if (Equals(left, Predicate<T>.True) ||
            Equals(right, Predicate<T>.True))
        {
            return Predicate<T>.True;
        }
        
        var body = Expression.OrElse(left.Body, 
                                     right.Body.ReplaceParameter(right.Parameters[0], left.Parameters[0]));
        return Expression.Lambda<Func<T, bool>>(body, left.Parameters);
    }
}