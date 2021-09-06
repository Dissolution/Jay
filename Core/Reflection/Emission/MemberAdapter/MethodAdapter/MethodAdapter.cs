using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Reflection;
using System.Reflection.Emit;
using Jay.Debugging;

namespace Jay.Reflection.Emission
{
    public static partial class MethodAdapter
    {
        private static readonly object? _nullObject = null;
        private static readonly FieldInfo _nullObjectField;
        
        
        
        static MethodAdapter()
        {
            _nullObjectField = typeof(MethodAdapter)
                               .GetField(nameof(_nullObject), BindingFlags.NonPublic | BindingFlags.Static)
                               .ThrowIfNull();
        }



    

        /* switch (delegateParameter.Modifier, methodParameter.Modifier)
                {
                    case (ParameterModifier.Default, ParameterModifier.Default):
                    case (ParameterModifier.Default, ParameterModifier.In):
                    case (ParameterModifier.Default, ParameterModifier.Ref):
                    case (ParameterModifier.Default, ParameterModifier.Pointer):
                    case (ParameterModifier.Default, ParameterModifier.Out):
                    case (ParameterModifier.In, ParameterModifier.Default):
                    case (ParameterModifier.In, ParameterModifier.In):
                    case (ParameterModifier.In, ParameterModifier.Ref):
                    case (ParameterModifier.In, ParameterModifier.Pointer):
                    case (ParameterModifier.In, ParameterModifier.Out):
                    case (ParameterModifier.Ref, ParameterModifier.Default):
                    case (ParameterModifier.Ref, ParameterModifier.In):
                    case (ParameterModifier.Ref, ParameterModifier.Ref):
                    case (ParameterModifier.Ref, ParameterModifier.Pointer):
                    case (ParameterModifier.Ref, ParameterModifier.Out):
                    case (ParameterModifier.Pointer, ParameterModifier.Default):
                    case (ParameterModifier.Pointer, ParameterModifier.In):
                    case (ParameterModifier.Pointer, ParameterModifier.Ref):
                    case (ParameterModifier.Pointer, ParameterModifier.Pointer):
                    case (ParameterModifier.Pointer, ParameterModifier.Out):
                    case (ParameterModifier.Out, ParameterModifier.Default):
                    case (ParameterModifier.Out, ParameterModifier.In):
                    case (ParameterModifier.Out, ParameterModifier.Ref):
                    case (ParameterModifier.Out, ParameterModifier.Pointer):
                    case (ParameterModifier.Out, ParameterModifier.Out):
                    default:
                    {
                        
                    }
                }
         *
         *
         * 
         */

      
    }
}