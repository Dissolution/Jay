using System.Linq.Expressions;

namespace Jay.Expressions;

public static class Expressions
{
    public static Func<TValue, TResult> CompileUnaryExpression<TValue, TResult>(
        Func<Expression, UnaryExpression> body)
    {
        ParameterExpression arg = Expression.Parameter(typeof(TValue), "arg");
        try
        {
            var lambda = Expression.Lambda<Func<TValue, TResult>>(body(arg), arg);
            return lambda.Compile();
        }
        catch (Exception ex)
        {
            // Return a function that throws the exception
            return _ => throw new InvalidOperationException(ex.Message);
        }
    }

    public static Func<T1, T2, TResult> CompileBinaryExpression<T1, T2, TResult>(Func<Expression, Expression, BinaryExpression> body)
    {
        ParameterExpression arg1 = Expression.Parameter(typeof(T1), "arg1");
        ParameterExpression arg2 = Expression.Parameter(typeof(T2), "arg2");
        try
        {
            var lambda = Expression.Lambda<Func<T1, T2, TResult>>(body(arg1, arg2), arg1, arg2);
            return lambda.Compile();
        }
        catch (Exception ex)
        {
            // Return a function that throws the exception
            return (_,_) => throw new InvalidOperationException(ex.Message);
        }
    }
}