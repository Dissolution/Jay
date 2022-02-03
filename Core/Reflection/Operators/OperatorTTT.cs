using System.Linq.Expressions;

namespace Jay.Reflection.Operators;

public static class Operators<T1, T2, TResult>
{
    private static readonly Lazy<Func<T1, T2, TResult>> _add;
    private static readonly Lazy<Func<T1, T2, TResult>> _subtract;
    private static readonly Lazy<Func<T1, T2, TResult>> _multiply;
    private static readonly Lazy<Func<T1, T2, TResult>> _divide;
    private static readonly Lazy<Func<T1, T2, TResult>> _modulo;

    private static readonly Lazy<Func<T1, T2, TResult>> _and;
    private static readonly Lazy<Func<T1, T2, TResult>> _or;
    private static readonly Lazy<Func<T1, T2, TResult>> _xor;

    static Operators()
    {
        _add = new Lazy<Func<T1, T2, TResult>>(() => Jay.Expressions.CompileBinaryExpression<T1, T2, TResult>(Expression.Add));
        _subtract = new Lazy<Func<T1, T2, TResult>>(() => Jay.Expressions.CompileBinaryExpression<T1, T2, TResult>(Expression.Subtract));
        _multiply = new Lazy<Func<T1, T2, TResult>>(() => Jay.Expressions.CompileBinaryExpression<T1, T2, TResult>(Expression.Multiply));
        _divide = new Lazy<Func<T1, T2, TResult>>(() => Jay.Expressions.CompileBinaryExpression<T1, T2, TResult>(Expression.Divide));
        _modulo = new Lazy<Func<T1, T2, TResult>>(() => Jay.Expressions.CompileBinaryExpression<T1, T2, TResult>(Expression.Modulo));

        _and = new Lazy<Func<T1, T2, TResult>>(() => Jay.Expressions.CompileBinaryExpression<T1, T2, TResult>(Expression.And));
        _or = new Lazy<Func<T1, T2, TResult>>(() => Jay.Expressions.CompileBinaryExpression<T1, T2, TResult>(Expression.Or));
        _xor = new Lazy<Func<T1, T2, TResult>>(() => Jay.Expressions.CompileBinaryExpression<T1, T2, TResult>(Expression.ExclusiveOr));
    }

    public static TResult Multiply(T1 value1, T2 value2) => _multiply.Value(value1, value2);
    public static TResult Divide(T1 value1, T2 value2) => _divide.Value(value1, value2);
    public static TResult Mod(T1 value1, T2 value2) => _modulo.Value(value1, value2);
    public static TResult Add(T1 value1, T2 value2) => _add.Value(value1, value2);
    public static TResult Subtract(T1 value1, T2 value2) => _subtract.Value(value1, value2);

    public static TResult And(T1 value1, T2 value2) => _and.Value(value1, value2);
    public static TResult Or(T1 value1, T2 value2) => _or.Value(value1, value2);
    public static TResult Xor(T1 value1, T2 value2) => _xor.Value(value1, value2);
}