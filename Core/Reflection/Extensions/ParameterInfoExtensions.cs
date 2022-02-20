using System.Diagnostics;
using System.Reflection;
using System.Runtime.CompilerServices;

namespace Jay.Reflection;

public static class ParameterInfoExtensions
{
    public enum Access
    {
        Default,
        In,
        Ref,
        Out,
    }

    public static Access GetAccess(this ParameterInfo parameter, out Type parameterType)
    {
        parameterType = parameter.ParameterType;
        if (parameterType.IsByRef)
        {
            parameterType = parameterType.GetElementType();
            Debug.Assert(parameterType != null);
            if (parameter.IsIn)
            {
                return Access.In;
            }

            if (parameter.IsOut)
            {
                return Access.Out;
            }

            return Access.Ref;
        }
        return Access.Default;
    }

    [return: NotNullIfNotNull("parameter")]
    public static Type? NonRefType(this ParameterInfo? parameter)
    {
        if (parameter is null) return null;
        var parameterType = parameter.ParameterType;
        if (parameterType.IsByRef || parameterType.IsByRefLike || parameterType.IsPointer)
        {
            parameterType = parameterType.GetElementType();
            Debug.Assert(parameterType != null);
        }
        return parameterType;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool IsParams(this ParameterInfo parameter) 
        => Attribute.IsDefined(parameter, typeof(ParamArrayAttribute), true);

}