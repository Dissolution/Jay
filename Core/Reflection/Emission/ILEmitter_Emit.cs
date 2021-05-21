using Jay.Collections;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace Jay.Reflection.Emission
{
    public partial class ILEmitter
    {
        #region EmitConvert

        // public Result TryConvert(Type stackType, Type destType, bool @unsafe = false)
        // {
        //     // Look through Unmanaged conversions
        //     if (Unmanaged.IsUnmanaged(stackType) &&
        //         Unmanaged.IsUnmanaged(destType))
        //     {
        //         if (destType == typeof(IntPtr))
        //         {
        //             return new NotImplementedException();
        //         }
        //         else if (@unsafe && destType == typeof(byte))
        //         {
        //         
        //         }
        //     }
        //     
        // }
        #endregion

        /*
        private readonly ConcurrentTypeCache<MethodInfo> _equalsDefaultCache = new ConcurrentTypeCache<MethodInfo>();
        
        public ILEmitter EmitEquals<T>() => EmitEquals(typeof(T));

        public ILEmitter EmitEquals(Type stackType)
        {
            if (stackType is null)
                throw new ArgumentNullException(nameof(stackType));
            switch (Type.GetTypeCode(stackType))
            {
                case TypeCode.Boolean:
                case TypeCode.Char:
                case TypeCode.SByte:
                case TypeCode.Byte:
                case TypeCode.Int16:
                case TypeCode.UInt16:
                case TypeCode.Int32:
                case TypeCode.UInt32:
                case TypeCode.Int64:
                case TypeCode.UInt64:
                case TypeCode.Single:
                case TypeCode.Double:
                {
                    return Ceq();
                }
            }

            var method = _equalsDefaultCache.GetOrAdd(stackType, type => typeof(EqualityComparer).GetMethod(nameof(EqualityComparer.Equals),
                                                                                                            Reflect.StaticFlags)
                                                                                                 .ThrowIfNull()
                                                                                                 .MakeGenericMethod(type));
            return Call(method);
        }
        
        
        public ILEmitter EmitLoadInstance(ParameterSig parameter, Type? instanceType)
        {
            TryLoadInstance(parameter, instanceType).ThrowIfFailed();
            return this;
        }
        
        /// <summary>
        /// Tries to load the given <paramref name="parameter"/> as an instance of the given <paramref name="instanceType"/>.
        /// </summary>
        /// <param name="parameter">The instance parameter to try to load.</param>
        /// <param name="instanceType">The type of instance that the method is expecting.</param>
        /// <returns>A <see cref="Result"/> that describes whether or not the loading succeeded.</returns>
        public Result TryLoadInstance(ParameterSig parameter, Type? instanceType)
        {
            if (parameter.RootType == typeof(void))
                return new ArgumentException("Cannot load a void parameter", nameof(parameter));
            if (instanceType is null || instanceType == typeof(void))
                return new ArgumentNullException(nameof(instanceType));
            // If our instance is static, we can't load it
            if (instanceType.IsStatic())
                return new ArgumentException("Cannot load anything into a static instance", nameof(instanceType));
            // out parameters can never be an instance
            if (parameter.Access == Access.Out)
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
                            Ldarga(parameter.Index);
                            return true;
                        }
                        // We have a reference, load it
                        case Access.In:
                        case Access.Ref:
                        {
                            Ldarg(parameter.Index);
                            return true;
                        }
                        default:
                            return new ArgumentException($"Invalid Parameter Passed By: {parameter.Access}", nameof(parameter));
                    }
                }
                // We have an object
                else if (parameter.RootType == typeof(object))
                {
                    switch (parameter.Access)
                    {
                        // We can unbox it to the value pointer directly
                        case Access.Default:
                        {
                            Ldarg(parameter.Index).Unbox(instanceType);
                            return true;
                        }
                        case Access.In:
                        case Access.Ref:
                        {
                            Ldarg(parameter.Index)
                                   .Ldind_Ref()
                                   .Unbox(instanceType);
                            return true;
                        }
                        default:
                            return new ArgumentException($"Invalid Parameter Passed By: {parameter.Access}", nameof(parameter));
                    }
                }
                else
                {
                    return new NotImplementedException();
                }
            }
            // We need a class instance
            else
            {
                // If we have that class
                if (parameter.RootType == instanceType)
                {
                    // Load it (don't care if it's byref)
                    Ldarg(parameter.Index);
                }
                // We have an object or implementing class
                else if (parameter.RootType == typeof(object) ||
                         parameter.RootType.Implements(instanceType))
                {
                    // Load it
                    Ldarg(parameter.Index);
                    
                    // Make sure it's an object and not a ref
                    if (parameter.Access.HasAnyFlags(Access.In, Access.Ref))
                    {
                        Ldind_Ref();
                    }
                    
                    // Cast it
                    Castclass(instanceType);
                }
                else
                {
                    return new NotImplementedException();
                }
            }
            
            // All fails would have exited early
            return true;
        }

        public ILEmitter EmitLoadArgument(ParameterSig source, ParameterSig dest)
        {
            TryLoadArgument(source, dest).ThrowIfFailed();
            return this;
        }
        
         /// <summary>
        /// Tries to load the given <paramref name="source"/> parameter to fulfill the requirements of the <paramref name="dest"/>
        /// parameter.
        /// </summary>
        /// <param name="source">The source parameter to try to load.</param>
        /// <param name="dest">The destination parameter the method is expecting.</param>
        /// <returns>A <see cref="Result"/> that describes whether or not the loading succeeded.</returns>
        public Result TryLoadArgument(ParameterSig source, ParameterSig dest)
        {
            if (source.RootType == typeof(void))
                return new ArgumentException("Invalid source Type: void", nameof(source));
            if (dest.RootType == typeof(void))
                return new ArgumentException("Invalid dest Type: void", nameof(dest));
           
            // If we have an exact 1:1, just use it and move on
            if (source.RootType == dest.RootType &&
                source.Access == dest.Access)
            {
                Ldarg(source.Index);
                return true;
            }
            
            // Do we need to end up with a struct?
            if (dest.IsValueType)
            {
                // If we have some variation of that struct
                if (source.RootType == dest.RootType)
                {
                    switch (source.Access, dest.Access)
                    {
                        case (Access.Default, Access.Default):
                        {
                            // struct -> struct
                            Ldarg(source.Index);
                            return true;
                        }
                        case (Access.Default, Access.In):
                        case (Access.Default, Access.Ref):
                        {
                            // struct -> &struct
                            Ldarga(source.Index);
                            return true;
                        }
                        case (Access.In, Access.Default):
                        case (Access.Ref, Access.Default):
                        {
                            // &struct -> struct
                            Ldarg(source.Index)
                                   .Ldind(dest.RootType);
                            return true;
                        }
                        case (Access.In, Access.In):
                        case (Access.In, Access.Ref):
                        case (Access.Out, Access.Out):
                        case (Access.Ref, Access.In):
                        case (Access.Ref, Access.Out):    // This mapping is okay
                        case (Access.Ref, Access.Ref):
                        {
                            // &struct -> &struct
                            Ldarg(source.Index);
                            return true;
                        }
                        case (Access.Default, Access.Out):
                        case (Access.In, Access.Out):
                        case (Access.Out, Access.Default):
                        case (Access.Out, Access.In):
                        case (Access.Out, Access.Ref):
                        default:
                        {
                            // Incompatible signatures (technically)
                            return new InvalidOperationException($"Cannot convert from a '{source}' to a '{dest}' parameter");
                        }
                    }
                }
               
                // We have an object parameter
                if (source.RootType == typeof(object))
                {
                    switch (source.Access, dest.Access)
                    {
                        case (Access.Default, Access.Default):
                        {
                            // object -> struct
                            Ldarg(source.Index)
                                   .Unbox_Any(dest.RootType);
                            return true;
                        }
                        case (Access.In, Access.Default):
                        case (Access.Ref, Access.Default):
                        {
                            // &object -> object -> struct
                            Ldarg(source.Index)
                                   .Ldind_Ref()
                                   .Unbox_Any(dest.RootType);
                            return true;
                        }
                        case (Access.Default, Access.In):
                        case (Access.Default, Access.Ref):
                        {
                            // object -> &struct
                            Ldarg(source.Index)
                                   .Unbox(dest.RootType);
                            return true;
                        }
                        case (Access.In, Access.In):
                        case (Access.In, Access.Ref):
                        case (Access.Out, Access.Out):
                        case (Access.Ref, Access.Out):
                        {
                            // &object -> &struct
                            Ldarg(source.Index)
                                   .Ldind_Ref()
                                   .Unbox(dest.RootType);
                            return true;
                        }
                        default:
                        {
                            return new NotImplementedException();
                        }
                    }
                }
                // Non object, non struct-Type
                else
                {
                    return new NotImplementedException();
                }
            }
            // We need a class instance (dest.IsClass)
            else
            {
                // If we have some variation of that class
                if (source.RootType == dest.RootType)
                {
                    switch (source.Access, dest.Access)
                    {
                        case (Access.Default, Access.Default):
                        {
                            // class -> class
                            Ldarg(source.Index);
                            return true;
                        }
                        case (Access.Default, Access.In):
                        case (Access.Default, Access.Ref):
                        {
                            // class -> &class
                            Ldarga(source.Index);
                            return true;
                        }
                        case (Access.In, Access.Default):
                        case (Access.Ref, Access.Default):
                        {
                            // &class -> class
                            Ldarg(source.Index)
                                   .Ldind(dest.RootType)
                                   .Castclass(dest.RootType);
                            return true;
                        }
                        case (Access.In, Access.In):
                        case (Access.In, Access.Ref):
                        case (Access.Out, Access.Out):
                        case (Access.Ref, Access.In):
                        case (Access.Ref, Access.Out):    // This mapping is okay
                        case (Access.Ref, Access.Ref):
                        {
                            // &class -> &class
                            Ldarg(source.Index);
                            return true;
                        }
                        case (Access.Default, Access.Out):
                        case (Access.In, Access.Out):
                        case (Access.Out, Access.Default):
                        case (Access.Out, Access.In):
                        case (Access.Out, Access.Ref):
                        default:
                        {
                            // Incompatible signatures (technically)
                            return new InvalidOperationException($"Cannot convert from a '{source}' to a '{dest}' parameter");
                        }
                    }
                }
                else
                {
                    return new NotImplementedException();
                }
            }
            
            // All fails would have exited early
            //return true;
        }


         internal Result TryLoadParams(ParameterSig paramsSig,
                                       MethodSig methodSig)
         {
             Debug.Assert(paramsSig.IsParams);
             Debug.Assert(methodSig != null);

             // If the method requires no parameters, we just ignore the parameter
             if (methodSig.ParameterCount == 0)
                 return true;
             
             // Start with a null check in case anyone did '(object[])null' as a default
             this.Ldarg(paramsSig.Index)
                 .Brtrue(out var notNull)
                 .EmitThrowException<ArgumentNullException>(paramsSig.Name)
                 // Now we know know it's not null. Normally we'd just accept null as object[0], but we already checked
                 // if a zero length was okay above
                 .MarkLabel(notNull)
                 // object[].Length == method.ParameterCount
                 .Ldarg(paramsSig.Index)
                 .Ldlen()
                 .Ldc_I4(methodSig.ParameterCount)
                 .Ceq()
                 .Brtrue(out var lenOkay)
                 // Throw new ArgumentOutOfRangeException(string? paramName, object? actualValue, string? message)
                 .Ldstr(paramsSig.Name)
                 .Ldarg(paramsSig.Index) // automatically will be object
                 // Going to string.Format("message", (object)arg0, (object)arg1)
                 .Ldstr("{0} arguments expected but only {1} provided.")
                 .Ldc_I4(methodSig.ParameterCount)
                 .Box<int>()
                 .Ldarg(paramsSig.Index)
                 .Ldlen()
                 .Box<int>()
                 .Call(typeof(string).GetMethod("Format", new Type[] { typeof(string), typeof(object), typeof(object) }).ThrowIfNull())
                 .Newobj(typeof(ArgumentOutOfRangeException).GetConstructor(new Type[] { typeof(string), typeof(object), typeof(string) }).ThrowIfNull())
                 .Throw()
                 // Everything after here we know that we have the right number of objects
                 .MarkLabel(lenOkay);
             // Now we emit an unrolled loop of the arguments being loaded and converted
             for (var i = 0; i < methodSig.ParameterCount; i++)
             {
                 this.Ldarg(paramsSig.Index)
                     .Ldc_I4(i)
                     .Ldelem_Ref()
                     .EmitCast(typeof(object), methodSig.ParameterTypes[i]);
             }

             return true;
         }
         
         
         
         
         public ILEmitter EmitConvertReturn(Type stackType, Type returnType)
         {
             TryConvertReturn(stackType, returnType).ThrowIfFailed();
             return this;
         }
         
        /// <summary>
        /// Tries to convert the given <paramref name="stackType"/> variable to the required <paramref name="returnType"/>.
        /// </summary>
        /// <param name="stackType">The type of the variable currently on the stack.</param>
        /// <param name="returnType">The type the variable needs to be converted to.</param>
        /// <returns>A <see cref="Result"/> that describes whether or not the conversion succeeded.</returns>
        public Result TryConvertReturn(Type stackType, Type returnType)
        {
            Validate.ReplaceIfNull(ref stackType, typeof(void));
            Validate.ReplaceIfNull(ref returnType, typeof(void));
            
            if (stackType.IsPointer || 
                stackType.IsByRef ||
                stackType.IsByRefLike)
                return new ArgumentException("", nameof(stackType));
            if (returnType.IsPointer || 
                returnType.IsByRef ||
                returnType.IsByRefLike)
                return new ArgumentException("", nameof(returnType));

            if (stackType == returnType)
            {
                // They're the same
            }
            else if (returnType == typeof(void))
            {
                Pop();
            }
            else if (returnType == typeof(object))
            {
                if (stackType.IsValueType)
                {
                    Box(stackType);
                }
                else
                {
                    // All classes are implicitly objects
                }
            }
            else if (stackType == typeof(object))
            {
                if (returnType.IsValueType)
                {
                    Unbox_Any(returnType);
                }
                else
                {
                    Castclass(returnType);
                }
            }
            else if (stackType.Implements(returnType))
            {
                Castclass(returnType);
            }
            else if (returnType == typeof(void))
            {
                Pop();
            }
            else
            {
                return ConversionException.Create(stackType, returnType);
            }
            
            // We exited already for any failures
            return true;
        }
        */
        
        public ILEmitter EmitCast<TIn, TOut>() => EmitCast(typeof(TIn), typeof(TOut));

        public ILEmitter EmitCast(Type? inType, Type? outType)
        {
            //Validate.ReplaceIfNull(ref inType, typeof(void));
            //Validate.ReplaceIfNull(ref outType, typeof(void));
            if (inType == outType)
            {
                return this;
            }
            else if (inType == typeof(object))
            {
                if (outType.IsValueType)
                {
                    return Unbox_Any(outType);
                }
                else
                {
                    return Castclass(outType);
                }
            }
            else if (outType == typeof(object))
            {
                if (inType.IsValueType)
                {
                    return Box(inType);
                }
                else
                {
                    return this;
                }
            }
            else if (inType.Implements(outType))
            {
                return Castclass(outType);
            }
            else
            {
                throw new NotImplementedException();
            }
        }


        private static readonly MethodInfo _runtimeHelpersGetUninitializedObjectMethod =
            typeof(RuntimeHelpers)
                .GetMethod(nameof(RuntimeHelpers.GetUninitializedObject),
                           Reflect.StaticFlags)
            ?? throw new InvalidOperationException("RuntimeHelpers.GetUninitializedObject(Type) cannot be found!");


        public ILEmitter EmitGetUninitialized<T>()
        {
            if (RuntimeHelpers.IsReferenceOrContainsReferences<T>())
            {
                return EmitGetUninitializedObject(typeof(T))
                    .EmitCast<object, T>();
            }
            else
            {
                return DeclareLocal<T>(out var temp)
                       .Ldloca(temp)
                       .Initobj(typeof(T))
                       .Ldloc(temp);
            }
        }
        
        public ILEmitter EmitGetUninitializedObject(Type type)
        {
            if (type is null)
                throw new ArgumentNullException(nameof(type));
            return this.Ldtoken(type)
                       .Call(_runtimeHelpersGetUninitializedObjectMethod);
        }

        private static readonly ConstructorInfo _dbNullCtor = typeof(DBNull)
                                                                  .GetConstructor(Reflect.InstanceFlags, null, Type.EmptyTypes, null) ??
                                                              throw new InvalidOperationException();
        
        // /// <summary>
        // /// Pushes the given <see cref="object"/> <paramref name="value"/> onto the stack.
        // /// </summary>
        // public ILEmitter EmitLoad(object? value)
        // {
        //     if (value is null)
        //         return Ldnull();
        //     switch (value)
        //     {
        //         case bool boolean:
        //             return boolean ? Ldc_I4(1) : Ldc_I4(0);
        //         case byte b:
        //             return Ldc_I4(b);
        //         case sbyte sb:
        //             return Ldc_I4(sb);
        //         case short sh:
        //             return Ldc_I4(sh);
        //         case ushort ush:
        //             return Ldc_I4(ush);
        //         case char c:
        //             return Ldc_I4(c);
        //         case int i:
        //             return Ldc_I4(i);
        //         case uint ui:
        //             return Ldc_I4((int) ui);
        //         case long l:
        //             return Ldc_I8(l);
        //         case ulong ul:
        //             return Ldc_I8((long) ul);
        //         case float f:
        //             return Ldc_R4(f);
        //         case double d:
        //             return Ldc_R8(d);
        //         case string s:
        //             return Ldstr(s);
        //         case DBNull dbNull:
        //             return Newobj(_dbNullCtor);
        //     }
        //
        //
        // }
        
        /// <summary>
        /// Pushes the given <paramref name="value"/> onto the stack.
        /// </summary>
        /// <typeparam name="T">The <see cref="Type"/> of value to load.</typeparam>
        /// <param name="value">The value to load.</param>
        public ILEmitter EmitLoad<T>([AllowNull] T value)
        {
            if (value is null)
                return Ldnull();
            switch (value)
            {
                case bool boolean:
                    return boolean ? Ldc_I4(1) : Ldc_I4(0);
                case byte b:
                    return Ldc_I4(b);
                case sbyte sb:
                    return Ldc_I4(sb);
                case short sh:
                    return Ldc_I4(sh);
                case ushort ush:
                    return Ldc_I4(ush);
                case char c:
                    return Ldc_I4(c);
                case int i:
                    return Ldc_I4(i);
                case uint ui:
                    return Ldc_I4((int) ui);
                case long l:
                    return Ldc_I8(l);
                case ulong ul:
                    return Ldc_I8((long) ul);
                case float f:
                    return Ldc_R4(f);
                case double d:
                    return Ldc_R8(d);
                case string s:
                    return Ldstr(s);
                case DBNull dbNull:
                    return Newobj(_dbNullCtor);
            }

            throw new NotImplementedException();
            
            // // Value Types
            // if (typeof(T).IsValueType)
            // {
            //     throw new NotImplementedException();
            // }
            // else
            // {
            //     // https://stackoverflow.com/questions/4989681/place-an-object-on-top-of-stack-in-ilgenerator
            //     
            //     var valueGCHandle = GCHandle.Alloc(value);
            //     var valuePtr = GCHandle.ToIntPtr(valueGCHandle);
            //
            //     if (IntPtr.Size == 4)
            //     {
            //         Ldc_I4(valuePtr.ToInt32());
            //     }
            //     else
            //     {
            //         Ldc_I8(valuePtr.ToInt64());
            //     }
            //
            //     Ldobj<T>();
            //
            //     // Do this only if you can otherwise ensure that 'inst' outlives the DynamicMethod
            //     valueGCHandle.Free();
            //     
            //     return this;
            // }
        }
        
        
        // /// <summary>
        // /// Emits the instructions to throw an <see cref="Exception"/>.
        // /// </summary>
        // /// <typeparam name="TException">The <see cref="Type"/> of <see cref="Exception"/> to throw.</typeparam>
        // public ILEmitter EmitThrowException<TException>(params object?[] args)
        //     where TException : Exception
        // {
        //     var ctor = typeof(TException).GetConstructors(Reflect.InstanceFlags)
        //                                  .FirstOrDefault(con =>
        //                                  {
        //                                      var conSig = MethodSig.Of(con!);
        //                                      if (conSig.ParameterCount != args.Length)
        //                                          return false;
        //                                      for (var i = 0; i < args.Length; i++)
        //                                      {
        //                                          var argType = args[i]?.GetType() ?? typeof(void);
        //                                          if (conSig.ParameterTypes[i] != argType)
        //                                              return false;
        //                                      }
        //                                      
        //                                      // TODO: DelegateSig.Matches related to what we canconvert!!!!!
        //                                      
        //
        //                                      return true;
        //                                  }, null);
        //     if (ctor is null)
        //         throw new ArgumentException(nameof(TException));
        //     var ctorSig = MethodSig.Of(ctor);
        //     for (var i = 0; i < args.Length; i++)
        //     {
        //         EmitLoad(args[i])
        //             .EmitCast(args[i]?.GetType(), ctorSig.ParameterTypes[i]);
        //     }
        //
        //     return Newobj(ctor).Throw();
        // }
    }
}