using System.Linq.Expressions;

namespace Jay.Reflection.Operators;

public static class Operator<TValue, TResult>
{
    private static readonly Lazy<Func<TValue, TResult, bool>> _lessThan;
    private static readonly Lazy<Func<TValue, TResult, bool>> _lessThanOrEqual;
    private static readonly Lazy<Func<TValue, TResult, bool>> _greaterThan;
    private static readonly Lazy<Func<TValue, TResult, bool>> _greaterThanOrEqual;

    private static readonly Lazy<Func<TValue, TResult, bool>> _equal;
    private static readonly Lazy<Func<TValue, TResult, bool>> _notEqual;

    private static readonly Lazy<Func<TValue, TResult>> _increment;
    private static readonly Lazy<Func<TValue, TResult>> _decrement;
    private static readonly Lazy<Func<TValue, TResult>> _plus;
    private static readonly Lazy<Func<TValue, TResult>> _minus;
    private static readonly Lazy<Func<TValue, TResult>> _not;
    private static readonly Lazy<Func<TValue, TResult>> _complement;

    private static readonly Lazy<Func<TValue, TResult>> _convert;

    private static readonly Lazy<Func<TValue, int, TResult>> _leftShift;
    private static readonly Lazy<Func<TValue, int, TResult>> _rightShift;

    static Operator()
    {
        _lessThan = new Lazy<Func<TValue, TResult, bool>>(() => Expressions.Expressions.CompileBinaryExpression<TValue, TResult, bool>(Expression.LessThan));
        _lessThanOrEqual = new Lazy<Func<TValue, TResult, bool>>(() => Expressions.Expressions.CompileBinaryExpression<TValue, TResult, bool>(Expression.LessThanOrEqual));
        _greaterThan = new Lazy<Func<TValue, TResult, bool>>(() => Expressions.Expressions.CompileBinaryExpression<TValue, TResult, bool>(Expression.GreaterThan));
        _greaterThanOrEqual = new Lazy<Func<TValue, TResult, bool>>(() => Expressions.Expressions.CompileBinaryExpression<TValue, TResult, bool>(Expression.GreaterThanOrEqual));

        _equal = new Lazy<Func<TValue, TResult, bool>>(() => Expressions.Expressions.CompileBinaryExpression<TValue, TResult, bool>(Expression.Equal));
        _notEqual = new Lazy<Func<TValue, TResult, bool>>(() => Expressions.Expressions.CompileBinaryExpression<TValue, TResult, bool>(Expression.NotEqual));

        _increment = new Lazy<Func<TValue, TResult>>(() => Expressions.Expressions.CompileUnaryExpression<TValue, TResult>(Expression.Increment));
        _decrement = new Lazy<Func<TValue, TResult>>(() => Expressions.Expressions.CompileUnaryExpression<TValue, TResult>(Expression.Decrement));
        _plus = new Lazy<Func<TValue, TResult>>(() => Expressions.Expressions.CompileUnaryExpression<TValue, TResult>(Expression.UnaryPlus));
        _minus = new Lazy<Func<TValue, TResult>>(() => Expressions.Expressions.CompileUnaryExpression<TValue, TResult>(Expression.Negate));
        _not = new Lazy<Func<TValue, TResult>>(() => Expressions.Expressions.CompileUnaryExpression<TValue, TResult>(Expression.Not));
        _complement = new Lazy<Func<TValue, TResult>>(() => Expressions.Expressions.CompileUnaryExpression<TValue, TResult>(Expression.OnesComplement));

        _convert = new Lazy<Func<TValue, TResult>>(() => Expressions.Expressions.CompileUnaryExpression<TValue, TResult>(body => Expression.Convert(body, typeof(TResult))));

        _leftShift = new Lazy<Func<TValue, int, TResult>>(() => Expressions.Expressions.CompileBinaryExpression<TValue, int, TResult>(Expression.LeftShift));
        _rightShift = new Lazy<Func<TValue, int, TResult>>(() => Expressions.Expressions.CompileBinaryExpression<TValue, int, TResult>(Expression.RightShift));
    }

    public static bool LessThan(TValue left, TResult right) => _lessThan.Value(left, right);
    public static bool LessThanOrEqual(TValue left, TResult right) => _lessThanOrEqual.Value(left, right);
    public static bool GreaterThan(TValue left, TResult right) => _greaterThan.Value(left, right);
    public static bool GreaterThanOrEqual(TValue left, TResult right) => _greaterThanOrEqual.Value(left, right);

    public static bool Equals(TValue left, TResult right) => _equal.Value(left, right);
    public static bool NotEquals(TValue left, TResult right) => _notEqual.Value(left, right);


    public static TResult Increment(TValue value) => _increment.Value(value);
    public static TResult Decrement(TValue value) => _decrement.Value(value);
    public static TResult Plus(TValue value) => _plus.Value(value);
    public static TResult Minus(TValue value) => _minus.Value(value);
    public static TResult Not(TValue value) => _not.Value(value);
    public static TResult Complement(TValue value) => _complement.Value(value);

    public static TResult LeftShift(TValue value, int shift) => _leftShift.Value(value, shift);
    public static TResult RightShift(TValue value, int shift) => _rightShift.Value(value, shift);


}