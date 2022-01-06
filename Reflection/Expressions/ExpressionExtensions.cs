using System.Linq.Expressions;
using System.Reflection;

namespace Jay.Reflection.Expressions;

public static class ExpressionExtensions
{
    public static IEnumerable<TMember> GetMembers<TMember>(this Expression? expression)
        where TMember : MemberInfo
    {
        switch (expression)
        {
            case null:
            {
                break;
            }
            case BinaryExpression binaryExpression:
            {
                foreach (var member in binaryExpression.Left.GetMembers<TMember>())
                {
                    yield return member;
                }

                if (binaryExpression.Method is TMember method)
                {
                    yield return method;
                }

                foreach (var member in binaryExpression.Right.GetMembers<TMember>())
                {
                    yield return member;
                }

                break;
            }
            case BlockExpression blockExpression:
            {
                foreach (var expr in blockExpression.Expressions)
                {
                    foreach (var member in expr.GetMembers<TMember>())
                    {
                        yield return member;
                    }
                }

                break;
            }
            case ConditionalExpression conditionalExpression:
            {
                foreach (var member in conditionalExpression.IfTrue.GetMembers<TMember>())
                {
                    yield return member;
                }

                foreach (var member in conditionalExpression.IfFalse.GetMembers<TMember>())
                {
                    yield return member;
                }

                break;
            }
            case ConstantExpression constantExpression:
            {
                if (constantExpression.Value is TMember member)
                {
                    yield return member;
                }

                break;
            }
            case DebugInfoExpression debugInfoExpression:
            {
                break;
            }
            case DefaultExpression defaultExpression:
            {
                break;
            }
            case DynamicExpression dynamicExpression:
            {
                foreach(var expr in dynamicExpression.Arguments)
                {
                    foreach (var member in expr.GetMembers<TMember>())
                    {
                        yield return member;
                    }
                }
                break;
            }
            case GotoExpression gotoExpression:
            {
                foreach (var member in gotoExpression.Value.GetMembers<TMember>())
                {
                    yield return member;
                }
                break;
            }
            case IndexExpression indexExpression:
            {
                foreach (var member in indexExpression.Object.GetMembers<TMember>())
                {
                    yield return member;
                }
                foreach (var expr in indexExpression.Arguments)
                {
                    foreach (var member in expr.GetMembers<TMember>())
                    {
                        yield return member;
                    }
                }
                break;
            }
            case InvocationExpression invocationExpression:
            {
                foreach (var member in invocationExpression.Expression.GetMembers<TMember>())
                {
                    yield return member;
                }
                foreach (var expr in invocationExpression.Arguments)
                {
                    foreach (var member in expr.GetMembers<TMember>())
                    {
                        yield return member;
                    }
                }
                break;
            }
            case LabelExpression labelExpression:
            {
                foreach (var member in labelExpression.DefaultValue.GetMembers<TMember>())
                {
                    yield return member;
                }
                break;
            }
            case LambdaExpression lambdaExpression:
            {
                foreach (var member in lambdaExpression.Body.GetMembers<TMember>())
                {
                    yield return member;
                }
                foreach (var expr in lambdaExpression.Parameters)
                {
                    foreach (var member in expr.GetMembers<TMember>())
                    {
                        yield return member;
                    }
                }
                break;
            }
            case ListInitExpression listInitExpression:
            {
                foreach (var member in listInitExpression.NewExpression.GetMembers<TMember>())
                {
                    yield return member;
                }
                break;
            }
            case LoopExpression loopExpression:
            {
                foreach (var member in loopExpression.Body.GetMembers<TMember>())
                {
                    yield return member;
                }
                break;
            }
            case MemberExpression memberExpression:
            {
                if (memberExpression.Member is TMember membr)
                {
                    yield return membr;
                }
                foreach (var member in memberExpression.Expression.GetMembers<TMember>())
                {
                    yield return member;
                }
                break;
            }
            case MemberInitExpression memberInitExpression:
            {
                foreach (var member in memberInitExpression.NewExpression.GetMembers<TMember>())
                {
                    yield return member;
                }
                break;
            }
            case MethodCallExpression methodCallExpression:
            {
                foreach (var member in methodCallExpression.Object.GetMembers<TMember>())
                {
                    yield return member;
                }
                if (methodCallExpression.Method is TMember method)
                {
                    yield return method;
                }
                foreach (var expr in methodCallExpression.Arguments)
                {
                    foreach (var member in expr.GetMembers<TMember>())
                    {
                        yield return member;
                    }
                }
                break;
            }
            case NewArrayExpression newArrayExpression:
            {
                foreach (var expr in newArrayExpression.Expressions)
                {
                    foreach (var member in expr.GetMembers<TMember>())
                    {
                        yield return member;
                    }
                }
                break;
            }
            case NewExpression newExpression:
            {
                if (newExpression.Constructor is TMember ctor)
                {
                    yield return ctor;
                }

                if (newExpression.Members != null)
                {
                    foreach (var member in newExpression.Members.OfType<TMember>())
                    {
                        yield return member;
                    }
                }
                foreach (var expr in newExpression.Arguments)
                {
                    foreach (var member in expr.GetMembers<TMember>())
                    {
                        yield return member;
                    }
                }
                break;
            }
            case ParameterExpression parameterExpression:
            {
                break;
            }
            case RuntimeVariablesExpression runtimeVariablesExpression:
            {
                foreach (var expr in runtimeVariablesExpression.Variables)
                {
                    foreach (var member in expr.GetMembers<TMember>())
                    {
                        yield return member;
                    }
                }
                break;
            }
            case SwitchExpression switchExpression:
            {
                foreach (var switchCase in switchExpression.Cases)
                {
                    foreach (var expr in switchCase.TestValues)
                    {
                        foreach (var member in expr.GetMembers<TMember>())
                        {
                            yield return member;
                        }
                    }
                    foreach (var member in switchCase.Body.GetMembers<TMember>())
                    {
                        yield return member;
                    }
                }
                foreach (var member in switchExpression.DefaultBody.GetMembers<TMember>())
                {
                    yield return member;
                }
                break;
            }
            case TryExpression tryExpression:
            {
                foreach (var member in tryExpression.Body.GetMembers<TMember>())
                {
                    yield return member;
                }

                foreach (var handler in tryExpression.Handlers)
                {
                    foreach (var member in handler.Filter.GetMembers<TMember>())
                    {
                        yield return member;
                    }
                    foreach (var member in handler.Variable.GetMembers<TMember>())
                    {
                        yield return member;
                    }
                    foreach (var member in handler.Body.GetMembers<TMember>())
                    {
                        yield return member;
                    }
                }

                foreach (var member in tryExpression.Fault.GetMembers<TMember>())
                {
                    yield return member;
                }
                foreach (var member in tryExpression.Finally.GetMembers<TMember>())
                {
                    yield return member;
                }
                break;
            }
            case TypeBinaryExpression typeBinaryExpression:
            {
                foreach (var member in typeBinaryExpression.Expression.GetMembers<TMember>())
                {
                    yield return member;
                }
                break;
            }
            case UnaryExpression unaryExpression:
            {
                if (unaryExpression.Method is TMember method)
                {
                    yield return method;
                }
                foreach (var member in unaryExpression.Operand.GetMembers<TMember>())
                {
                    yield return member;
                }
                break;
            }
        }
    }
}