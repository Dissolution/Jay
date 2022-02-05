using System.Collections.Concurrent;
using System.Linq.Expressions;
using System.Reflection;
using Jay.Collections;

namespace Jay.Reflection.Operators;

public static class Operator
{
   // TODO: Giant cache of all Type:Operator:Func
}


public class ExpressionDelegates
{
    private readonly ConcurrentDictionary<ExpressionType, Delegate> _cache;

    public Type Type { get; }

    public ExpressionDelegates(Type type)
    {
        this.Type = type;
        _cache = new ConcurrentDictionary<ExpressionType, Delegate>();
    }

    protected Func<object?, object?> CreateUnaryFunction(Func<Expression, UnaryExpression> unaryExpr)
    {
        var objParam = Expression.Parameter(typeof(object), "obj");
        var objConvExpr = Expression.Convert(objParam, Type);
        var convBack = Expression.Convert(unaryExpr(objConvExpr), Type);
        var lambda = Expression.Lambda<Func<object?, object?>>(convBack, objParam);
        return lambda.Compile();
    }

    protected Func<object?, object?, object?> CreateBinaryFunction(Func<Expression, Expression, UnaryExpression> binaryExpr)
    {
        var obj1 = Expression.Parameter(typeof(object), "obj1");
        var arg1 = Expression.Convert(obj1, Type);
        var obj2 = Expression.Parameter(typeof(object), "obj2");
        var arg2 = Expression.Convert(obj2, Type);
        var body = binaryExpr(arg1, arg2);
        var ret = Expression.Convert(body, typeof(object));
        var lambda = Expression.Lambda<Func<object?, object?, object?>>(ret, obj1, obj2);
        return lambda.Compile();
    }

    protected static readonly Dictionary<ExpressionType, string> _implicitMethodNames;

    internal sealed record class ExpressionDetails(ExpressionType ExpressionType,
                                                   string OperatorMethodName,
                                                   string Rep);

    static ExpressionDelegates()
    {
        _implicitMethodNames = new Dictionary<ExpressionType, string>
        {
            {ExpressionType.Convert, "op_Implicit"},
            {ExpressionType.Add, "op_Addition"},
            {ExpressionType.Subtract, "op_Subtraction"},
            {ExpressionType.Multiply, "op_Multiply"},
            {ExpressionType.Divide, "op_Division"},
            {ExpressionType.Modulo, "op_Modulus"},
            {ExpressionType.ExclusiveOr, "op_ExclusiveOr"},
            {ExpressionType.And, "op_BitwiseAnd"},
            {ExpressionType.Or, "op_BitwiseOr"},
            {ExpressionType.AndAlso, "op_LogicalAnd"},
            {ExpressionType.OrElse, "op_LogicalOr"},
            {ExpressionType.Assign, "op_Assign"},
            {ExpressionType.LeftShift, "op_LeftShift"},
            {ExpressionType.RightShift, "op_RightShift"},
            {ExpressionType.Equal, "op_Equality"},
            {ExpressionType.NotEqual, "op_Inequality"},
            {ExpressionType.GreaterThan, "op_GreaterThan"},
            {ExpressionType.LessThan, "op_LessThan"},
            {ExpressionType.GreaterThanOrEqual, "op_GreaterThanOrEqual"},
            {ExpressionType.LessThanOrEqual, "op_LessThanOrEqual"},
            {ExpressionType.MultiplyAssign, "op_MultiplicationAssignment"},
            {ExpressionType.SubtractAssign, "op_SubtractionAssignment"},
            {ExpressionType.ExclusiveOrAssign, "op_ExclusiveOrAssignment"},
            {ExpressionType.LeftShiftAssign, "op_LeftShiftAssignment"},
            {ExpressionType.RightShiftAssign, "op_RightShiftAssignment"},
            {ExpressionType.ModuloAssign, "op_ModulusAssignment"},
            {ExpressionType.AddAssign, "op_AdditionAssignment"},
            {ExpressionType.AndAssign, "op_BitwiseAndAssignment"},
            {ExpressionType.OrAssign, "op_BitwiseOrAssignment"},
            {ExpressionType.DivideAssign, "op_DivisionAssignment"},
            {ExpressionType.Decrement, "op_Decrement"},
            {ExpressionType.Increment, "op_Increment"},
            {ExpressionType.Negate, "op_UnaryNegation"},
            {ExpressionType.UnaryPlus, "op_UnaryPlus"},
            {ExpressionType.OnesComplement, "op_OnesComplement"},
        };
    }

    private Func<object?[], object?> CreateDelegate(ExpressionType expressionType)
    {
        MethodInfo? method = null;
        if (_implicitMethodNames.TryGetValue(expressionType, out var opMethodName))
        {
            method = Type.GetMethod(opMethodName, Reflect.StaticFlags);
        }

        if (method is null)
        {
            var exprName = expressionType.ToString();

            // method = Type.GetMethods(Reflect.AllFlags)
            //              .Where(method =>
            //              {
            //                  if (string.Equals(method.Name, exprName, StringComparison.OrdinalIgnoreCase))
            //                  {
            //                      
            //                  }
            //              })
            throw new NotImplementedException();
        }
        throw new NotImplementedException();
    }


    public TDelegate GetDelegate<TDelegate>(ExpressionType expressionType,
                                            params object?[] args)
        where TDelegate : Delegate
    {
        var del = _cache.GetOrAdd(expressionType, CreateDelegate);
        del.DynamicInvoke(args);

        throw new NotImplementedException();

    }

   
}



