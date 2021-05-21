using Jay.Collections;
using Jay.Debugging;
using Jay.Reflection.Emission;
using System;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Reflection;
using System.Runtime.CompilerServices;

namespace Jay.Reflection
{
    [return: NotNullIfNotNull("value")]
    internal delegate T CloneDelegate<T>([AllowNull] T value);
    
    public static class Cloner
    {
        private static readonly ConcurrentTypeCache<MethodInfo> _cloneMethods;
        private static readonly ConcurrentTypeCache<CloneDelegate<object?>> _objectCloneMethods;

        static Cloner()
        {
            _cloneMethods = new ConcurrentTypeCache<MethodInfo>();
            _objectCloneMethods = new ConcurrentTypeCache<CloneDelegate<object?>>();
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static MethodInfo FindCloneMethod(Type type)
        {
            var genericCloner = typeof(Cloner<>).MakeGenericType(type);
            RuntimeHelpers.RunClassConstructor(genericCloner.TypeHandle);
            return genericCloner.GetMethod("Clone", Reflect.StaticFlags)
                                .ThrowIfNull("Cannot find Cloner<T>.Clone(T value)");
        }

        internal static MethodInfo GetCloneMethod(Type type)
        {
            Debug.Assert(type != null);
            return _cloneMethods.GetOrAdd(type, FindCloneMethod);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static T Clone<T>([AllowNull] T value) => Cloner<T>.Clone(value);

        [return: NotNullIfNotNull("value")]
        public static object? Clone(object? value)
        {
            if (value is null)
            {
                return null;
            }

            var cloner = _objectCloneMethods.GetOrAdd(value.GetType(), BuildObjectCloneDelegate)
                                            .ThrowIfNull("Cannot build Clone(object?) delegate");
            return cloner(value)!;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static CloneDelegate<object?> BuildObjectCloneDelegate(Type type) 
            => DelegateBuilder.Build<CloneDelegate<object?>>(emitter => emitter.Ldarg(0)
                                                                               .UnboxOrCastclass(type)
                                                                               .Call(GetCloneMethod(type))
                                                                               .BoxIfNeeded(type)
                                                                               .Ret());
    }

    public static class Cloner<T>
    {
        private static readonly CloneDelegate<T> _clone;

        static Cloner()
        {
            _clone = DelegateBuilder.Build<CloneDelegate<T>>(EmitCloneMethod)
                                    .ThrowIfNull("Could not emit the clone delegate");
        }

        private static void EmitCloneMethod(ILEmitter emitter)
        {
            var type = typeof(T);
            
            // If we know it's a struct and only contains structs (unmanaged) or is a string,
            // we can just return it as it will be copied.
            if (!RuntimeHelpers.IsReferenceOrContainsReferences<T>() || type == typeof(string))
            {
                emitter.Ldarg(0).Ret();
                return;
            }

            // We need to construct a new T to hydrate
            emitter.DeclareLocal(type, out var newValue);
            // If it has a new(), use it
            var constructor = type.GetConstructor(Reflect.InstanceFlags, null, Type.EmptyTypes, null);
            if (constructor != null)
            {
                emitter.Newobj(constructor);
            }
            else
            {
                // Brute force construct
                emitter.EmitGetUninitializedObject(type)
                       .Castclass(type);
            }

            // Store
            emitter.Stloc(newValue);

            // Special Array handling
            if (type.IsArray)
            {
                emitter.DeclareLocal<int>(out var len);
                
                var elementType = type.GetElementType()
                                      .ThrowIfNull("Array<null>!");
                var cloneMethod = Cloner.GetCloneMethod(elementType);
                // int len = array.Length;
                emitter.Ldarg(0)
                       .Ldlen()
                       .Stloc(len);

                throw new NotImplementedException();

            }
            
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
}