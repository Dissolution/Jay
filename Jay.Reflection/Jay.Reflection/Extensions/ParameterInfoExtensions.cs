using System.Diagnostics;

namespace Jay.Reflection.Extensions;

public static class ParameterInfoExtensions
{
    public static ParameterAccess GetAccess(this ParameterInfo parameter, out Type parameterType)
    {
        parameterType = parameter.ParameterType;
        if (parameterType.IsByRef)
        {
            parameterType = parameterType.GetElementType().ThrowIfNull();
            if (parameter.IsIn)
            {
                return ParameterAccess.In;
            }

            if (parameter.IsOut)
            {
                return ParameterAccess.Out;
            }

            return ParameterAccess.Ref;
        }

        if (parameter.IsIn || parameter.IsOut)
            throw new NotImplementedException();
        
        return ParameterAccess.Default;
    }

    /// <summary>
    /// Gets this <paramref name="parameter"/>'s value <see cref="Type"/>
    /// as a non-reference-like <see cref="Type"/>
    /// </summary>
    /// <param name="parameter"></param>
    /// <returns></returns>
    [return: NotNullIfNotNull(nameof(parameter))]
    public static Type? GetNonRefType(this ParameterInfo? parameter)
    {
        if (parameter is null) return null;
        var parameterType = parameter.ParameterType;
        if (parameterType.IsByRef || parameterType.IsPointer)
        {
            parameterType = parameterType.GetElementType();
            Debug.Assert(parameterType != null);
        }
        return parameterType!;
    }

    /// <summary>
    /// Does this <paramref name="parameter"/> have <c>params</c> applied to it?
    /// </summary>
    /// <param name="parameter"></param>
    /// <returns></returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool IsParams(this ParameterInfo parameter) 
        => Attribute.IsDefined(parameter, typeof(ParamArrayAttribute), true);

    /// <summary>
    /// Is this <see cref="ParameterInfo"/> for an <see cref="object"/> <see cref="Array"/>?
    /// </summary>
    public static bool IsObjectArray(this ParameterInfo parameter)
    {
        return !parameter.IsIn &&
               !parameter.IsOut &&
               parameter.ParameterType == typeof(object[]);
    }
}