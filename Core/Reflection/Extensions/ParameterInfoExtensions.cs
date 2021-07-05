using System;
using System.Diagnostics;
using System.Reflection;
using ParameterModifier = Jay.Reflection.Emission.ParameterModifier;

namespace Jay.Reflection
{
    public static class ParameterInfoExtensions
    {
        public static bool IsParams(this ParameterInfo? parameterInfo)
        {
            if (parameterInfo is null) return false;
            return Attribute.GetCustomAttribute(parameterInfo, typeof(ParamArrayAttribute)) != null;
        }

        public static ParameterModifier GetParameterModifier(this ParameterInfo parameterInfo)
        {
            var type = parameterInfo.ParameterType;
            if (type.IsByRef || type.IsByRefLike)
            {
                if (parameterInfo.IsIn)
                {
                    return ParameterModifier.In;
                }
                else if (parameterInfo.IsOut)
                {
                    return ParameterModifier.Out;
                }
                else
                {
                    return ParameterModifier.Ref;
                }
            }
            else if (type.IsPointer)
            {
                return ParameterModifier.Pointer;
            }
            else
            {
                Debug.Assert(!parameterInfo.IsIn);
                Debug.Assert(!parameterInfo.IsOut);
                return ParameterModifier.Default;
            }
        }
    }
}