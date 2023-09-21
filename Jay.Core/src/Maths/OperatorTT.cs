using System.Linq.Expressions;

namespace Jay.Maths;

/// <summary>
/// Provides standard operators (such as addition) that operate over operands of
/// different types. For operators, the return type is assumed to match the first
/// operand.
/// </summary>
/// <seealso cref="Operator&lt;T&gt;"/>
/// <seealso cref="Operator"/>
public static class Operator<TValue, TResult>
{
    /// <summary>
    /// Returns a delegate to convert a value between two types; this delegate will throw
    /// an InvalidOperationException if the type T does not provide a suitable cast, or for
    /// Nullable&lt;TInner&gt; if TInner does not provide this cast.
    /// </summary>
    public static Func<TValue, TResult> Convert { get; }

    static Operator()
    {
        Convert = ExpressionUtil.CreateExpression<TValue, TResult>(body => Expression.Convert(body, typeof(TResult)));
        Add = ExpressionUtil.CreateExpression<TResult, TValue, TResult>(Expression.Add, true);
        Subtract = ExpressionUtil.CreateExpression<TResult, TValue, TResult>(Expression.Subtract, true);
        Multiply = ExpressionUtil.CreateExpression<TResult, TValue, TResult>(Expression.Multiply, true);
        Divide = ExpressionUtil.CreateExpression<TResult, TValue, TResult>(Expression.Divide, true);
    }

    /// <summary>
    /// Returns a delegate to evaluate binary addition (+) for the given types; this delegate will throw
    /// an InvalidOperationException if the type T does not provide this operator, or for
    /// Nullable&lt;TInner&gt; if TInner does not provide this operator.
    /// </summary>
    public static Func<TResult, TValue, TResult> Add { get; }

    /// <summary>
    /// Returns a delegate to evaluate binary subtraction (-) for the given types; this delegate will throw
    /// an InvalidOperationException if the type T does not provide this operator, or for
    /// Nullable&lt;TInner&gt; if TInner does not provide this operator.
    /// </summary>
    public static Func<TResult, TValue, TResult> Subtract { get; }

    /// <summary>
    /// Returns a delegate to evaluate binary multiplication (*) for the given types; this delegate will throw
    /// an InvalidOperationException if the type T does not provide this operator, or for
    /// Nullable&lt;TInner&gt; if TInner does not provide this operator.
    /// </summary>
    public static Func<TResult, TValue, TResult> Multiply { get; }

    /// <summary>
    /// Returns a delegate to evaluate binary division (/) for the given types; this delegate will throw
    /// an InvalidOperationException if the type T does not provide this operator, or for
    /// Nullable&lt;TInner&gt; if TInner does not provide this operator.
    /// </summary>
    public static Func<TResult, TValue, TResult> Divide { get; }
}