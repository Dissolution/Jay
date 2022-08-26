using System.Linq.Expressions;
using System.Reflection;

namespace Jay.Expressions;

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

public static class ExpressionExtensions
{
    public static TMember? ExtractMember<TMember>(this Expression expression)
        where TMember : MemberInfo
    {
        return ExtractMembers(expression).OfType<TMember>().FirstOrDefault();
    }

    public static TValue? ExtractValue<TValue>(this Expression expression)
    {
        return ExtractValues(expression)
               .OfType<TValue>()
               .FirstOrDefault();
    }

    public static IEnumerable<object?> ExtractValues(this Expression? expression)
    {
        switch (expression)
        {
            case null:
            {
                break;
            }
            case BinaryExpression binaryExpression:
            {
                foreach (var value in binaryExpression.Left.ExtractValues())
                {
                    yield return value;
                }
                //
                // if (binaryExpression.Method is not null)
                // {
                //     yield return binaryExpression.Method;
                // }

                foreach (var value in binaryExpression.Right.ExtractValues())
                {
                    yield return value;
                }

                break;
            }
            case BlockExpression blockExpression:
            {
                foreach (var value in blockExpression.Expressions
                                                     .SelectMany(expr => expr.ExtractValues()))
                {
                    yield return value;
                }

                break;
            }
            case ConditionalExpression conditionalExpression:
            {
                foreach (var value in conditionalExpression.IfTrue.ExtractValues())
                {
                    yield return value;
                }

                foreach (var value in conditionalExpression.IfFalse.ExtractValues())
                {
                    yield return value;
                }

                break;
            }
            case ConstantExpression constantExpression:
            {
                yield return constantExpression.Value;

                break;
            }
            // ReSharper disable once UnusedVariable
            case DebugInfoExpression debugInfoExpression:
            {
                break;
            }
            // ReSharper disable once UnusedVariable
            case DefaultExpression defaultExpression:
            {
                break;
            }
            case DynamicExpression dynamicExpression:
            {
                foreach (var value in dynamicExpression.Arguments
                                                       .SelectMany(expr => expr.ExtractValues()))
                {
                    yield return value;
                }

                break;
            }
            case GotoExpression gotoExpression:
            {
                foreach (var value in gotoExpression.Value.ExtractValues())
                {
                    yield return value;
                }

                break;
            }
            case IndexExpression indexExpression:
            {
                foreach (var value in indexExpression.Object.ExtractValues())
                {
                    yield return value;
                }

                foreach (var value in indexExpression.Arguments
                                                     .SelectMany(expr => expr.ExtractValues()))
                {
                    yield return value;
                }

                break;
            }
            case InvocationExpression invocationExpression:
            {
                foreach (var value in invocationExpression.Expression.ExtractValues())
                {
                    yield return value;
                }

                foreach (var value in invocationExpression.Arguments
                                                          .SelectMany(expr => expr.ExtractValues()))
                {
                    yield return value;
                }

                break;
            }
            case LabelExpression labelExpression:
            {
                foreach (var value in labelExpression.DefaultValue.ExtractValues())
                {
                    yield return value;
                }

                break;
            }
            case LambdaExpression lambdaExpression:
            {
                foreach (var value in lambdaExpression.Body.ExtractValues())
                {
                    yield return value;
                }

                foreach (var value in lambdaExpression.Parameters
                                                      .SelectMany(expr => expr.ExtractValues()))
                {
                    yield return value;
                }

                break;
            }
            case ListInitExpression listInitExpression:
            {
                foreach (var value in listInitExpression.NewExpression.ExtractValues())
                {
                    yield return value;
                }

                break;
            }
            case LoopExpression loopExpression:
            {
                foreach (var value in loopExpression.Body.ExtractValues())
                {
                    yield return value;
                }

                break;
            }
            case MemberExpression memberExpression:
            {
                foreach (var value in memberExpression.Expression.ExtractValues())
                {
                    yield return value;
                }

                break;
            }
            case MemberInitExpression memberInitExpression:
            {
                foreach (var value in memberInitExpression.NewExpression.ExtractValues())
                {
                    yield return value;
                }

                break;
            }
            case MethodCallExpression methodCallExpression:
            {
                foreach (var value in methodCallExpression.Object.ExtractValues())
                {
                    yield return value;
                }

                foreach (var value in methodCallExpression.Arguments
                                                          .SelectMany(expr => expr.ExtractValues()))
                {
                    yield return value;
                }

                break;
            }
            case NewArrayExpression newArrayExpression:
            {
                foreach (var value in newArrayExpression.Expressions
                                                        .SelectMany(expr => expr.ExtractValues()))
                {
                    yield return value;
                }

                break;
            }
            case NewExpression newExpression:
            {
                foreach (var value in newExpression.Arguments
                                                   .SelectMany(expr => expr.ExtractValues()))
                {
                    yield return value;
                }

                break;
            }
            // ReSharper disable once UnusedVariable
            case ParameterExpression parameterExpression:
            {
                break;
            }
            case RuntimeVariablesExpression runtimeVariablesExpression:
            {
                foreach (var value in runtimeVariablesExpression.Variables
                                                                .SelectMany(expr => expr.ExtractValues()))
                {
                    yield return value;
                }

                break;
            }
            case SwitchExpression switchExpression:
            {
                foreach (var switchCase in switchExpression.Cases)
                {
                    foreach (var value in switchCase.TestValues
                                                    .SelectMany(expr => expr.ExtractValues()))
                    {
                        yield return value;
                    }

                    foreach (var value in switchCase.Body.ExtractValues())
                    {
                        yield return value;
                    }
                }

                foreach (var value in switchExpression.DefaultBody.ExtractValues())
                {
                    yield return value;
                }

                break;
            }
            case TryExpression tryExpression:
            {
                foreach (var value in tryExpression.Body.ExtractValues())
                {
                    yield return value;
                }

                foreach (var handler in tryExpression.Handlers)
                {
                    foreach (var value in handler.Filter.ExtractValues())
                    {
                        yield return value;
                    }

                    foreach (var value in handler.Variable.ExtractValues())
                    {
                        yield return value;
                    }

                    foreach (var value in handler.Body.ExtractValues())
                    {
                        yield return value;
                    }
                }

                foreach (var value in tryExpression.Fault.ExtractValues())
                {
                    yield return value;
                }

                foreach (var value in tryExpression.Finally.ExtractValues())
                {
                    yield return value;
                }

                break;
            }
            case TypeBinaryExpression typeBinaryExpression:
            {
                foreach (var value in typeBinaryExpression.Expression.ExtractValues())
                {
                    yield return value;
                }

                break;
            }
            case UnaryExpression unaryExpression:
            {
                foreach (var value in unaryExpression.Operand.ExtractValues())
                {
                    yield return value;
                }

                break;
            }
            default:
                throw new NotImplementedException();
        }
    }

