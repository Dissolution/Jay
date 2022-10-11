using Jay.Text;
using System.Linq.Expressions;

namespace Jay.Dumping;

public static partial class Dumper
{
    private static void DumpExpressionTo(Expression? expression, TextBuilder textBuilder)
    {
        switch (expression)
        {
            case null:
            {
                return;
            }
            case BinaryExpression binaryExpression:
            {
                throw new NotImplementedException();
            }
            case BlockExpression blockExpression:
                throw new NotImplementedException();
            case ConditionalExpression conditionalExpression:
                throw new NotImplementedException();
            case ConstantExpression constantExpression:
            {
                DumpValueTo<object>(constantExpression.Value, textBuilder);
                return;
            }
            case DebugInfoExpression debugInfoExpression:
                throw new NotImplementedException();
            case DefaultExpression defaultExpression:
                throw new NotImplementedException();
            case DynamicExpression dynamicExpression:
                throw new NotImplementedException();
            case GotoExpression gotoExpression:
                throw new NotImplementedException();
            case IndexExpression indexExpression:
                throw new NotImplementedException();
            case InvocationExpression invocationExpression:
                throw new NotImplementedException();
            case LabelExpression labelExpression:
                throw new NotImplementedException();
            case LambdaExpression lambdaExpression:
                throw new NotImplementedException();
            case ListInitExpression listInitExpression:
                throw new NotImplementedException();
            case LoopExpression loopExpression:
                throw new NotImplementedException();
            case MemberExpression memberExpression:
                throw new NotImplementedException();
            case MemberInitExpression memberInitExpression:
                throw new NotImplementedException();
            case MethodCallExpression methodCallExpression:
                throw new NotImplementedException();
            case NewArrayExpression newArrayExpression:
                throw new NotImplementedException();
            case NewExpression newExpression:
                throw new NotImplementedException();
            case ParameterExpression parameterExpression:
                throw new NotImplementedException();
            case RuntimeVariablesExpression runtimeVariablesExpression:
                throw new NotImplementedException();
            case SwitchExpression switchExpression:
                throw new NotImplementedException();
            case TryExpression tryExpression:
                throw new NotImplementedException();
            case TypeBinaryExpression typeBinaryExpression:
                throw new NotImplementedException();
            case UnaryExpression unaryExpression:
                throw new NotImplementedException();
            default:
                throw new ArgumentOutOfRangeException(nameof(expression));
        }
    }
}