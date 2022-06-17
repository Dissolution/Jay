using System.Linq.Expressions;

namespace Jay.Reflection.Operators;

public static class Operator<T>
{
    private static readonly Lazy<Func<T, bool>> _isTrue;
    private static readonly Lazy<Func<T, bool>> _isFalse;

    public static T? Default { get; }
    public static T? Zero { get; }

    static Operator()
    {
        Default = default(T);
        var type = typeof(T);
        if (type.IsValueType && type.IsGenericType && type.GetGenericTypeDefinition() == typeof(Nullable<>))
        {
            // Zero is (T)0, not default(T) (which is null for Nullable<T>)
            Zero = (T?)Activator.CreateInstance(type.GetGenericArguments()[0]);
        }
        else
        {
            Zero = default(T);
        }

        _isTrue = new Lazy<Func<T, bool>>(() => Expressions.Expressions.CompileUnaryExpression<T, bool>(Expression.IsTrue));
        _isFalse = new Lazy<Func<T, bool>>(() => Expressions.Expressions.CompileUnaryExpression<T, bool>(Expression.IsFalse));
    }

    public static bool HasValue(T? value) => value is not null;

    public static bool IsTrue(T value) => _isTrue.Value(value);
    public static bool IsFalse(T value) => _isFalse.Value(value);

}