    public static IEnumerable<MemberInfo> ExtractMembers(this Expression? expression)
    {
        switch (expression)
        {
            case null:
            {
                break;
            }
            case BinaryExpression binaryExpression:
            {
                foreach (var member in binaryExpression.Left.ExtractMembers())
                {
                    yield return member;
                }

                if (binaryExpression.Method is not null)
                {
                    yield return binaryExpression.Method;
                }

                foreach (var member in binaryExpression.Right.ExtractMembers())
                {
                    yield return member;
                }

                break;
            }
            case BlockExpression blockExpression:
            {
                foreach (var member in blockExpression.Expressions
                                                      .SelectMany(expr => expr.ExtractMembers()))
                {
                    yield return member;
                }

                break;
            }
            case ConditionalExpression conditionalExpression:
            {
                foreach (var member in conditionalExpression.IfTrue.ExtractMembers())
                {
                    yield return member;
                }

                foreach (var member in conditionalExpression.IfFalse.ExtractMembers())
                {
                    yield return member;
                }

                break;
            }
            case ConstantExpression constantExpression:
            {
                if (constantExpression.Value is MemberInfo member)
                {
                    yield return member;
                }

                break;
            }
            // ReSharper disable once UnusedVariable
            case DebugInfoExpression debugInfoExpression:
            {
                break;
            }
            // ReSharper disable once UnusedVariable
            case DefaultExpression defaultExpression:
            {
                break;
            }
            case DynamicExpression dynamicExpression:
            {
                foreach (var member in dynamicExpression.Arguments
                                                        .SelectMany(expr => expr.ExtractMembers()))
                {
                    yield return member;
                }

                break;
            }
            case GotoExpression gotoExpression:
            {
                foreach (var member in gotoExpression.Value.ExtractMembers())
                {
                    yield return member;
                }

                break;
            }
            case IndexExpression indexExpression:
            {
                foreach (var member in indexExpression.Object.ExtractMembers())
                {
                    yield return member;
                }

                foreach (var member in indexExpression.Arguments
                                                      .SelectMany(expr => expr.ExtractMembers()))
                {
                    yield return member;
                }

                break;
            }
            case InvocationExpression invocationExpression:
            {
                foreach (var member in invocationExpression.Expression.ExtractMembers())
                {
                    yield return member;
                }

                foreach (var member in invocationExpression.Arguments
                                                           .SelectMany(expr => expr.ExtractMembers()))
                {
                    yield return member;
                }

                break;
            }
            case LabelExpression labelExpression:
            {
                foreach (var member in labelExpression.DefaultValue.ExtractMembers())
                {
                    yield return member;
                }

                break;
            }
            case LambdaExpression lambdaExpression:
            {
                foreach (var member in lambdaExpression.Body.ExtractMembers())
                {
                    yield return member;
                }

                foreach (var member in lambdaExpression.Parameters
                                                       .SelectMany(expr => expr.ExtractMembers()))
                {
                    yield return member;
                }

                break;
            }
            case ListInitExpression listInitExpression:
            {
                foreach (var member in listInitExpression.NewExpression.ExtractMembers())
                {
                    yield return member;
                }

                break;
            }
            case LoopExpression loopExpression:
            {
                foreach (var member in loopExpression.Body.ExtractMembers())
                {
                    yield return member;
                }

                break;
            }
            case MemberExpression memberExpression:
            {
                yield return memberExpression.Member;

                foreach (var member in memberExpression.Expression.ExtractMembers())
                {
                    yield return member;
                }

                break;
            }
            case MemberInitExpression memberInitExpression:
            {
                foreach (var member in memberInitExpression.NewExpression.ExtractMembers())
                {
                    yield return member;
                }

                break;
            }
            case MethodCallExpression methodCallExpression:
            {
                foreach (var member in methodCallExpression.Object.ExtractMembers())
                {
                    yield return member;
                }

                foreach (var member in methodCallExpression.Arguments
                                                           .SelectMany(expr => expr.ExtractMembers()))
                {
                    yield return member;
                }

                yield return methodCallExpression.Method;

                break;
            }
            case NewArrayExpression newArrayExpression:
            {
                foreach (var member in newArrayExpression.Expressions
                                                         .SelectMany(expr => expr.ExtractMembers()))
                {
                    yield return member;
                }

                break;
            }
            case NewExpression newExpression:
            {
                foreach (var member in newExpression.Arguments
                                                    .SelectMany(expr => expr.ExtractMembers()))
                {
                    yield return member;
                }

                if (newExpression.Constructor is not null)
                {
                    yield return newExpression.Constructor;
                }

                if (newExpression.Members != null)
                {
                    foreach (var member in newExpression.Members)
                    {
                        yield return member;
                    }
                }

                break;
            }
            // ReSharper disable once UnusedVariable
            case ParameterExpression parameterExpression:
            {
                break;
            }
            case RuntimeVariablesExpression runtimeVariablesExpression:
            {
                foreach (var member in runtimeVariablesExpression.Variables
                                                                 .SelectMany(expr => expr.ExtractMembers()))
                {
                    yield return member;
                }

                break;
            }
            case SwitchExpression switchExpression:
            {
                foreach (var switchCase in switchExpression.Cases)
                {
                    foreach (var member in switchCase.TestValues
                                                     .SelectMany(expr => expr.ExtractMembers()))
                    {
                        yield return member;
                    }

                    foreach (var member in switchCase.Body.ExtractMembers())
                    {
                        yield return member;
                    }
                }

                foreach (var member in switchExpression.DefaultBody.ExtractMembers())
                {
                    yield return member;
                }

                break;
            }
            case TryExpression tryExpression:
            {
                foreach (var member in tryExpression.Body.ExtractMembers())
                {
                    yield return member;
                }

                foreach (var handler in tryExpression.Handlers)
                {
                    foreach (var member in handler.Filter.ExtractMembers())
                    {
                        yield return member;
                    }

                    foreach (var member in handler.Variable.ExtractMembers())
                    {
                        yield return member;
                    }

                    foreach (var member in handler.Body.ExtractMembers())
                    {
                        yield return member;
                    }
                }

                foreach (var member in tryExpression.Fault.ExtractMembers())
                {
                    yield return member;
                }

                foreach (var member in tryExpression.Finally.ExtractMembers())
                {
                    yield return member;
                }

                break;
            }
            case TypeBinaryExpression typeBinaryExpression:
            {
                foreach (var member in typeBinaryExpression.Expression.ExtractMembers())
                {
                    yield return member;
                }

                break;
            }
            case UnaryExpression unaryExpression:
            {
                if (unaryExpression.Method is not null)
                {
                    yield return unaryExpression.Method;
                }

                foreach (var member in unaryExpression.Operand.ExtractMembers())
                {
                    yield return member;
                }

                break;
            }
            default:
                throw new NotImplementedException();
        }
    }

