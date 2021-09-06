using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq.Expressions;
using System.Reflection;

// All extensions live at Jay.
// ReSharper disable once CheckNamespace
namespace Jay
{
    public static class ExpressionExtensions
    {
        internal static IEnumerable<Expression?> EnumerateExpressions(this Expression? expression)
        {
            yield return expression;
            switch (expression)
            {
                case BinaryExpression binaryExpression:
                {
                    foreach (var expr in binaryExpression.Left.EnumerateExpressions())
                        yield return expr;
                    foreach (var expr in binaryExpression.Right.EnumerateExpressions())
                        yield return expr;
                    foreach (var expr in binaryExpression.Conversion.EnumerateExpressions())
                        yield return expr;
                    yield break;
                }
                case BlockExpression blockExpression:
                {
                    foreach (var expr in blockExpression.Variables)
                    foreach (var subExpr in expr.EnumerateExpressions())
                        yield return subExpr;
                    foreach (var expr in blockExpression.Expressions)
                    foreach (var subExpr in expr.EnumerateExpressions())
                        yield return subExpr;
                    foreach (var subExpr in blockExpression.Result.EnumerateExpressions())
                        yield return subExpr;
                    yield break;
                }
                case ConditionalExpression conditionalExpression:
                {
                    foreach (var subExpr in conditionalExpression.Test.EnumerateExpressions())
                        yield return subExpr;
                    foreach (var subExpr in conditionalExpression.IfTrue.EnumerateExpressions())
                        yield return subExpr;
                    foreach (var subExpr in conditionalExpression.IfFalse.EnumerateExpressions())
                        yield return subExpr;
                    yield break;
                }
                case ConstantExpression constantExpression:
                {
                    yield break;
                }
                case DebugInfoExpression debugInfoExpression:
                {
                    yield break;
                }
                case DefaultExpression defaultExpression:
                {
                    yield break;
                }
                case DynamicExpression dynamicExpression:
                {
                    // Base
                    foreach (var expr in dynamicExpression.Arguments)
                    foreach (var subExpr in expr.EnumerateExpressions())
                        yield return subExpr;
                    yield break;
                }
                case GotoExpression gotoExpression:
                {
                    yield break;
                }
                case IndexExpression indexExpression:
                {
                    // Base
                    foreach (var expr in indexExpression.Object.EnumerateExpressions())
                        yield return expr;
                    foreach (var expr in indexExpression.Arguments)
                    foreach (var subExpr in expr.EnumerateExpressions())
                        yield return subExpr;
                    yield break;
                }
                case InvocationExpression invocationExpression:
                {
                    foreach (var expr in invocationExpression.Arguments)
                    foreach (var subExpr in expr.EnumerateExpressions())
                        yield return subExpr;
                    foreach (var expr in invocationExpression.Expression.EnumerateExpressions())
                        yield return expr;
                    yield break;
                }
                case LabelExpression labelExpression:
                {
                    foreach (var expr in labelExpression.DefaultValue.EnumerateExpressions())
                        yield return expr;
                    yield break;
                }
                case LambdaExpression lambdaExpression:
                {
                    foreach (var expr in lambdaExpression.Parameters)
                    foreach (var subExpr in expr.EnumerateExpressions())
                        yield return subExpr;
                    foreach (var expr in lambdaExpression.Body.EnumerateExpressions())
                        yield return expr;
                    yield break;
                }
                case ListInitExpression listInitExpression:
                {
                    foreach (var expr in listInitExpression.NewExpression.EnumerateExpressions())
                        yield return expr;
                    yield break;
                }
                case LoopExpression loopExpression:
                {
                    foreach (var expr in loopExpression.Body.EnumerateExpressions())
                        yield return expr;
                    yield break;
                }
                case MemberExpression memberExpression:
                {
                    foreach (var expr in memberExpression.Expression.EnumerateExpressions())
                        yield return expr;
                    yield break;
                }
                case MemberInitExpression memberInitExpression:
                {
                    foreach (var expr in memberInitExpression.NewExpression.EnumerateExpressions())
                        yield return expr;
                    yield break;
                }
                case MethodCallExpression methodCallExpression:
                {
                    foreach (var expr in methodCallExpression.Arguments)
                    foreach (var subExpr in expr.EnumerateExpressions())
                        yield return subExpr;
                    foreach (var expr in methodCallExpression.Object.EnumerateExpressions())
                        yield return expr;
                    yield break;
                }
                case NewArrayExpression newArrayExpression:
                {
                    foreach (var expr in newArrayExpression.Expressions)
                    foreach (var subExpr in expr.EnumerateExpressions())
                        yield return subExpr;
                    yield break;
                }
                case NewExpression newExpression:
                {
                    foreach (var expr in newExpression.Arguments)
                    foreach (var subExpr in expr.EnumerateExpressions())    
                        yield return subExpr;
                    yield break;
                }
                case ParameterExpression parameterExpression:
                {
                    yield break;
                }
                case RuntimeVariablesExpression runtimeVariablesExpression:
                {
                    foreach (var expr in runtimeVariablesExpression.Variables)
                    foreach (var subExpr in expr.EnumerateExpressions())    
                        yield return subExpr;
                    yield break;
                }
                case SwitchExpression switchExpression:
                {
                    foreach (var expr in switchExpression.SwitchValue.EnumerateExpressions())
                        yield return expr;
                    foreach (var expr in switchExpression.DefaultBody.EnumerateExpressions())
                        yield return expr;
                    yield break;
                }
                case TryExpression tryExpression:
                {
                    foreach (var expr in tryExpression.Body.EnumerateExpressions())
                        yield return expr;
                    foreach (var expr in tryExpression.Fault.EnumerateExpressions())
                        yield return expr;
                    foreach (var expr in tryExpression.Finally.EnumerateExpressions())
                        yield return expr;
                    yield break;
                }
                case TypeBinaryExpression typeBinaryExpression:
                {
                    foreach (var expr in typeBinaryExpression.Expression.EnumerateExpressions())
                        yield return expr;
                    yield break;
                }
                case UnaryExpression unaryExpression:
                {
                    foreach (var expr in unaryExpression.Operand.EnumerateExpressions())
                        yield return expr;
                    yield break;
                }
                default:
                    yield break;
            }
        }
        
        
        public static TDelegate? Compile<TDelegate>(this LambdaExpression? lambdaExpression)
            where TDelegate : Delegate
        {
            return lambdaExpression?.Compile() as TDelegate;
        }
        
