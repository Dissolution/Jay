namespace Jay.Reflection.Expressions;

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

public static class ExpressionExplorationExtensions
{
    private static void ParseValue<T, TValue>(this List<T> data, TValue? value)
    {
        if (value is T dataValue)
        {
            data.Add(dataValue);
        }
    }

    private static void ParseExpressions<T>(this List<T> data, IEnumerable<Expression?>? expressions)
    {
        if (expressions is not null)
        {
            foreach (Expression? expression in expressions)
            {
                ParseExpression(data, expression);
            }
        }
    }

    private static void ParseExpression<T>(this List<T> data, Expression? expression)
    {
        if (expression is null) return;
        ParseValue(data, expression);
        switch (expression)
        {
            case BinaryExpression binaryExpression:
            {
                ParseExpression(data, binaryExpression.Right);
                ParseExpression(data, binaryExpression.Left);
                ParseValue(data, binaryExpression.Method);
                ParseExpression(data, binaryExpression.Conversion);
                return;
            }
            case BlockExpression blockExpression:
            {
                ParseExpressions(data, blockExpression.Expressions);
                ParseExpressions(data, blockExpression.Variables);
                ParseExpression(data, blockExpression.Result);
                return;
            }
            case ConditionalExpression conditionalExpression:
            {
                ParseExpression(data, conditionalExpression.Test);
                ParseExpression(data, conditionalExpression.IfTrue);
                ParseExpression(data, conditionalExpression.IfFalse);
                return;
            }
            case ConstantExpression constantExpression:
            {
                ParseValue(data, constantExpression.Value);
                return;
            }
            case DebugInfoExpression debugInfoExpression:
            {
                ParseValue(data, debugInfoExpression.StartLine);
                ParseValue(data, debugInfoExpression.StartColumn);
                ParseValue(data, debugInfoExpression.EndLine);
                ParseValue(data, debugInfoExpression.EndColumn);
                ParseValue(data, debugInfoExpression.Document);
                return;
            }
            case DefaultExpression defaultExpression:
            {
                return;
            }
            case DynamicExpression dynamicExpression:
            {
                ParseValue(data, dynamicExpression.Binder);
                ParseValue(data, dynamicExpression.DelegateType);
                ParseExpressions(data, dynamicExpression.Arguments);
                return;
            }
            case GotoExpression gotoExpression:
            {
                ParseExpression(data, gotoExpression.Value);
                ParseValue(data, gotoExpression.Target);
                ParseValue(data, gotoExpression.Kind);
                return;
            }
            case IndexExpression indexExpression:
            {
                ParseExpression(data, indexExpression.Object);
                ParseValue(data, indexExpression.Indexer);
                ParseExpressions(data, indexExpression.Arguments);
                return;
            }
            case InvocationExpression invocationExpression:
            {
                ParseExpression(data, invocationExpression.Expression);
                ParseExpressions(data, invocationExpression.Arguments);
                return;
            }
            case LabelExpression labelExpression:
            {
                ParseValue(data, labelExpression.Target);
                ParseExpression(data, labelExpression.DefaultValue);
                return;
            }
            case LambdaExpression lambdaExpression:
            {
                ParseExpressions(data, lambdaExpression.Parameters);
                ParseValue(data, lambdaExpression.Name);
                ParseExpression(data, lambdaExpression.Body);
                ParseValue(data, lambdaExpression.ReturnType);
                return;
            }
            case ListInitExpression listInitExpression:
            {
                ParseExpression(data, listInitExpression.NewExpression);
                foreach (var initializer in listInitExpression.Initializers)
                {
                    ParseExpressions(data, initializer.Arguments);
                    ParseValue(data, initializer.AddMethod);
                }
                return;
            }
            case LoopExpression loopExpression:
            {
                ParseExpression(data, loopExpression.Body);
                ParseValue(data, loopExpression.BreakLabel);
                ParseValue(data, loopExpression.ContinueLabel);
                return;
            }
            case MemberExpression memberExpression:
            {
                ParseValue(data, memberExpression.Member);
                ParseExpression(data, memberExpression.Expression);
                return;
            }
            case MemberInitExpression memberInitExpression:
            {
                ParseExpression(data, memberInitExpression.NewExpression);
                foreach (var memberBinding in memberInitExpression.Bindings)
                {
                    ParseValue(data, memberBinding.BindingType);
                    ParseValue(data, memberBinding.Member);
                }
                return;
            }
            case MethodCallExpression methodCallExpression:
            {
                ParseValue(data, methodCallExpression.Method);
                ParseExpression(data, methodCallExpression.Object);
                ParseExpressions(data, methodCallExpression.Arguments);
                return;
            }
            case NewArrayExpression newArrayExpression:
            {
                ParseExpressions(data, newArrayExpression.Expressions);
                return;
            }
            case NewExpression newExpression:
            {
                ParseValue(data, newExpression.Constructor);
                ParseExpressions(data, newExpression.Arguments);
                if (newExpression.Members is not null)
                {
                    foreach (MemberInfo member in newExpression.Members)
                    {
                        ParseValue(data, member);
                    }
                }
                return;
            }
            case ParameterExpression parameterExpression:
            {
                ParseValue(data, parameterExpression.Name);
                return;
            }
            case RuntimeVariablesExpression runtimeVariablesExpression:
            {
                ParseExpressions(data, runtimeVariablesExpression.Variables);
                return;
            }
            case SwitchExpression switchExpression:
            {
                ParseExpression(data, switchExpression.SwitchValue);
                foreach (var switchCase in switchExpression.Cases)
                {
                    ParseExpression(data, switchCase.Body);
                    ParseExpressions(data, switchCase.TestValues);
                }
                ParseExpression(data, switchExpression.DefaultBody);
                ParseValue(data, switchExpression.Comparison);
                return;
            }
            case TryExpression tryExpression:
            {
                ParseExpression(data, tryExpression.Body);
                foreach (var handler in tryExpression.Handlers)
                {
                    ParseExpression(data, handler.Body);
                    ParseValue(data, handler.Test);
                    ParseExpression(data, handler.Filter);
                    ParseExpression(data, handler.Variable);
                }
                ParseExpression(data, tryExpression.Finally);
                ParseExpression(data, tryExpression.Fault);
                return;
            }
            case TypeBinaryExpression typeBinaryExpression:
            {
                ParseExpression(data, typeBinaryExpression.Expression);
                ParseValue(data, typeBinaryExpression.TypeOperand);
                return;
            }
            case UnaryExpression unaryExpression:
            {
                ParseExpression(data, unaryExpression.Operand);
                ParseValue(data, unaryExpression.Method);
                return;
            }
            default:
                Debugger.Break();
                throw new NotImplementedException();
        }
    }