    public static IEnumerable<TExpression> ExtractExpressions<TExpression>(this Expression? expression)
        where TExpression : Expression
    {
        if (expression is TExpression tExpression)
        {
            yield return tExpression;
        }

        switch (expression)
        {
            case null:
                yield break;
            case BinaryExpression binaryExpression:
            {
                foreach (var expr in binaryExpression.Left
                                                     .ExtractExpressions<TExpression>())
                {
                    yield return expr;
                }

                foreach (var expr in binaryExpression.Right
                                                     .ExtractExpressions<TExpression>())
                {
                    yield return expr;
                }

                yield break;
            }
            case BlockExpression blockExpression:
            {
                foreach (var expr in blockExpression.Expressions
                                                    .SelectMany(expr => expr.ExtractExpressions<TExpression>()))
                {
                    yield return expr;
                }

                yield break;
            }
            case ConditionalExpression conditionalExpression:
            {
                foreach (var expr in conditionalExpression.IfTrue
                                                          .ExtractExpressions<TExpression>())
                {
                    yield return expr;
                }

                foreach (var expr in conditionalExpression.IfFalse
                                                          .ExtractExpressions<TExpression>())
                {
                    yield return expr;
                }

                yield break;
            }
            case DynamicExpression dynamicExpression:
            {
                foreach (var expr in dynamicExpression.Arguments
                                                      .SelectMany(expr => expr.ExtractExpressions<TExpression>()))
                {
                    yield return expr;
                }

                yield break;
            }
            case GotoExpression gotoExpression:
            {
                foreach (var expr in gotoExpression.Value
                                                   .ExtractExpressions<TExpression>())
                {
                    yield return expr;
                }

                yield break;
            }
            case IndexExpression indexExpression:
            {
                foreach (var expr in indexExpression.Object
                                                    .ExtractExpressions<TExpression>())
                {
                    yield return expr;
                }

                foreach (var expr in indexExpression.Arguments
                                                    .SelectMany(expr => expr.ExtractExpressions<TExpression>()))
                {
                    yield return expr;
                }

                yield break;
            }
            case InvocationExpression invocationExpression:
            {
                foreach (var expr in invocationExpression.Expression
                                                         .ExtractExpressions<TExpression>())
                {
                    yield return expr;
                }

                foreach (var expr in invocationExpression.Arguments
                                                         .SelectMany(expr => expr.ExtractExpressions<TExpression>()))
                {
                    yield return expr;
                }

                yield break;
            }
            case LabelExpression labelExpression:
            {
                foreach (var expr in labelExpression.DefaultValue
                                                    .ExtractExpressions<TExpression>())
                {
                    yield return expr;
                }

                yield break;
            }
            case LambdaExpression lambdaExpression:
            {
                foreach (var expr in lambdaExpression.Body
                                                     .ExtractExpressions<TExpression>())
                {
                    yield return expr;
                }

                foreach (var expr in lambdaExpression.Parameters
                                                     .SelectMany(expr => expr.ExtractExpressions<TExpression>()))
                {
                    yield return expr;
                }

                yield break;
            }
            case ListInitExpression listInitExpression:
            {
                foreach (var expr in listInitExpression.NewExpression
                                                       .ExtractExpressions<TExpression>())
                {
                    yield return expr;
                }

                yield break;
            }
            case LoopExpression loopExpression:
            {
                foreach (var expr in loopExpression.Body
                                                   .ExtractExpressions<TExpression>())
                {
                    yield return expr;
                }

                yield break;
            }
            case MemberExpression memberExpression:
            {
                foreach (var expr in memberExpression.Expression
                                                     .ExtractExpressions<TExpression>())
                {
                    yield return expr;
                }

                yield break;
            }
            case MemberInitExpression memberInitExpression:
            {
                foreach (var expr in memberInitExpression.NewExpression
                                                         .ExtractExpressions<TExpression>())
                {
                    yield return expr;
                }

                yield break;
            }
            case MethodCallExpression methodCallExpression:
            {
                foreach (var expr in methodCallExpression.Object
                                                         .ExtractExpressions<TExpression>())
                {
                    yield return expr;
                }

                foreach (var expr in methodCallExpression.Arguments
                                                         .SelectMany(expr => expr.ExtractExpressions<TExpression>()))
                {
                    yield return expr;
                }

                yield break;
            }
            case NewArrayExpression newArrayExpression:
            {
                foreach (var expr in newArrayExpression.Expressions
                                                       .SelectMany(expr => expr.ExtractExpressions<TExpression>()))
                {
                    yield return expr;
                }

                yield break;
            }
            case NewExpression newExpression:
            {
                foreach (var expr in newExpression.Arguments
                                                  .SelectMany(expr => expr.ExtractExpressions<TExpression>()))
                {
                    yield return expr;
                }

                yield break;
            }
            case RuntimeVariablesExpression runtimeVariablesExpression:
            {
                foreach (var expr in runtimeVariablesExpression.Variables
                                                               .SelectMany(expr =>
                                                                   expr.ExtractExpressions<TExpression>()))
                {
                    yield return expr;
                }

                yield break;
            }
            case SwitchExpression switchExpression:
            {
                foreach (var switchCase in switchExpression.Cases)
                {
                    foreach (var expr in switchCase.TestValues
                                                   .SelectMany(expr => expr.ExtractExpressions<TExpression>()))
                    {
                        yield return expr;
                    }

                    foreach (var expr in switchCase.Body
                                                   .ExtractExpressions<TExpression>())
                    {
                        yield return expr;
                    }
                }

                foreach (var expr in switchExpression.DefaultBody
                                                     .ExtractExpressions<TExpression>())
                {
                    yield return expr;
                }

                yield break;
            }
            case TryExpression tryExpression:
            {
                foreach (var expr in tryExpression.Body
                                                  .ExtractExpressions<TExpression>())
                {
                    yield return expr;
                }

                foreach (var handler in tryExpression.Handlers)
                {
                    foreach (var expr in handler.Filter
                                                .ExtractExpressions<TExpression>())
                    {
                        yield return expr;
                    }

                    foreach (var expr in handler.Variable
                                                .ExtractExpressions<TExpression>())
                    {
                        yield return expr;
                    }

                    foreach (var expr in handler.Body
                                                .ExtractExpressions<TExpression>())
                    {
                        yield return expr;
                    }
                }

                foreach (var expr in tryExpression.Fault
                                                  .ExtractExpressions<TExpression>())
                {
                    yield return expr;
                }

                foreach (var expr in tryExpression.Finally
                                                  .ExtractExpressions<TExpression>())
                {
                    yield return expr;
                }

                yield break;
            }
            case TypeBinaryExpression typeBinaryExpression:
            {
                foreach (var expr in typeBinaryExpression.Expression
                                                         .ExtractExpressions<TExpression>())
                {
                    yield return expr;
                }

                yield break;
            }
            case UnaryExpression unaryExpression:
            {
                foreach (var expr in unaryExpression.Operand
                                                    .ExtractExpressions<TExpression>())
                {
                    yield return expr;
                }

                yield break;
            }
        }
    }
}