        public static TMember? GetMember<TMember>(this Expression? expression)
            where TMember : MemberInfo
            => GetMember(expression) as TMember;
        
        public static MemberInfo? GetMember(this Expression? expression)
        {
            return expression switch
            {
                BinaryExpression binaryExpression => GetMember(binaryExpression),
                BlockExpression blockExpression => GetMember(blockExpression),
                ConditionalExpression conditionalExpression => GetMember(conditionalExpression),
                ConstantExpression constantExpression => GetMember(constantExpression),
                DebugInfoExpression debugInfoExpression => GetMember(debugInfoExpression),
                DefaultExpression defaultExpression => GetMember(defaultExpression),
                DynamicExpression dynamicExpression => GetMember(dynamicExpression),
                GotoExpression gotoExpression => GetMember(gotoExpression),
                IndexExpression indexExpression => GetMember(indexExpression),
                InvocationExpression invocationExpression => GetMember(invocationExpression),
                LabelExpression labelExpression => GetMember(labelExpression),
                LambdaExpression lambdaExpression => GetMember(lambdaExpression),
                ListInitExpression listInitExpression => GetMember(listInitExpression),
                LoopExpression loopExpression => GetMember(loopExpression),
                MemberExpression memberExpression => GetMember(memberExpression),
                MemberInitExpression memberInitExpression => GetMember(memberInitExpression),
                MethodCallExpression methodCallExpression => GetMember(methodCallExpression),
                NewArrayExpression newArrayExpression => GetMember(newArrayExpression),
                NewExpression newExpression => GetMember(newExpression),
                ParameterExpression parameterExpression => GetMember(parameterExpression),
                RuntimeVariablesExpression runtimeVariablesExpression => GetMember(runtimeVariablesExpression),
                SwitchExpression switchExpression => GetMember(switchExpression),
                TryExpression tryExpression => GetMember(tryExpression),
                TypeBinaryExpression typeBinaryExpression => GetMember(typeBinaryExpression),
                UnaryExpression unaryExpression => GetMember(unaryExpression),
                _ => null
            };
        }