    public static IReadOnlyList<T> Extract<T>(this Expression? expression)
    {
        var data = new List<T>();
        ParseExpression(data, expression);
        return data;
    }
}

public static class ExpressionExtensions
{
    public static IReadOnlyList<T> Extract<T>(this Expression expression)
    {
        var data = new List<T>();
        AddExpr(data, expression);
        return data;
    }

    private static void Add<T, TValue>(List<T> data, TValue? value)
    {
        if (value is T dataValue)
        {
            data.Add(dataValue);
        }
    }

    private static void AddEnumerate<T, TValue>(this List<T> data, IEnumerable<TValue?>? values)
    {
        if (values is not null)
        {
            if (values is T)
            {
                data.Add((T)values);
            }

            foreach (TValue? value in values)
            {
                if (value is T dataValue)
                {
                    data.Add(dataValue);
                }
            }
        }
    }

    private static void AddExpressions<T>(this List<T> data, IEnumerable<Expression?>? expressions)
    {
        if (expressions is not null)
        {
            foreach (Expression? expression in expressions)
            {
                AddExpr(data, expression);
            }
        }
    }

    private static void AddExpr<T>(this List<T> data, Expression? expression)
    {
        if (expression is null) return;

        // Always check if the expression itself is what we're capturing
        Add(data, expression);

        // Per expression implementation
        switch (expression)
        {
            case BinaryExpression binaryExpression:
            {
                AddExpr(data, binaryExpression.Left);
                AddExpr(data, binaryExpression.Right);
                Add(data, binaryExpression.Method);
                AddExpr(data, binaryExpression.Conversion);
                return;
            }
            case BlockExpression blockExpression:
            {
                AddExpressions(data, blockExpression.Variables);
                AddExpressions(data, blockExpression.Expressions);
                break;
            }
            case ConditionalExpression conditionalExpression:
            {
                AddExpr(data, conditionalExpression.Test);
                AddExpr(data, conditionalExpression.IfTrue);
                AddExpr(data, conditionalExpression.IfFalse);
                break;
            }
            case ConstantExpression constantExpression:
            {
                Add(data, constantExpression.Value);
                break;
            }
            case DebugInfoExpression debugInfoExpression:
            {
                // Todo: check all variables
                break;
            }
            case DefaultExpression defaultExpression:
            {
                // Todo: any variables?
                break;
            }
            case DynamicExpression dynamicExpression:
            {
                Add(data, dynamicExpression.DelegateType);
                Add(data, dynamicExpression.Binder);
                AddExpressions(data, dynamicExpression.Arguments);
                break;
            }
            case GotoExpression gotoExpression:
            {
                Add(data, gotoExpression.Target);
                AddExpr(data, gotoExpression.Value);
                break;
            }
            case IndexExpression indexExpression:
            {
                Add(data, indexExpression.Object);
                Add(data, indexExpression.Indexer);
                AddExpressions(data, indexExpression.Arguments);
                break;
            }
            case InvocationExpression invocationExpression:
            {
                AddExpr(data, invocationExpression.Expression);
                AddExpressions(data, invocationExpression.Arguments);
                break;
            }
            case LabelExpression labelExpression:
            {
                Add(data, labelExpression.Target);
                AddExpr(data, labelExpression.DefaultValue);
                break;
            }
            case LambdaExpression lambdaExpression:
            {
                Add(data, lambdaExpression.Name);
                AddExpressions(data, lambdaExpression.Parameters);
                Add(data, lambdaExpression.ReturnType);
                AddExpr(data, lambdaExpression.Body);
                break;
            }
            case ListInitExpression listInitExpression:
            {
                foreach (var initializer in listInitExpression.Initializers)
                {
                    data.AddExpressions(initializer.Arguments);
                    Add(data, initializer.AddMethod);
                }
                AddExpr(data, listInitExpression.NewExpression);
                break;
            }
            case LoopExpression loopExpression:
            {
                Add(data, loopExpression.ContinueLabel);
                Add(data, loopExpression.BreakLabel);
                AddExpr(data, loopExpression.Body);
                break;
            }
            case MemberExpression memberExpression:
            {
                Add(data, memberExpression.Member);
                AddExpr(data, memberExpression.Expression);
                break;
            }
            case MemberInitExpression memberInitExpression:
            {
                foreach (var binding in memberInitExpression.Bindings)
                {
                    Add(data, binding.Member);
                    Add(data, binding.BindingType);
                }
                AddExpr(data, memberInitExpression.NewExpression);
                break;
            }
            case MethodCallExpression methodCallExpression:
            {
                AddExpr(data, methodCallExpression.Object);
                AddExpressions(data, methodCallExpression.Arguments);
                Add(data, methodCallExpression.Method);
                break;
            }
            case NewArrayExpression newArrayExpression:
            {
                AddExpressions(data, newArrayExpression.Expressions);
                break;
            }
            case NewExpression newExpression:
            {
                AddExpressions(data, newExpression.Arguments);
                Add(data, newExpression.Constructor);
                AddEnumerate(data, newExpression.Members);
                break;
            }
            case ParameterExpression parameterExpression:
            {
                Add(data, parameterExpression.Name);
                break;
            }
            case RuntimeVariablesExpression runtimeVariablesExpression:
            {
                AddExpressions(data, runtimeVariablesExpression.Variables);
                break;
            }
            case SwitchExpression switchExpression:
            {
                AddExpr(data, switchExpression.SwitchValue);
                Add(data, switchExpression.Comparison);
                foreach (var switchCase in switchExpression.Cases)
                {
                    AddExpressions(data, switchCase.TestValues);
                    AddExpr(data, switchCase.Body);
                }
                AddExpr(data, switchExpression.DefaultBody);

                break;
            }
            case TryExpression tryExpression:
            {
                AddExpr(data, tryExpression.Body);
                foreach (var handler in tryExpression.Handlers)
                {
                    Add(data, handler.Test);
                    AddExpr(data, handler.Filter);
                    AddExpr(data, handler.Variable);
                    AddExpr(data, handler.Body);
                }
                AddExpr(data, tryExpression.Fault);
                AddExpr(data, tryExpression.Finally);
                break;
            }
            case TypeBinaryExpression typeBinaryExpression:
            {
                Add(data, typeBinaryExpression.Type);
                AddExpr(data, typeBinaryExpression.Expression);
                break;
            }
            case UnaryExpression unaryExpression:
            {
                AddExpr(data, unaryExpression.Operand);
                Add(data, unaryExpression.Method);
                break;
            }
            default:
                throw new NotImplementedException();
        }
    }



