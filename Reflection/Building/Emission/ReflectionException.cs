using System.Diagnostics.CodeAnalysis;
using System.Reflection;
using System.Runtime.CompilerServices;

namespace Jay.Reflection.Emission;

internal static class Validation
{
    public static void IsValue([AllowNull, NotNull] Type? valueType,
                               [CallerArgumentExpression("valueType")]
                               string? paramName = null)
    {
        ArgumentNullException.ThrowIfNull(valueType, paramName);
        if (!valueType.IsValueType)
        {
            throw new ArgumentException($"{paramName} is not a value type", paramName);
        }
    }

    public static void IsClass([AllowNull, NotNull] Type? classType,
                               [CallerArgumentExpression("classType")]
                               string? paramName = null)
    {
        ArgumentNullException.ThrowIfNull(classType, paramName);
        if (classType.IsValueType)
        {
            throw new ArgumentException($"{paramName} is not a class type", paramName);
        }
    }

    public static void IsStatic([AllowNull, NotNull] FieldInfo? field,
                                [CallerArgumentExpression("field")] 
                                string? paramName = null)
    {
        ArgumentNullException.ThrowIfNull(field, paramName);
        if (!field.IsStatic)
        {
            throw new ArgumentException($"{paramName} is not a static Field");
        }
    }
}

public class ReflectionException : SystemException
{
    public ReflectionException(string? message = null, Exception? innerException = null)
        : base(message, innerException)
    {

    }
}