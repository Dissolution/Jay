using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Linq.Expressions;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

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
    public static T ThrowIfNull<T>([AllowNull, NotNull] this T value,
                                   [CallerArgumentExpression("value")] string? valueName = null)
    {
        if (value is null)
        {
            throw new ArgValidationException(value, $"{valueName} is null");
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
            if (result.Failed(out var ex))
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
            if (result.Failed(out var ex))
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