    public static IEnumerable<Expression> Descendants(this Expression? expression)
    {
        if (expression is null) yield break;
        // Always return the root
        yield return expression;
        // Special handling for each Expression type
        switch (expression)
        {
            case BinaryExpression binaryExpression:
            {
                yield return binaryExpression.Left;
                if (binaryExpression.Conversion is not null)
                    yield return binaryExpression.Conversion;
                yield return binaryExpression.Right;
                yield break;
            }
            case BlockExpression blockExpression:
            {
                foreach (var expr in blockExpression.Expressions)
                    yield return expr;
                foreach (var expr in blockExpression.Variables)
                    yield return expr;
                yield break;
            }
            case ConditionalExpression conditionalExpression:
            {
                yield return conditionalExpression.IfTrue;
                yield return conditionalExpression.IfFalse;
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
                foreach (var expr in dynamicExpression.Arguments)
                    yield return expr;
                yield break;
            }
            case GotoExpression gotoExpression:
            {
                if (gotoExpression.Value is not null)
                    yield return gotoExpression.Value;
                yield break;
            }
            case IndexExpression indexExpression:
            {
                if (indexExpression.Object is not null)
                    yield return indexExpression.Object;
                foreach (var expr in indexExpression.Arguments)
                    yield return expr;
                yield break;
            }
            case InvocationExpression invocationExpression:
            {
                foreach (var expr in invocationExpression.Arguments)
                    yield return expr;
                yield return invocationExpression.Expression;
                yield break;
            }
            case LabelExpression labelExpression:
            {
                if (labelExpression.DefaultValue is not null)
                    yield return labelExpression.DefaultValue;
                yield break;
            }
            case LambdaExpression lambdaExpression:
            {
                foreach (var expr in lambdaExpression.Parameters)
                    yield return expr;
                yield return lambdaExpression.Body;
                yield break;
            }
            case ListInitExpression listInitExpression:
            {
                foreach (var expr in Enumerable.SelectMany(listInitExpression.Initializers, init => init.Arguments))
                {
                    yield return expr;
                }
                yield return listInitExpression.NewExpression;
                yield break;
            }
            case LoopExpression loopExpression:
            {
                yield return loopExpression.Body;
                yield break;
            }
            case MemberExpression memberExpression:
            {
                if (memberExpression.Expression is not null)
                    yield return memberExpression.Expression;
                yield break;
            }
            case MemberInitExpression memberInitExpression:
            {
                yield return memberInitExpression.NewExpression;
                yield break;
            }
            case MethodCallExpression methodCallExpression:
            {
                if (methodCallExpression.Object is not null)
                    yield return methodCallExpression.Object;
                foreach (var expr in methodCallExpression.Arguments)
                    yield return expr;
                yield break;
            }
            case NewArrayExpression newArrayExpression:
            {
                foreach (var expr in newArrayExpression.Expressions)
                    yield return expr;
                yield break;
            }
            case NewExpression newExpression:
            {
                foreach (var expr in newExpression.Arguments)
                    yield return expr;
                yield break;
            }
            case ParameterExpression parameterExpression:
            {
                yield break;
            }
            case RuntimeVariablesExpression runtimeVariablesExpression:
            {
                foreach (var expr in runtimeVariablesExpression.Variables)
                    yield return expr;
                yield break;
            }
            case SwitchExpression switchExpression:
            {
                foreach (var cass in switchExpression.Cases)
                {
                    foreach (var expr in cass.TestValues)
                        yield return expr;
                    yield return cass.Body;
                }
                yield return switchExpression.SwitchValue;
                if (switchExpression.DefaultBody is not null)
                    yield return switchExpression.DefaultBody;
                yield break;
            }
            case TryExpression tryExpression:
            {
                yield return tryExpression.Body;
                foreach (var handler in tryExpression.Handlers)
                {
                    if (handler.Filter is not null)
                        yield return handler.Filter;
                    yield return handler.Body;
                    if (handler.Variable is not null)
                        yield return handler.Variable;
                }
                if (tryExpression.Fault is not null)
                    yield return tryExpression.Fault;
                if (tryExpression.Finally is not null)
                    yield return tryExpression.Finally;
                yield break;
            }
            case TypeBinaryExpression typeBinaryExpression:
            {
                yield return typeBinaryExpression.Expression;
                yield break;
            }
            case UnaryExpression unaryExpression:
            {
                yield return unaryExpression.Operand;
                yield break;
            }
            default:
            {
                throw new NotImplementedException();
            }
        }
    }


