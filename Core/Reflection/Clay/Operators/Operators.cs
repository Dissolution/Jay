using System;
using System.Collections.Concurrent;
using System.Diagnostics;
using System.Linq.Expressions;
using System.Runtime.CompilerServices;
using Jay.Debugging.Dumping;
using Jay.Expressions;
using Jay.Reflection.Runtime;

namespace Jay.Reflection.Operators
{
    public static class Operators
    {
        
    }

    public static class Operators<T1>
    {
        
    }

    public static class Operators<T1, T2>
    {
        
    }

    public static class Operators<T1, T2, TReturn>
    {
        private static readonly Lazy<Func<T1, T2, TReturn>> _add;

        static Operators()
        {
            _add = new Lazy<Func<T1, T2, TReturn>>(() => CreateFunction(Expression.Add));
        }

        private static Func<T1, T2, TReturn> CreateFunction(Func<Expression, Expression, BinaryExpression> expressionMethod)
        {
            ParameterExpression? left = Expression.Parameter(typeof(T1), "left");
            ParameterExpression? right = Expression.Parameter(typeof(T2), "right");

            BinaryExpression? binaryExpression;
            try
            {
                binaryExpression = expressionMethod(left, right);
            }
            catch (Exception ex)
            {
                throw new NotImplementedException();
            }

            Expression? body = binaryExpression;
            try
            {
                var lambda = Expression.Lambda(body, left, right);
                var func = lambda.Compile<Func<T1, T2, TReturn>>();
                if (func != null)
                    return func;
            }
            catch (Exception ex)
            {
                throw new NotImplementedException();
            }

            
            throw new NotImplementedException();
        }

    }
}