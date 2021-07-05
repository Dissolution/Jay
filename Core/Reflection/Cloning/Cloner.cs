using System;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using System.Runtime.CompilerServices;
using Jay.Collections;
using Jay.Debugging;
using Jay.Reflection.Emission;
using Jay.Reflection.Runtime;

namespace Jay.Reflection.Cloning
{
    public static class Cloner
    {
        [return: NotNullIfNotNull("value")]
        private delegate T? CloneDelegate<T>([AllowNull] T? value);

        private record CloneMethods(Delegate CloneValue, CloneDelegate<object> CloneObject);

        private static readonly ConcurrentTypeCache<CloneMethods> _cloneMethods;
        private static readonly MethodInfo _getUninitializedObjectMethod;

        static Cloner()
        {
            _cloneMethods = new ConcurrentTypeCache<CloneMethods>();
            _getUninitializedObjectMethod = typeof(RuntimeHelpers)
                                            .GetMethod(nameof(RuntimeHelpers.GetUninitializedObject),
                                                       Reflect.StaticFlags)
                                            .ThrowIfNull(exceptionMessage:
                                                         $"Cannot find {nameof(RuntimeHelpers.GetUninitializedObject)}");
        }

        private static MethodInfo GetCloneValueMethod(Type valueType)
        {
            return typeof(Cloner).GetMethods(BindingFlags.Public | BindingFlags.Static)
                                 .Where(method => method.Name == nameof(Clone))
                                 .Where(method => method.IsGenericMethod)
                                 .OneOrDefault()
                                 .ThrowIfNull()
                                 .MakeGenericMethod(valueType);
        }

        [return: NotNullIfNotNull("value")]
        public static T? Clone<T>([AllowNull] T? value)
        {
            if (value is null) return default(T);
            var methods = _cloneMethods.GetOrAdd(typeof(T), CreateCloneMethods);
            var cloner = methods.CloneValue as CloneDelegate<T>;
            if (cloner is null)
                throw new InvalidOperationException();
            return cloner(value)!;
        }

        [return: NotNullIfNotNull("value")]
        public static object? Clone([AllowNull] object? value)
        {
            if (value is null) return null;
            var methods = _cloneMethods.GetOrAdd(value.GetType(), CreateCloneMethods);
            return methods.CloneObject(value)!;
        }
        
