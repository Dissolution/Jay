/*using System;
using System.Collections.Concurrent;
using System.Diagnostics;
using System.Linq.Expressions;
using System.Runtime.CompilerServices;
using Jay.Comparison;
using Jay.Debugging.Dumping;
using Jay.Expressions;
using Jay.Reflection.Comparison;
using Jay.Reflection.Runtime;

namespace Jay.Reflection.Operators
{
    public partial class Operators
    {
        private static readonly ConcurrentDictionary<Type[], Operators> _cache;

        static Operators()
        {
            _cache = new ConcurrentDictionary<Type[], Operators>(TypeEqualityComparer.Instance);
        }
    }

    partial class Operators
    {
        private readonly Type[] _types;

        internal readonly 

    }

    internal class Operators<T> : Operators
    {
        
    }
    
    
    public static class Operators<T>
    {
        internal static readonly Type Type = typeof(T);
        private static readonly Lazy<Func<T, T, T>> _orFunc;

        static Operators()
        {
            _orFunc = new Lazy<Func<T, T, T>>(CreateOrFunc);
        }

        private static Func<T, T, T> CreateOrFunc()
        {
            var dynamicMethod = DelegateBuilder.CreateDynamicMethod<Func<T, T, T>>(Dumper.Format($"{Type}_OR_{Type}"));
            var emitter = dynamicMethod.GetEmitter();
            // Unmanaged?
            if (!RuntimeHelpers.IsReferenceOrContainsReferences<T>())
            {
                var size = NotSafe.SizeOf<T>();
                if (size == 1)
                {
                    emitter.LoadArgument(0)
                           .LoadArgument(1)
                           .Or()
                           .Generate(g => g.Conv_U1())
                }
            }
        }
        
        public static T Or(T first, T second) => _orFunc.Value(first, second);
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
}*/