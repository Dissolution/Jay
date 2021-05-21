/*using System;
using System.Collections.Concurrent;
using System.Diagnostics;

namespace Jay.Reflection.Emission
{
    public static class ParameterLoader
    {
        #region LoadInstance
        public static Result CanLoadInstance(ParameterSig parameter, Type? instanceType, bool @unsafe = false)
        {
            // If our instance is a null or void, then we can load... by not loading
            if (instanceType is null || instanceType == typeof(void))
                return true;
            
            // If our parameter is null or void it's bad
            // if (parameter is null)
            //     return new ArgumentNullException(nameof(parameter));
            if (parameter.RootType == typeof(void))
                return new ArgumentException("Cannot load a void parameter", nameof(parameter));
           
            // Cannot load an instance from out unless we're unsafe
            if (parameter.Access == Access.Out && !@unsafe)
                return new ArgumentException("An instance cannot be gotten from an out parameter", nameof(parameter));

            // Are we needing a struct instance?
            if (instanceType.IsValueType)
            {
                // If we have that struct
                if (parameter.RootType == instanceType)
                {
                    switch (parameter.Access)
                    {
                        // We have the struct, load it as an address
                        case Access.Default:
                        {
                            return true;
                        }
                        // We have a reference, load it
                        case Access.In:
                        case Access.Out: // if we match this we know we're unsafe and it's okay
                        case Access.Ref:
                        {
                            return true;
                        }
                        default:
                            return new ArgumentException($"Invalid Parameter Passed By: {parameter.Access}", nameof(parameter));
                    }
                }
                
                // We have an object
                if (parameter.RootType == typeof(object))
                {
                    switch (parameter.Access)
                    {
                        // We can unbox it to the value pointer directly
                        case Access.Default:
                        {
                            return true;
                        }
                        case Access.In:
                        case Access.Out: // if we match this we know we're unsafe and it's okay
                        case Access.Ref:
                        {
                            return true;
                        }
                        default:
                            return new ArgumentException($"Invalid Parameter Passed By: {parameter.Access}", nameof(parameter));
                    }
                }

                // Cannot load it (yet)
                return new NotImplementedException();
            }
            // We need a class instance

            // If we have that class
            if (parameter.RootType == instanceType)
            {
                // Load it (don't care if it's byref)
                return true;
            }
            
            // We have an object or implementing class
            if (parameter.RootType == typeof(object) ||
                parameter.RootType.Implements(instanceType))
            {
                return true;
            }

            // Cannot load it (yet)
            return new NotImplementedException();
        }

        public static Result TryLoadInstance(this ILEmitter emitter,
                                                    ParameterSig parameter,
                                                    Type? instanceType,
                                                    bool @unsafe = false)
        {
            if (emitter is null)
                return new ArgumentNullException(nameof(emitter));
            // If our instance is a null or void, then we can load... by not loading anything
            if (instanceType is null || instanceType == typeof(void))
                return true;
            // If our parameter is null or void it's bad
            //if (parameter is null)
            //    return new ArgumentNullException(nameof(parameter));
            if (parameter.RootType == typeof(void))
                return new ArgumentException("Cannot load a void parameter", nameof(parameter));
           
            // Cannot load an instance from out unless we're unsafe
            if (parameter.Access == Access.Out && !@unsafe)
                return new ArgumentException("An instance cannot be gotten from an out parameter", nameof(parameter));

            // Are we needing a struct instance?
            if (instanceType.IsValueType)
            {
                // If we have that struct
                if (parameter.RootType == instanceType)
                {
                    switch (parameter.Access)
                    {
                        // We have the struct, load it as an address
                        case Access.Default:
                        {
                            emitter.Ldarga(parameter.Index);
                            return true;
                        }
                        // We have a reference, load it
                        case Access.In:
                        case Access.Out: // if we match this we know we're unsafe and it's okay
                        case Access.Ref:
                        {
                            emitter.Ldarg(parameter.Index);
                            return true;
                        }
                        default:
                            return new ArgumentException($"Invalid Parameter Passed By: {parameter.Access}", nameof(parameter));
                    }
                }
                
                // We have an object
                if (parameter.RootType == typeof(object))
                {
                    switch (parameter.Access)
                    {
                        // We can unbox it to the value pointer directly
                        case Access.Default:
                        {
                            emitter.Ldarg(parameter.Index)
                                   .Unbox(instanceType);
                            return true;
                        }
                        case Access.In:
                        case Access.Out: // if we match this we know we're unsafe and it's okay
                        case Access.Ref:
                        {
                            emitter.Ldarg(parameter.Index)
                                   .Ldind_Ref()
                                   .Unbox(instanceType);
                            return true;
                        }
                        default:
                            return new ArgumentException($"Invalid Parameter Passed By: {parameter.Access}", nameof(parameter));
                    }
                }

                // Cannot load it (yet)
                return new NotImplementedException();
            }
            // We need a class instance

            // If we have that class
            if (parameter.RootType == instanceType)
            {
                // Load it (don't care if it's byref)
                emitter.Ldarg(parameter.Index);
                return true;
            }
            
            // We have an object or implementing class
            if (parameter.RootType == typeof(object) ||
                parameter.RootType.Implements(instanceType))
            {
                // Load it
               emitter.Ldarg(parameter.Index);
                    
                // Make sure it's an object and not a ref
                if (parameter.Access != Access.Default)
                {
                    emitter.Ldind_Ref();
                }
                    
                // Cast it
                emitter.Castclass(instanceType);
                return true;
            }

            // Cannot load it (yet)
            return new NotImplementedException();
        }
        #endregion
        
        #region LoadArgument
        public static Result CanLoadArgument(ParameterSig sourceParam, ParameterSig? destParam, bool @unsafe = false)
        {
            throw new NotImplementedException();
        }

        public static Result TryLoadArgument(this ILEmitter emitter,
                                             ParameterSig sourceParam,
                                             ParameterSig destParam,
                                             bool @unsafe = false)
        {
            if (emitter is null)
                return new ArgumentNullException(nameof(emitter));
            // If our destParam is null or void, then we can load... by not loading anything
            if (/*destParam is null || #1#destParam.RootType == typeof(void))
                return true;
            // If our source parameter is null or void it's bad
            //if (sourceParam is null)
            //    return new ArgumentNullException(nameof(sourceParam));
            if (sourceParam.RootType == typeof(void))
                return new ArgumentException("Cannot load a void parameter", nameof(sourceParam));
            
            // If we have an exact T:T, use it
            if (sourceParam.RootType == destParam.RootType &&
                sourceParam.Access == destParam.Access)
            {
                emitter.Ldarg(sourceParam.Index);
                return true;
            }
            
            // Do we need to end up with a struct?
            if (destParam.IsValueType)
            {
                // ?struct -> ?struct
                if (sourceParam.RootType == destParam.RootType)
                {
                    // Map
                    switch (sourceParam.Access, destParam.Access)
                    {
                        case (Access.Default, Access.Default):
                        {
                            // struct -> struct
                            emitter.Ldarg(sourceParam.Index);
                            return true;
                        }
                        case (Access.Default, Access.In):
                        case (Access.Default, Access.Ref):
                        {
                            // struct -> &struct
                            emitter.Ldarga(sourceParam.Index);
                            return true;
                        }
                        case (Access.In, Access.Default):
                        case (Access.Ref, Access.Default):
                        {
                            // &struct -> struct
                            emitter.Ldarg(sourceParam.Index)
                                   .Ldind(destParam.RootType);
                            return true;
                        }
                        case (Access.In, Access.In):
                        case (Access.In, Access.Ref) when @unsafe:
                        case (Access.In, Access.Out) when @unsafe:
                        case (Access.Ref, Access.In):
                        case (Access.Ref, Access.Ref):
                        case (Access.Ref, Access.Out): // This mapping is okay
                        case (Access.Out, Access.Ref) when @unsafe:
                        case (Access.Out, Access.Out):
                        {
                            // &struct -> &struct
                            emitter.Ldarg(sourceParam.Index);
                            return true;
                        }
                        default:
                        {
                            // Incompatible signatures (technically)
                            return new InvalidOperationException($"Cannot convert from a '{sourceParam}' to a '{destParam}' parameter");
                        }
                    }
                }
                
                // ?object -> ?struct
                if (sourceParam.RootType == typeof(object))
                {
                    // Map
                    switch (sourceParam.Access, destParam.Access)
                    {
                        case (Access.Default, Access.Default):
                        {
                            // object -> struct
                            emitter.Ldarg(sourceParam.Index)
                                   .Unbox_Any(destParam.RootType);
                            return true;
                        }
                        case (Access.In, Access.Default):
                        case (Access.Ref, Access.Default):
                        {
                            // &object -> object -> struct
                            emitter.Ldarg(sourceParam.Index)
                                   .Ldind_Ref()
                                   .Unbox_Any(destParam.RootType);
                            return true;
                        }
                        case (Access.Default, Access.In):
                        case (Access.Default, Access.Ref):
                        {
                            // object -> &struct
                            emitter.Ldarg(sourceParam.Index)
                                   .Unbox(destParam.RootType);
                            return true;
                        }
                        case (Access.In, Access.In):
                        case (Access.In, Access.Ref) when @unsafe:
                        case (Access.In, Access.Out) when @unsafe:
                        case (Access.Ref, Access.In):
                        case (Access.Ref, Access.Ref):
                        case (Access.Ref, Access.Out): // This mapping is okay
                        case (Access.Out, Access.Ref) when @unsafe:
                        case (Access.Out, Access.Out):
                        {
                            // &object -> &struct
                            emitter.Ldarg(sourceParam.Index)
                                   .Ldind_Ref()
                                   .Unbox(destParam.RootType);
                            return true;
                        }
                        default:
                        {
                            // Incompatible signatures (technically)
                            return new InvalidOperationException($"Cannot convert from a '{sourceParam}' to a '{destParam}' parameter");
                        }
                    }
                }
                // non-struct, non-object
                else
                {
                    // look for an implicit or explicit conversion
                    
                }
                
                
                
                return new NotImplementedException();
            }
            
            // To object?
            if (destParam.RootType == typeof(object))
            {
                // struct?
                if (sourceParam.IsValueType)
                {
                    // Map
                    switch (sourceParam.Access, destParam.Access)
                    {
                        case (Access.Default, Access.Default):
                        {
                            // struct -> object
                            emitter.Ldarg(sourceParam.Index)
                                   .Box(sourceParam.RootType);
                            return true;
                        }
                        case (Access.Default, Access.In):
                        case (Access.Default, Access.Ref):
                        {
                            // struct -> object -> &object
                            emitter.Ldarg(sourceParam.Index)
                                   .Box(sourceParam.RootType)
                                   .Nop();
                            return true;
                        }
                        case (Access.In, Access.Default):
                        case (Access.Ref, Access.Default):
                        {
                            // &struct -> struct -> object
                            emitter.Ldarg(sourceParam.Index)
                                   .Ldind(sourceParam.RootType)
                                   .Box(sourceParam.RootType);
                            return true;
                        }
                        case (Access.In, Access.In):
                        case (Access.In, Access.Ref) when @unsafe:
                        case (Access.In, Access.Out) when @unsafe:
                        case (Access.Ref, Access.In):
                        case (Access.Ref, Access.Ref):
                        case (Access.Ref, Access.Out): // This mapping is okay
                        case (Access.Out, Access.Ref) when @unsafe:
                        case (Access.Out, Access.Out):
                        {
                            // &struct -> struct -> object -> &object
                            emitter.Ldarg(sourceParam.Index)
                                   .Ldind(sourceParam.RootType)
                                   .Box(sourceParam.RootType)
                                   .Nop();
                            return true;
                        }
                        default:
                        {
                            // Incompatible signatures (technically)
                            return new InvalidOperationException($"Cannot convert from a '{sourceParam}' to a '{destParam}' parameter");
                        }
                    }
                }
                
                // class type!
                
                throw new NotImplementedException();
                
                
                // switch (sourceParam.Access, destParam.Access)
                // {
                //     case (Access.Default, Access.Default):
                //     {
                //         // object -> struct
                //         emitter.Ldarg(sourceParam.Index)
                //                .Unbox_Any(destParam.Type);
                //         return true;
                //     }
                //     case (Access.In, Access.Default):
                //     case (Access.Ref, Access.Default):
                //     {
                //         // &object -> object -> struct
                //         emitter.Ldarg(sourceParam.Index)
                //                .Ldind_Ref()
                //                .Unbox_Any(destParam.Type);
                //         return true;
                //     }
                //     case (Access.Default, Access.In):
                //     case (Access.Default, Access.Ref):
                //     {
                //         // object -> &struct
                //         emitter.Ldarg(sourceParam.Index)
                //                .Unbox(destParam.Type);
                //         return true;
                //     }
                //     case (Access.In, Access.In):
                //     case (Access.In, Access.Ref) when @unsafe:
                //     case (Access.In, Access.Out) when @unsafe:
                //     case (Access.Ref, Access.In):
                //     case (Access.Ref, Access.Ref):
                //     case (Access.Ref, Access.Out): // This mapping is okay
                //     case (Access.Out, Access.Ref) when @unsafe:
                //     case (Access.Out, Access.Out):
                //     {
                //         // &object -> &struct
                //         emitter.Ldarg(sourceParam.Index)
                //                .Ldind_Ref()
                //                .Unbox(destParam.Type);
                //         return true;
                //     }
                //     default:
                //     {
                //         // Incompatible signatures (technically)
                //         return new InvalidOperationException($"Cannot convert from a '{sourceParam}' to a '{destParam}' parameter");
                //     }
                // }
                //
                // // non-struct, non-object
                // return new NotImplementedException();
            }
            
            // Class instance
            else
            {
                throw new NotImplementedException();
            }
        }
        #endregion
    }
}*/