        public static MemberInfo? GetMember(this BinaryExpression? binaryExpression)
        {
            return GetMember(binaryExpression?.Left) ??
                   GetMember(binaryExpression?.Right);
        }
        
        public static MemberInfo? GetMember(this BlockExpression? blockExpression)
        {
            if (blockExpression is null) return null;
            MemberInfo? member = GetMember(blockExpression.Result);
            if (member is null)
            {
                foreach (var expr in blockExpression.Expressions)
                {
                    member = GetMember(expr);
                    if (member != null)
                        break;
                }
            }
            return member;
        }
        
        public static MemberInfo? GetMember(this ConditionalExpression? conditionalExpression)
        {
            if (conditionalExpression is null) return null;
            return GetMember(conditionalExpression.Test) ??
                   GetMember(conditionalExpression.IfTrue) ??
                   GetMember(conditionalExpression.IfFalse);
        }
        
        public static MemberInfo? GetMember(this ConstantExpression? constantExpression)
        {
            return constantExpression?.Value as MemberInfo;
        }
        
        public static MemberInfo? GetMember(this DebugInfoExpression? debugInfoExpression)
        {
            return null;
        }
        
        public static MemberInfo? GetMember(this DefaultExpression? defaultExpression)
        {
            return defaultExpression?.Type;
        }
        
        public static MemberInfo? GetMember(this DynamicExpression? dynamicExpression)
        {
            return null;
        }
        
        public static MemberInfo? GetMember(this GotoExpression? gotoExpression)
        {
            return null;
        }
        
        public static MemberInfo? GetMember(this IndexExpression? indexExpression)
        {
            return indexExpression?.Indexer;
        }
        
        public static MemberInfo? GetMember(this InvocationExpression? invocationExpression)
        {
            return GetMember(invocationExpression?.Expression);
        }
        
        public static MemberInfo? GetMember(this LabelExpression? labelExpression)
        {
            return null;
        }
        
        public static MemberInfo? GetMember(this LambdaExpression? lambdaExpression)
        {
            return GetMember(lambdaExpression?.Body);
        }
        
        public static MemberInfo? GetMember(this ListInitExpression? listInitExpression)
        {
            return GetMember(listInitExpression?.NewExpression);
        }
        
        public static MemberInfo? GetMember(this LoopExpression? loopExpression)
        {
            return GetMember(loopExpression?.Body);
        }
        
        public static MemberInfo? GetMember(this MemberExpression? memberExpression)
        {
            return memberExpression?.Member;
        }
        
        public static MemberInfo? GetMember(this MemberInitExpression? memberInitExpression)
        {
            return GetMember(memberInitExpression?.NewExpression);
        }
        
        public static MemberInfo? GetMember(this MethodCallExpression? methodCallExpression)
        {
            return methodCallExpression?.Method;
        }
        
        public static MemberInfo? GetMember(this NewArrayExpression? newArrayExpression)
        {
            return null;
        }
        
        public static MemberInfo? GetMember(this NewExpression? newExpression)
        {
            return newExpression?.Constructor;
        }
        
        public static MemberInfo? GetMember(this ParameterExpression? parameterExpression)
        {
            return parameterExpression?.Type;
        }
        
        public static MemberInfo? GetMember(this RuntimeVariablesExpression? runtimeVariablesExpression)
        {
            return null;
        }
        
        public static MemberInfo? GetMember(this SwitchExpression? switchExpression)
        {
            return null;
        }
        
        public static MemberInfo? GetMember(this TryExpression? tryExpression)
        {
            if (tryExpression is null) return null;
            return GetMember(tryExpression.Body) ??
                   GetMember(tryExpression.Fault) ??
                   GetMember(tryExpression.Finally);
        }
        
        public static MemberInfo? GetMember(this TypeBinaryExpression? typeBinaryExpression)
        {
            return typeBinaryExpression?.Type;
        }
        
        public static MemberInfo? GetMember(this UnaryExpression? unaryExpression)
        {
            return GetMember(unaryExpression?.Operand);
        }
    }
}