    public static TMember? ExtractMember<TMember>(this Expression expression)
        where TMember : MemberInfo
    {
        return Enumerable.FirstOrDefault(Enumerable.OfType<TMember>(ExtractMembers(expression)));
    }

    public static TValue? ExtractValue<TValue>(this Expression expression)
    {
        return Enumerable.FirstOrDefault(Enumerable.OfType<TValue>(ExtractValues(expression)));
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
                foreach (var value in ExtractValues(binaryExpression.Left))
                {
                    yield return value;
                }
                //
                // if (binaryExpression.Method is not null)
                // {
                //     yield return binaryExpression.Method;
                // }

                foreach (var value in ExtractValues(binaryExpression.Right))
                {
                    yield return value;
                }

                break;
            }
            case BlockExpression blockExpression:
            {
                foreach (var value in Enumerable.SelectMany(blockExpression.Expressions, expr => ExtractValues(expr)))
                {
                    yield return value;
                }

                break;
            }
            case ConditionalExpression conditionalExpression:
            {
                foreach (var value in ExtractValues(conditionalExpression.IfTrue))
                {
                    yield return value;
                }

                foreach (var value in ExtractValues(conditionalExpression.IfFalse))
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
                foreach (var value in Enumerable.SelectMany(dynamicExpression.Arguments, expr => ExtractValues(expr)))
                {
                    yield return value;
                }

                break;
            }
            case GotoExpression gotoExpression:
            {
                foreach (var value in ExtractValues(gotoExpression.Value))
                {
                    yield return value;
                }

                break;
            }
            case IndexExpression indexExpression:
            {
                foreach (var value in ExtractValues(indexExpression.Object))
                {
                    yield return value;
                }

                foreach (var value in Enumerable.SelectMany(indexExpression.Arguments, expr => ExtractValues(expr)))
                {
                    yield return value;
                }

                break;
            }
            case InvocationExpression invocationExpression:
            {
                foreach (var value in ExtractValues(invocationExpression.Expression))
                {
                    yield return value;
                }

                foreach (var value in Enumerable.SelectMany(invocationExpression.Arguments, expr => ExtractValues(expr)))
                {
                    yield return value;
                }

                break;
            }
            case LabelExpression labelExpression:
            {
                foreach (var value in ExtractValues(labelExpression.DefaultValue))
                {
                    yield return value;
                }

                break;
            }
            case LambdaExpression lambdaExpression:
            {
                foreach (var value in ExtractValues(lambdaExpression.Body))
                {
                    yield return value;
                }

                foreach (var value in Enumerable.SelectMany(lambdaExpression.Parameters, expr => ExtractValues(expr)))
                {
                    yield return value;
                }

                break;
            }
            case ListInitExpression listInitExpression:
            {
                foreach (var value in ExtractValues(listInitExpression.NewExpression))
                {
                    yield return value;
                }

                break;
            }
            case LoopExpression loopExpression:
            {
                foreach (var value in ExtractValues(loopExpression.Body))
                {
                    yield return value;
                }

                break;
            }
            case MemberExpression memberExpression:
            {
                foreach (var value in ExtractValues(memberExpression.Expression))
                {
                    yield return value;
                }

                break;
            }
            case MemberInitExpression memberInitExpression:
            {
                foreach (var value in ExtractValues(memberInitExpression.NewExpression))
                {
                    yield return value;
                }

                break;
            }
            case MethodCallExpression methodCallExpression:
            {
                foreach (var value in ExtractValues(methodCallExpression.Object))
                {
                    yield return value;
                }

                foreach (var value in Enumerable.SelectMany(methodCallExpression.Arguments, expr => ExtractValues(expr)))
                {
                    yield return value;
                }

                break;
            }
            case NewArrayExpression newArrayExpression:
            {
                foreach (var value in Enumerable.SelectMany(newArrayExpression.Expressions, expr => ExtractValues(expr)))
                {
                    yield return value;
                }

                break;
            }
            case NewExpression newExpression:
            {
                foreach (var value in Enumerable.SelectMany(newExpression.Arguments, expr => ExtractValues(expr)))
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
                foreach (var value in Enumerable.SelectMany(runtimeVariablesExpression.Variables, expr => ExtractValues(expr)))
                {
                    yield return value;
                }

                break;
            }
            case SwitchExpression switchExpression:
            {
                foreach (var switchCase in switchExpression.Cases)
                {
                    foreach (var value in Enumerable.SelectMany(switchCase.TestValues, expr => ExtractValues(expr)))
                    {
                        yield return value;
                    }

                    foreach (var value in ExtractValues(switchCase.Body))
                    {
                        yield return value;
                    }
                }

                foreach (var value in ExtractValues(switchExpression.DefaultBody))
                {
                    yield return value;
                }

                break;
            }
            case TryExpression tryExpression:
            {
                foreach (var value in ExtractValues(tryExpression.Body))
                {
                    yield return value;
                }

                foreach (var handler in tryExpression.Handlers)
                {
                    foreach (var value in ExtractValues(handler.Filter))
                    {
                        yield return value;
                    }

                    foreach (var value in ExtractValues(handler.Variable))
                    {
                        yield return value;
                    }

                    foreach (var value in ExtractValues(handler.Body))
                    {
                        yield return value;
                    }
                }

                foreach (var value in ExtractValues(tryExpression.Fault))
                {
                    yield return value;
                }

                foreach (var value in ExtractValues(tryExpression.Finally))
                {
                    yield return value;
                }

                break;
            }
            case TypeBinaryExpression typeBinaryExpression:
            {
                foreach (var value in ExtractValues(typeBinaryExpression.Expression))
                {
                    yield return value;
                }

                break;
            }
            case UnaryExpression unaryExpression:
            {
                foreach (var value in ExtractValues(unaryExpression.Operand))
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
                foreach (var member in ExtractMembers(binaryExpression.Left))
                {
                    yield return member;
                }

                if (binaryExpression.Method is not null)
                {
                    yield return binaryExpression.Method;
                }

                foreach (var member in ExtractMembers(binaryExpression.Right))
                {
                    yield return member;
                }

                break;
            }
            case BlockExpression blockExpression:
            {
                foreach (var member in Enumerable.SelectMany(blockExpression.Expressions, expr => ExtractMembers(expr)))
                {
                    yield return member;
                }

                break;
            }
            case ConditionalExpression conditionalExpression:
            {
                foreach (var member in ExtractMembers(conditionalExpression.IfTrue))
                {
                    yield return member;
                }

                foreach (var member in ExtractMembers(conditionalExpression.IfFalse))
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
                foreach (var member in Enumerable.SelectMany(dynamicExpression.Arguments, expr => ExtractMembers(expr)))
                {
                    yield return member;
                }

                break;
            }
            case GotoExpression gotoExpression:
            {
                foreach (var member in ExtractMembers(gotoExpression.Value))
                {
                    yield return member;
                }

                break;
            }
            case IndexExpression indexExpression:
            {
                foreach (var member in ExtractMembers(indexExpression.Object))
                {
                    yield return member;
                }

                foreach (var member in Enumerable.SelectMany(indexExpression.Arguments, expr => ExtractMembers(expr)))
                {
                    yield return member;
                }

                break;
            }
            case InvocationExpression invocationExpression:
            {
                foreach (var member in ExtractMembers(invocationExpression.Expression))
                {
                    yield return member;
                }

                foreach (var member in Enumerable.SelectMany(invocationExpression.Arguments, expr => ExtractMembers(expr)))
                {
                    yield return member;
                }

                break;
            }
            case LabelExpression labelExpression:
            {
                foreach (var member in ExtractMembers(labelExpression.DefaultValue))
                {
                    yield return member;
                }

                break;
            }
            case LambdaExpression lambdaExpression:
            {
                foreach (var member in ExtractMembers(lambdaExpression.Body))
                {
                    yield return member;
                }

                foreach (var member in Enumerable.SelectMany(lambdaExpression.Parameters, expr => ExtractMembers(expr)))
                {
                    yield return member;
                }

                break;
            }
            case ListInitExpression listInitExpression:
            {
                foreach (var member in ExtractMembers(listInitExpression.NewExpression))
                {
                    yield return member;
                }

                break;
            }
            case LoopExpression loopExpression:
            {
                foreach (var member in ExtractMembers(loopExpression.Body))
                {
                    yield return member;
                }

                break;
            }
            case MemberExpression memberExpression:
            {
                yield return memberExpression.Member;

                foreach (var member in ExtractMembers(memberExpression.Expression))
                {
                    yield return member;
                }

                break;
            }
            case MemberInitExpression memberInitExpression:
            {
                foreach (var member in ExtractMembers(memberInitExpression.NewExpression))
                {
                    yield return member;
                }

                break;
            }
            case MethodCallExpression methodCallExpression:
            {
                foreach (var member in ExtractMembers(methodCallExpression.Object))
                {
                    yield return member;
                }

                foreach (var member in Enumerable.SelectMany(methodCallExpression.Arguments, expr => ExtractMembers(expr)))
                {
                    yield return member;
                }

                yield return methodCallExpression.Method;

                break;
            }
            case NewArrayExpression newArrayExpression:
            {
                foreach (var member in Enumerable.SelectMany(newArrayExpression.Expressions, expr => ExtractMembers(expr)))
                {
                    yield return member;
                }

                break;
            }
            case NewExpression newExpression:
            {
                foreach (var member in Enumerable.SelectMany(newExpression.Arguments, expr => ExtractMembers(expr)))
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
                foreach (var member in Enumerable.SelectMany(runtimeVariablesExpression.Variables, expr => ExtractMembers(expr)))
                {
                    yield return member;
                }

                break;
            }
            case SwitchExpression switchExpression:
            {
                foreach (var switchCase in switchExpression.Cases)
                {
                    foreach (var member in Enumerable.SelectMany(switchCase.TestValues, expr => ExtractMembers(expr)))
                    {
                        yield return member;
                    }

                    foreach (var member in ExtractMembers(switchCase.Body))
                    {
                        yield return member;
                    }
                }

                foreach (var member in ExtractMembers(switchExpression.DefaultBody))
                {
                    yield return member;
                }

                break;
            }
            case TryExpression tryExpression:
            {
                foreach (var member in ExtractMembers(tryExpression.Body))
                {
                    yield return member;
                }

                foreach (var handler in tryExpression.Handlers)
                {
                    foreach (var member in ExtractMembers(handler.Filter))
                    {
                        yield return member;
                    }

                    foreach (var member in ExtractMembers(handler.Variable))
                    {
                        yield return member;
                    }

                    foreach (var member in ExtractMembers(handler.Body))
                    {
                        yield return member;
                    }
                }

                foreach (var member in ExtractMembers(tryExpression.Fault))
                {
                    yield return member;
                }

                foreach (var member in ExtractMembers(tryExpression.Finally))
                {
                    yield return member;
                }

                break;
            }
            case TypeBinaryExpression typeBinaryExpression:
            {
                foreach (var member in ExtractMembers(typeBinaryExpression.Expression))
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

                foreach (var member in ExtractMembers(unaryExpression.Operand))
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
                foreach (var expr in ExtractExpressions<TExpression>(binaryExpression.Left))
                {
                    yield return expr;
                }

                foreach (var expr in ExtractExpressions<TExpression>(binaryExpression.Right))
                {
                    yield return expr;
                }

                yield break;
            }
            case BlockExpression blockExpression:
            {
                foreach (var expr in Enumerable.SelectMany(blockExpression.Expressions, expr => ExtractExpressions<TExpression>(expr)))
                {
                    yield return expr;
                }

                yield break;
            }
            case ConditionalExpression conditionalExpression:
            {
                foreach (var expr in ExtractExpressions<TExpression>(conditionalExpression.IfTrue))
                {
                    yield return expr;
                }

                foreach (var expr in ExtractExpressions<TExpression>(conditionalExpression.IfFalse))
                {
                    yield return expr;
                }

                yield break;
            }
            case DynamicExpression dynamicExpression:
            {
                foreach (var expr in Enumerable.SelectMany(dynamicExpression.Arguments, expr => ExtractExpressions<TExpression>(expr)))
                {
                    yield return expr;
                }

                yield break;
            }
            case GotoExpression gotoExpression:
            {
                foreach (var expr in ExtractExpressions<TExpression>(gotoExpression.Value))
                {
                    yield return expr;
                }

                yield break;
            }
            case IndexExpression indexExpression:
            {
                foreach (var expr in ExtractExpressions<TExpression>(indexExpression.Object))
                {
                    yield return expr;
                }

                foreach (var expr in Enumerable.SelectMany(indexExpression.Arguments, expr => ExtractExpressions<TExpression>(expr)))
                {
                    yield return expr;
                }

                yield break;
            }
            case InvocationExpression invocationExpression:
            {
                foreach (var expr in ExtractExpressions<TExpression>(invocationExpression.Expression))
                {
                    yield return expr;
                }

                foreach (var expr in Enumerable.SelectMany(invocationExpression.Arguments, expr => ExtractExpressions<TExpression>(expr)))
                {
                    yield return expr;
                }

                yield break;
            }
            case LabelExpression labelExpression:
            {
                foreach (var expr in ExtractExpressions<TExpression>(labelExpression.DefaultValue))
                {
                    yield return expr;
                }

                yield break;
            }
            case LambdaExpression lambdaExpression:
            {
                foreach (var expr in ExtractExpressions<TExpression>(lambdaExpression.Body))
                {
                    yield return expr;
                }

                foreach (var expr in Enumerable.SelectMany(lambdaExpression.Parameters, expr => ExtractExpressions<TExpression>(expr)))
                {
                    yield return expr;
                }

                yield break;
            }
            case ListInitExpression listInitExpression:
            {
                foreach (var expr in ExtractExpressions<TExpression>(listInitExpression.NewExpression))
                {
                    yield return expr;
                }

                yield break;
            }
            case LoopExpression loopExpression:
            {
                foreach (var expr in ExtractExpressions<TExpression>(loopExpression.Body))
                {
                    yield return expr;
                }

                yield break;
            }
            case MemberExpression memberExpression:
            {
                foreach (var expr in ExtractExpressions<TExpression>(memberExpression.Expression))
                {
                    yield return expr;
                }

                yield break;
            }
            case MemberInitExpression memberInitExpression:
            {
                foreach (var expr in ExtractExpressions<TExpression>(memberInitExpression.NewExpression))
                {
                    yield return expr;
                }

                yield break;
            }
            case MethodCallExpression methodCallExpression:
            {
                foreach (var expr in ExtractExpressions<TExpression>(methodCallExpression.Object))
                {
                    yield return expr;
                }

                foreach (var expr in Enumerable.SelectMany(methodCallExpression.Arguments, expr => ExtractExpressions<TExpression>(expr)))
                {
                    yield return expr;
                }

                yield break;
            }
            case NewArrayExpression newArrayExpression:
            {
                foreach (var expr in Enumerable.SelectMany(newArrayExpression.Expressions, expr => ExtractExpressions<TExpression>(expr)))
                {
                    yield return expr;
                }

                yield break;
            }
            case NewExpression newExpression:
            {
                foreach (var expr in Enumerable.SelectMany(newExpression.Arguments, expr => ExtractExpressions<TExpression>(expr)))
                {
                    yield return expr;
                }

                yield break;
            }
            case RuntimeVariablesExpression runtimeVariablesExpression:
            {
                foreach (var expr in Enumerable.SelectMany(runtimeVariablesExpression.Variables,
                             expr =>
                                 ExtractExpressions<TExpression>(expr)))
                {
                    yield return expr;
                }

                yield break;
            }
            case SwitchExpression switchExpression:
            {
                foreach (var switchCase in switchExpression.Cases)
                {
                    foreach (var expr in Enumerable.SelectMany(switchCase.TestValues, expr => ExtractExpressions<TExpression>(expr)))
                    {
                        yield return expr;
                    }

                    foreach (var expr in ExtractExpressions<TExpression>(switchCase.Body))
                    {
                        yield return expr;
                    }
                }

                foreach (var expr in ExtractExpressions<TExpression>(switchExpression.DefaultBody))
                {
                    yield return expr;
                }

                yield break;
            }
            case TryExpression tryExpression:
            {
                foreach (var expr in ExtractExpressions<TExpression>(tryExpression.Body))
                {
                    yield return expr;
                }

                foreach (var handler in tryExpression.Handlers)
                {
                    foreach (var expr in ExtractExpressions<TExpression>(handler.Filter))
                    {
                        yield return expr;
                    }

                    foreach (var expr in ExtractExpressions<TExpression>(handler.Variable))
                    {
                        yield return expr;
                    }

                    foreach (var expr in ExtractExpressions<TExpression>(handler.Body))
                    {
                        yield return expr;
                    }
                }

                foreach (var expr in ExtractExpressions<TExpression>(tryExpression.Fault))
                {
                    yield return expr;
                }

                foreach (var expr in ExtractExpressions<TExpression>(tryExpression.Finally))
                {
                    yield return expr;
                }

                yield break;
            }
            case TypeBinaryExpression typeBinaryExpression:
            {
                foreach (var expr in ExtractExpressions<TExpression>(typeBinaryExpression.Expression))
                {
                    yield return expr;
                }

                yield break;
            }
            case UnaryExpression unaryExpression:
            {
                foreach (var expr in ExtractExpressions<TExpression>(unaryExpression.Operand))
                {
                    yield return expr;
                }

                yield break;
            }
        }
    }
}