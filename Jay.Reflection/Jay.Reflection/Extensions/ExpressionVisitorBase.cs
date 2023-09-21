using System.Linq.Expressions;

namespace Jay.Reflection.Extensions;

public abstract class ExpressionVisitorBase : ExpressionVisitor
{
    protected static readonly IReadOnlyDictionary<ExpressionType, string> _operators;

    static ExpressionVisitorBase()
    {
        _operators = new Dictionary<ExpressionType, string>
        {
            { ExpressionType.Not, "!" },
            { ExpressionType.Equal, "==" },
            { ExpressionType.NotEqual, "!=" },
            { ExpressionType.AndAlso, "&&" },
            { ExpressionType.OrElse, "||" },
            { ExpressionType.LessThan, "<" },
            { ExpressionType.LessThanOrEqual, "<=" },
            { ExpressionType.GreaterThan, ">" },
            { ExpressionType.GreaterThanOrEqual, ">=" }
        };
    }
}