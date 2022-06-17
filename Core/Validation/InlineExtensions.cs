using System.Linq.Expressions;
using Jay.Dumping;

namespace Jay.Validation;

public class ValidationException : Exception
{
    public ValidationException(string? message = null, Exception? innerException = null)
        : base(message, innerException)
    {

    }
}

public class ArgValidationException : ValidationException
{
    public object? Arg { get; }

    public ArgValidationException(object? arg, string? message = null, Exception? innerException = null)
        : base(message, innerException)
    {
        this.Arg = arg;
    }
}

public static class InlineExtensions
{
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    [return: NotNull]
    public static T ThrowIfNull<T>([AllowNull, NotNull] this T? value)
    {
        if (value is null)
        {
            throw new ValidationException("Value is null");
        }
        return value;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    [return: NotNull]
    public static T ThrowIfNull<T>([AllowNull, NotNull] this T? value,
                                   ref DumpStringHandler message)
    {
        if (value is null)
        {
            throw new ValidationException(message.ToStringAndClear());
        }
        return value;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static T Assert<T>(this T value, 
                              Expression<Func<T, Result.Result>> predicateExpression)
    {
#if DEBUG
        if (predicateExpression is null)
            throw new ArgumentNullException(nameof(predicateExpression));
        var predicate = predicateExpression.Compile();
        try
        {
            var result = predicate(value);
            if (result.IsFailure(out var ex))
            {
                throw new ArgValidationException(value, $"Assert Failed: {predicateExpression}", ex);
            }
        }
        catch (Exception ex)
        {
            throw new ArgValidationException(value, $"Assert Failed: {predicateExpression}", ex);
        }
#endif
        return value;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static T Assert<T>(this T value,
                              Expression<Func<Result.Result>> predicateExpression)
    {
#if DEBUG
        if (predicateExpression is null)
            throw new ArgumentNullException(nameof(predicateExpression));
        var predicate = predicateExpression.Compile();
        try
        {
            var result = predicate();
            if (result.IsFailure(out var ex))
            {
                throw new ValidationException($"Assert Failed: {predicateExpression}", ex);
            }
        }
        catch (Exception ex)
        {
            throw new ValidationException($"Assert Failed: {predicateExpression}", ex);
        }
#endif
        return value;
    }
}