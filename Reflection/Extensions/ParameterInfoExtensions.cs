﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

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

}