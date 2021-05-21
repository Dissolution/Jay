using System;
using System.Diagnostics;
using System.Linq.Expressions;
using System.Reflection;
using Jay.Debugging;

namespace Jay
{
    public static class ExceptionExtensions
    {
        private static readonly Func<Exception, StackTrace, Exception> _setStackTrace;

        static ExceptionExtensions()
        {
            ParameterExpression target = Expression.Parameter(typeof(Exception));
            ParameterExpression stack = Expression.Parameter(typeof(StackTrace));
            Type traceFormatType = typeof(StackTrace).GetNestedType("TraceFormat", 
                                                                    BindingFlags.NonPublic)!;
            MethodInfo toString = typeof(StackTrace).GetMethod("ToString", 
                                                               BindingFlags.NonPublic | BindingFlags.Instance, 
                                                               null, 
                                                               new Type[1] { traceFormatType }, 
                                                               null)!;
            object normalTraceFormat = Enum.GetValues(traceFormatType).GetValue(0)!;
            MethodCallExpression stackTraceString = Expression.Call(stack, 
                                                                    toString, 
                                                                    Expression.Constant(normalTraceFormat, traceFormatType));
            FieldInfo stackTraceStringField = typeof(Exception).GetField("_stackTraceString", 
                                                                         BindingFlags.NonPublic | BindingFlags.Instance)!;
            BinaryExpression assign = Expression.Assign(Expression.Field(target, stackTraceStringField), 
                                                        stackTraceString);
            var lambda = Expression.Lambda<Func<Exception, StackTrace, Exception>>(Expression.Block(assign, target), target, stack);
            _setStackTrace = lambda.Compile();
        }

        public static Exception SetStackTrace(this Exception exception, StackTrace? stackTrace)
        {
            return _setStackTrace(exception, stackTrace ?? new StackTrace(1, true));
        }
    }
}