using System.Linq.Expressions;
using Jay.Dumping;
using Dumper = Jay.Dumping.Refactor.Dumper;

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
    public static T ThrowIfNull<T>([NotNull] this T? value, 
        [CallerArgumentExpression("value")] string? valueName = null)
    {
        if (value is null)
        {
            throw new ArgumentNullException(
                valueName,
                Dumper.Dump($"{typeof(T)} value is null"));
        }
        return value;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    [return: NotNull]
    public static T ThrowIfNull<T>([NotNull] this T? value,
                                   ref DumpStringHandler message,
                                   [CallerArgumentExpression("value")] string? valueName = null)
    {
        if (value is null)
        {
            throw new ArgumentNullException(
                valueName,
                message.ToStringAndClear());
        }
        return value;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static T Assert<T>(this T value, 
                              Expression<Func<T, Result>> predicateExpression)
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
                              Expression<Func<Result>> predicateExpression)
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