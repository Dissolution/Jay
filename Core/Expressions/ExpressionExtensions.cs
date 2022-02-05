using System.Linq.Expressions;
using System.Reflection;

namespace Jay;

public static class ExpressionExtensions
{
    public static TMember? ExtractMember<TMember>(this Expression expression)
        where TMember : MemberInfo
    {
        return ExtractMembers(expression).OfType<TMember>().FirstOrDefault();
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
}
