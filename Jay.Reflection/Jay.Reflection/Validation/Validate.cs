using Jay.Reflection.Exceptions;

namespace Jay.Reflection.Validation;

internal static class ValidateType
{
    public static void IsDelegateType([AllowNull, NotNull] Type? type, [CallerArgumentExpression(nameof(type))] string? typeParamName = null)
    {
        if (type is null)
        {
            throw new ArgumentNullException(typeParamName);
        }
        if (!type.Implements<Delegate>())
        {
            throw new ArgumentException(CodeBuilder.Render($"The specified type '{type}' is not a Delegate"), typeParamName);
        }
    }

    public static void IsExceptionType([AllowNull, NotNull] Type? type, [CallerArgumentExpression(nameof(type))] string? typeParamName = null)
    {
        if (type is null)
        {
            throw new ArgumentNullException(typeParamName);
        }
        if (!type.Implements<Exception>())
        {
            throw new ArgumentException(CodeBuilder.Render($"The specified type '{type}' is not an Exception"), typeParamName);
        }
    }

    public static void IsAttributeType([AllowNull, NotNull] Type? type, [CallerArgumentExpression(nameof(type))] string? typeParamName = null)
    {
        if (type is null)
        {
            throw new ArgumentNullException(typeParamName);
        }
        if (!type.Implements<Attribute>())
        {
            throw new ArgumentException(CodeBuilder.Render($"The specified type '{type}' is not an Attribute"), typeParamName);
        }
    }

    public static void IsValueType([AllowNull, NotNull] Type? type, [CallerArgumentExpression(nameof(type))] string? typeParamName = null)
    {
        if (type is null)
        {
            throw new ArgumentNullException(typeParamName);
        }
        if (!type.IsValueType)
        {
            throw new ArgumentException(CodeBuilder.Render($"The given type '{type}' must be a value type"), typeParamName);
        }
    }

    public static void IsClassOrInterfaceType(
        [AllowNull, NotNull] Type? type, [CallerArgumentExpression(nameof(type))] string? typeParamName = null)
    {
        if (type is null)
        {
            throw new ArgumentNullException(typeParamName);
        }
        if (!type.IsClass && !type.IsInterface)
        {
            throw new ArgumentException(CodeBuilder.Render($"The given type '{type}' must be a class or interface type"), typeParamName);
        }
    }

    public static void IsStaticType([AllowNull, NotNull] Type? type, [CallerArgumentExpression(nameof(type))] string? typeParamName = null)
    {
        if (type is null)
        {
            throw new ArgumentNullException(typeParamName);
        }
        if (!type.IsStatic())
        {
            throw new ArgumentException(CodeBuilder.Render($"The given type '{type}' must be a static type"), typeParamName);
        }
    }

    public static void IsEnum(
        [AllowNull, NotNull] Type? type,
        [CallerArgumentExpression(nameof(type))]
        string? typeParameterName = null)
    {
        if (type is null)
        {
            throw new ArgumentNullException(typeParameterName);
        }
        if (!type.IsValueType || !type.IsEnum)
        {
            throw new ArgumentException(
                CodeBuilder.Render($"The given type '{type}' must be an enum type"), 
                typeParameterName);
        }
    }

    public static T[] LengthIs<T>(
        [AllowNull, NotNull] T[]? array, int length,
        [CallerArgumentExpression(nameof(array))]
        string? arrayParamName = null)
    {
        if (array is null)
        {
            throw new ArgumentNullException(arrayParamName);
        }
        if (array.Length != length)
        {
            throw new ArgumentException(CodeBuilder.Render($"The given {array.GetType()} does not have {length} items"), arrayParamName);
        }
        return array;
    }
}