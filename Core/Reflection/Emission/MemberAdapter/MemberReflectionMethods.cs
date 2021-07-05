using System;
using System.Diagnostics.CodeAnalysis;
using System.Reflection;
using System.Runtime.CompilerServices;
using Jay.Debugging;
using Jay.Reflection.Runtime;

namespace Jay.Reflection.Emission
{
    public static class MemberReflectionMethods
    {
        private static readonly MemberDelegateCache _cache = new MemberDelegateCache();

        static MemberReflectionMethods()
        {
            
        }
        
        #region Fields
        public static Getter<TInstance, TValue> CreateGetter<TInstance, TValue>(FieldInfo fieldInfo)
        {
            var dynamicMethod = DelegateBuilder.CreateDynamicMethod<Getter<TInstance, TValue>>($"get_{fieldInfo.Name}");
            var emitter = dynamicMethod.GetEmitter();
            if (!fieldInfo.IsStatic)
            {
                var delegateInfo = MemberDelegateCache.GetInvokeMethod<Getter<TInstance, TValue>>();
                var delegateParams = delegateInfo.GetParameters();
                // Load the instance
                emitter.EmitLoadParameter(new ParameterType(delegateParams[0]),
                                          fieldInfo.GetInstanceAdapterType(),
                                          out _);
            }
            emitter.LoadField(fieldInfo)
                   .EmitCast(fieldInfo.FieldType, typeof(TValue))
                   .Return();
            return dynamicMethod.CreateDelegate();
        }
        
        [return: MaybeNull]
        public static TValue GetValue<TInstance, TValue>(this FieldInfo fieldInfo, 
                                                         [DisallowNull] ref TInstance instance)
        {
            var getter = _cache.GetOrAdd(fieldInfo, field => CreateGetter<TInstance, TValue>(field));
            return getter(ref instance);
        }
        
        public static Setter<TInstance, TValue> CreateSetter<TInstance, TValue>(FieldInfo fieldInfo)
        {
            var dynamicMethod = DelegateBuilder.CreateDynamicMethod<Setter<TInstance, TValue>>($"set_{fieldInfo.Name}");
            var delegateInfo = MemberDelegateCache.GetInvokeMethod<Setter<TInstance, TValue>>();
            var delegateParams = delegateInfo.GetParameters();
            var emitter = dynamicMethod.GetEmitter();
            if (!fieldInfo.IsStatic)
            {
                // Load the instance
                emitter.EmitLoadParameter(delegateParams[0],
                                          fieldInfo.GetInstanceAdapterType(),
                                          out _);
            }
            // Load the value
            emitter.EmitLoadParameter(delegateParams[1], fieldInfo.FieldType, out _)
                   // Set the field
                   .StoreField(fieldInfo)
                   .Return();
            return dynamicMethod.CreateDelegate();
        }
        
        public static void SetValue<TInstance, TValue>(this FieldInfo fieldInfo, 
                                                       [DisallowNull] ref TInstance instance, [AllowNull] TValue value)
        {
            var setter = _cache.GetOrAdd(fieldInfo, field => CreateSetter<TInstance, TValue>(field));
            setter(ref instance, value);
        }
        #endregion
        
        #region Properties
        public static Getter<TInstance, TValue> CreateGetter<TInstance, TValue>(PropertyInfo propertyInfo)
        {
            MethodInfo? getMethod = propertyInfo.GetGetMethod(false);
            if (getMethod is null)
            {
                getMethod = propertyInfo.GetGetMethod(true);
                if (getMethod is null)
                {
                    var backingField = propertyInfo.GetBackingField();
                    if (backingField is null)
                        throw new InvalidOperationException($"Cannot create a Getter for {propertyInfo}");
                    return CreateGetter<TInstance, TValue>(backingField);
                }
            }

            return CreateAdapter<Getter<TInstance, TValue>>(getMethod);
        }
        
        [return: MaybeNull]
        public static TValue GetValue<TInstance, TValue>(this PropertyInfo propertyInfo, 
                                                         [DisallowNull] ref TInstance instance)
        {
            var getter = _cache.GetOrAdd(propertyInfo, property => CreateGetter<TInstance, TValue>(property));
            return getter(ref instance);
        }
        
        public static Setter<TInstance, TValue> CreateSetter<TInstance, TValue>(PropertyInfo propertyInfo)
        {
            MethodInfo? setMethod = propertyInfo.GetSetMethod(false);
            if (setMethod is null)
            {
                setMethod = propertyInfo.GetSetMethod(true);
                if (setMethod is null)
                {
                    var backingField = propertyInfo.GetBackingField();
                    if (backingField is null)
                        throw new InvalidOperationException($"Cannot create a Setter for {propertyInfo}");
                    return CreateSetter<TInstance, TValue>(backingField);
                }
            }
            return CreateAdapter<Setter<TInstance, TValue>>(setMethod);
        }
        