        private static CloneMethods CreateCloneMethods(Type type)
        {
            var delegateType = typeof(CloneDelegate<>).MakeGenericType(type);
            var cloneValueDelegate = DelegateBuilder.Generate(delegateType, generator =>
            {
                // We don't have to check for null as that's done on the Cloner level already
                
                // If we know it's a struct and only contains structs (unmanaged) or is a string,
                // we can just return it as it will be copied.
                if (!type.IsReferenceOrContainsReferences() || 
                    type == typeof(string) ||
                    type == typeof(Type))
                {
                    generator.Ldarg(0).Ret();
                    return;
                }
                
                // We need to construct a new T to hydrate
                generator.DeclareLocal(type, out var newValue);
                
                // Array
                if (type.IsArray(out var elementType))
                {
                    generator.Ldarg(0)
                             .Ldlen()
                             .DeclareLocal<int>(out var len)
                             .Stloc(len)
                             .Ldloc(len)
                             .Newarr(elementType)
                             .Stloc(newValue);
                    var cloneElementMethod = GetCloneValueMethod(elementType);

                    generator.DeclareLocal<int>(out var i)
                             .Ldc_I4(0)
                             .Stloc(i)
                             .Br(out Label check);

                    generator.DefineAndMarkLabel(out Label entry)
                             .Ldloc(newValue)
                             .Ldloc(i)
                             .Ldarg(0)
                             .Ldloc(i)
                             .Ldelem(elementType)
                             .Call(cloneElementMethod)
                             .Stelem(elementType)
                             .Ldloc(i)
                             .Ldc_I4(1)
                             .Add()
                             .Stloc(i);

                    generator.MarkLabel(check)
                             .Ldloc(i)
                             .Ldloc(len)
                             .Clt()
                             .Brtrue(entry);

                    /*
                    int length = 3;
                    object[] array = new object[length];
                    object[] source = new object[length];
                    for (var i = 0; i < length; i++)
                    {
                        array[i] = Clone(source[i]);
                    }
                    
                     // [93 26 - 93 35]
      IL_00a2: ldc.i4.0
      IL_00a3: stloc.s      i

      IL_00a5: br.s         IL_00be
      // start of loop, entry point: IL_00be

        // [94 21 - 94 22]
        IL_00a7: nop

        // [95 25 - 95 53]
        IL_00a8: ldloc.s      'array'
        IL_00aa: ldloc.s      i
        IL_00ac: ldloc.s      source
        IL_00ae: ldloc.s      i
        IL_00b0: ldelem.ref
        IL_00b1: call         object Jay.Reflection.Cloner::Clone(object)
        IL_00b6: stelem.ref

        // [96 21 - 96 22]
        IL_00b7: nop

        // [93 49 - 93 52]
        IL_00b8: ldloc.s      i
        IL_00ba: ldc.i4.1
        IL_00bb: add
        IL_00bc: stloc.s      i

        // [93 37 - 93 47]
        IL_00be: ldloc.s      i
        IL_00c0: ldloc.s      length
        IL_00c2: clt
        IL_00c4: stloc.s      V_10

        IL_00c6: ldloc.s      V_10
        IL_00c8: brtrue.s     IL_00a7
      // end of loop
      */

                }
                else
                {
                    // If it has a new(), use it
                    var constructor = type.GetConstructor(Reflect.InstanceFlags, null, Type.EmptyTypes, null);
                    if (constructor != null)
                    {
                        generator.Newobj(constructor)
                                 .Stloc(newValue);
                    }
                    else
                    {
                        if (type.IsValueType)
                        {
                            generator.Ldloca(newValue)
                                     .Initobj(type);
                        }
                        else
                        {
                            generator.Ldtoken(type)
                                     .Call(_getUninitializedObjectMethod)
                                     .Stloc(newValue);
                        }
                    }
                    
                    // Copy ALL instance fields using Clone
                    var fields = type.GetFields(Reflect.InstanceFlags);
                    foreach (var field in fields)
                    {
                        // Get the clone method
                        var cloneMethod = GetCloneValueMethod(field.FieldType);
                        // Load the field value, clone it, and store it in newValue.field
                        generator.Ldloc(newValue)   // newValue
                                 .Ldarg(0)          // newValue, value
                                 .Ldfld(field)      // newValue, _ <= value.field
                                 .Call(cloneMethod) // newValue, clone(fieldValue))
                                 .Stfld(field);     // newValue.field = clone
                    }
                }
                
                
                // Finished
                generator.Ldloc(newValue)
                         .Ret();

                var str = generator.ToString();
                Hold.Debug(str);
                return;
            });

            var cloneObjectDelegate = DelegateBuilder.Build<CloneDelegate<object>>(dynamicMethod =>
            {
                var emitter = dynamicMethod.GetEmitter();
                Result result;
                result = MethodAdapter.TryEmitLoadParameter(emitter, dynamicMethod.Parameters[0], type, out _);
                if (result)
                {
                    emitter.Call(GetCloneValueMethod(type));
                    result = MethodAdapter.TryEmitCast(emitter, type, typeof(object));
                    if (result)
                    {
                        emitter.Return();
                    }
                    else
                    {
                        result.ThrowIfFailed();
                    }
                }
                else
                {
                    result.ThrowIfFailed();
                }
            });

            return new CloneMethods(cloneValueDelegate, cloneObjectDelegate);
        }
    }
}

/*


       
            
            // Copy ALL instance fields using Clone
            var fields = type.GetFields(Reflect.InstanceFlags);
            foreach (var field in fields)
            {
                // Get the clone method
                var cloneMethod = Cloner.GetCloneMethod(field.FieldType);
                // Load the field value, clone it, and store it in newValue.field
                emitter.Ldloc(newValue)   // newValue
                       .Ldarg(0)          // newValue, this
                       .Ldfld(field)      // newValue, this.field
                       .Call(cloneMethod) // newValue, clone(this.field)
                       .Stfld(field);     // newValue.field = clone(this.field)
            }

            // Return the clone
            emitter.Ldloc(newValue)
                   .EmitCast(type, typeof(T))
                   .Ret();

            var str = emitter.ToString();
            Hold.Debug(str);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        [return: NotNullIfNotNull("value")]
        public static T Clone([AllowNull] T value) => _clone(value);
    }
}*/