        public static void SetValue<TInstance, TValue>(this PropertyInfo propertyInfo, 
                                                       [DisallowNull] ref TInstance instance, 
                                                       [AllowNull] TValue value)
        {
            var setter = _cache.GetOrAdd(propertyInfo, property => CreateSetter<TInstance, TValue>(property));
            setter(ref instance, value);
        }
#endregion
        
        #region Events

        public static Adder<TInstance, THandler> CreateAddHandler<TInstance, THandler>(EventInfo eventInfo,
                                                                                       [DisallowNull] ref TInstance instance,
                                                                                       [DisallowNull] THandler eventHandler)
            where THandler : Delegate
        {
            if (eventInfo is null) throw new ArgumentNullException(nameof(eventInfo));
            MethodInfo? addMethod = eventInfo.GetAddMethod(false);
            if (addMethod is null)
            {
                addMethod = eventInfo.GetAddMethod(true);
                if (addMethod is null)
                {
                    // TODO: Backing field
                    throw new InvalidOperationException();
                }
            }
            return CreateAdapter<Adder<TInstance, THandler>>(addMethod);
        }
        
        public static Remover<TInstance, THandler> CreateRemoveHandler<TInstance, THandler>(EventInfo eventInfo,
            [DisallowNull] ref TInstance instance,
            [DisallowNull] THandler eventHandler)
            where THandler : Delegate
        {
            if (eventInfo is null) throw new ArgumentNullException(nameof(eventInfo));
            MethodInfo? removeMethod = eventInfo.GetRemoveMethod(false);
            if (removeMethod is null)
            {
                removeMethod = eventInfo.GetRemoveMethod(true);
                if (removeMethod is null)
                {
                    // TODO: Backing field
                    throw new InvalidOperationException();
                }
            }
            return CreateAdapter<Remover<TInstance, THandler>>(removeMethod);
        }
        
        public static Raiser<TInstance> CreateRaiser<TInstance>(EventInfo eventInfo)
        {
            if (eventInfo is null) throw new ArgumentNullException(nameof(eventInfo));
            MethodInfo? raiseMethod = eventInfo.GetRaiseMethod(false);
            if (raiseMethod is null)
            {
                raiseMethod = eventInfo.GetRaiseMethod(true);
                if (raiseMethod is null)
                {
                    // TODO: Backing field
                    throw new InvalidOperationException();
                }
            }
            return CreateAdapter<Raiser<TInstance>>(raiseMethod);
        }
#endregion
        
        #region Constructors
        public static Constructor<TInstance> CreateConstructor<TInstance>(ConstructorInfo constructorInfo)
        {
            return CreateAdapter<Constructor<TInstance>>(constructorInfo);
        }

        [return: NotNull]
        public static TInstance CreateInstance<TInstance>(this ConstructorInfo constructorInfo,
                                                          params object?[] args)
        {
            var constructor = _cache.GetOrAdd(constructorInfo, ctor => CreateConstructor<TInstance>(ctor));
            return constructor(args);
        }
        #endregion
        
        #region Methods
        public static TDelegate CreateAdapter<TDelegate>(MethodBase method, Safety safety = Safety.Safe)
            where TDelegate : Delegate
        {
            var dynamicMethod = DelegateBuilder.CreateDynamicMethod<TDelegate>($"call_{method.Name}");
            var delegateInfo = MemberDelegateCache.GetInvokeMethod<TDelegate>();
            var emitter = dynamicMethod.GetEmitter();
            var result = MethodAdapter.TryEmitAdapt(emitter,
                                                    delegateInfo,
                                                    method,
                                                    safety);
            result.ThrowIfFailed();
            var str = emitter.ToString();
            Hold.Debug(str);
            
            return dynamicMethod.CreateDelegate()
                                .ThrowIfNull();
        }

        [return: MaybeNull]
        public static TResult Invoke<TInstance, TResult>(this MethodBase method,
                                                         [DisallowNull] ref TInstance instance,
                                                         params object?[] args)
        {
            var adapter = _cache.GetOrAdd(method, meth => CreateAdapter<InstanceFunc<TInstance, TResult>>(meth));
            return adapter(ref instance, args);
        }

        public static TDelegate Adapt<TDelegate>(this MethodBase method, Safety safety = Safety.Safe)
            where TDelegate : Delegate
        {
            return _cache.GetOrAdd(method, meth => CreateAdapter<TDelegate>(meth, safety));
        }
        #endregion
    }
}