using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using Jay.Comparison;
using Jay.Debugging;
using Jay.Debugging.Dumping;
using Jay.Reflection.Comparison;
using Jay.Reflection.Emission;
using Jay.Reflection.Runtime;

namespace Jay.Reflection
{
    internal sealed class InterfaceWrapperFactory
    {
        private static readonly IComparer<MemberInfo> _memberInfoComparer;

        static InterfaceWrapperFactory()
        {
            _memberInfoComparer = new FuncComparer<MemberInfo>((a, b) => a!.MemberType.CompareTo(b!.MemberType));
        }
        
        private readonly TypeBuilder _typeBuilder;
        private readonly Dictionary<MemberInfo, object> _memberBuilders;
        private readonly HashSet<Type> _implementedTypes;
        private readonly Type _sourceType;
        private readonly Type _destType;
        private FieldBuilder? _baseField;
        private ConstructorBuilder? _constructor;
        private Type? _proxyType;
        
        public InterfaceWrapperFactory(Type sourceType, Type destType)
        {
            _sourceType = sourceType ?? throw new ArgumentNullException(nameof(sourceType));
            _destType = destType ?? throw new ArgumentNullException(nameof(destType));
            if (!_destType.IsInterface)
	            throw new ArgumentException("Can only Wrap Interfaces", nameof(destType));
            var proxyTypeName = Dumper.Format($"({_sourceType}){_destType}_proxy");
            _typeBuilder = Clay.Build.GetTypeBuilder(proxyTypeName,
                                                     TypeAttributes.Public | TypeAttributes.Sealed | TypeAttributes.Class);
            _memberBuilders = new Dictionary<MemberInfo, object>(MemberInfoEqualityComparer.Default);
            _implementedTypes = new HashSet<Type>();
            _baseField = null;
        }
    
        private void ImplementMembers(Type type)
        {
            if (_implementedTypes.Contains(type))
                return;

            //All interfaces
            var interfaces = type.GetInterfaces();
            foreach (var @interface in interfaces)
            {
                ImplementMembers(@interface);
            }

            //All public instance members
            var members = type.GetMembers()//BindingFlags.Public | BindingFlags.Instance)
                              .OrderBy(_memberInfoComparer)
                              .ToList();
            foreach (var member in members)
            {
                if (member is MethodInfo method)
                {
                    var builder = ImplementMethod(method);
                    _memberBuilders.Add(member, builder);
                }
                else if (member is PropertyInfo property)
                {
                    var builder = ImplementProperty(property);
                    _memberBuilders.Add(member, builder);
                }
                else if (member is EventInfo @event)
                {
                    var builder = ImplementEvent(@event);
                    _memberBuilders.Add(member, builder);
                }
                else
                {
                    //Ignore
                }
            }

            //Done
            _implementedTypes.Add(type);
        }
        
        private MethodBuilder ImplementMethod(MethodInfo method)
        {
	        var parameterTypes = method.GetParametersTypes();
			
			//Get the method that will be called upon the base field
			var baseMethod = _sourceType.GetMethod(method.Name, parameterTypes);
			if (baseMethod is null)
			{
				throw new NotImplementedException();
			}

			var methodBuilder = _typeBuilder.DefineMethod(method.Name,
			                                              MethodAttributes.Public | MethodAttributes.Virtual | MethodAttributes.HideBySig | MethodAttributes.Final,
			                                              method.ReturnType,
			                                              parameterTypes);
			var result = MethodAdapter.TryEmitAdapt(methodBuilder.GetEmitter(), 
			                                        methodBuilder,
													baseMethod);
			result.ThrowIfFailed();
			_typeBuilder.DefineMethodOverride(methodBuilder, method);
			//fin
			return methodBuilder;
		}

		private PropertyBuilder ImplementProperty(PropertyInfo property)
		{
			var propertyBuilder = _typeBuilder.DefineProperty(property.Name, 
			                                                  PropertyAttributes.None, 
			                                                  property.PropertyType, 
			                                                  null);

			var getMethod = property.GetGetter();
			if (getMethod != null)
			{
				var getBuilder = _memberBuilders[getMethod] as MethodBuilder;
				if (getBuilder is null)
					throw new InvalidOperationException();
				propertyBuilder.SetGetMethod(getBuilder);
			}

			var setMethod = property.GetSetter();
			if (setMethod != null)
			{
				var setBuilder = _memberBuilders[setMethod] as MethodBuilder;
				if (setBuilder is null)
					throw new InvalidOperationException();
				propertyBuilder.SetSetMethod(setBuilder);
			}

			//Fin
			return propertyBuilder;
		}

		private System.Reflection.Emit.EventBuilder ImplementEvent(EventInfo @event)
		{
			Debug.Assert(@event.EventHandlerType != null);
			
			var eventBuilder = _typeBuilder.DefineEvent(@event.Name, 
			                                       EventAttributes.None,
			                                       @event.EventHandlerType);

			var addMethod = @event.GetAdder();
			if (addMethod != null)
			{
				var addBuilder = _memberBuilders[addMethod] as MethodBuilder;
				if (addBuilder is null)
					throw new InvalidOperationException();
				eventBuilder.SetAddOnMethod(addBuilder);
			}

			var removeMethod = @event.GetRemover();
			if (removeMethod != null)
			{
				var removeBuilder = _memberBuilders[removeMethod] as MethodBuilder;
				if (removeBuilder is null)
					throw new InvalidOperationException();
				eventBuilder.SetRemoveOnMethod(removeBuilder);
			}

			//Fin
			return eventBuilder;
		}
        
        public Type CreateProxyType()
        {
	        if (_proxyType is null)
	        {

		        // The field where we're storing the source instance
		        _baseField = _typeBuilder.DefineField("_base",
		                                              _sourceType,
		                                              FieldAttributes.Private | FieldAttributes.InitOnly);
		        _typeBuilder.AddInterfaceImplementation(_destType);

		        // Constructor that stores into that field
		        _constructor = _typeBuilder.DefineConstructor(MethodAttributes.Public,
		                                                      CallingConventions.Standard,
		                                                      new Type[1] {_sourceType});
		        _constructor.Emit(emitter =>
		        {
			        emitter.LoadArgument(0) // this
			               .LoadArgument(1) // instance
			               .StoreField(_baseField) //this._base = instance
			               .Return();
		        });
		        ImplementMembers(_destType);
		        _proxyType = _typeBuilder.CreateType() ?? throw new InvalidOperationException();
	        }
	        return _proxyType;
        }

        public Delegate CreateProxyTypeConstructor()
        {
	        CreateProxyType();
	        return DelegateBuilder.Generate(typeof(Func<,>).MakeGenericType(_sourceType, _destType),
	                                     gen => gen.Ldarg(0)
	                                               .Newobj(_constructor!)
	                                               .Ret());
        }
        
    }
    
    
    public static partial class Clay
    {
        public static class Proxy
        {
            private static readonly ConcurrentDictionary<(Type, Type), Type> _proxyTypes;

            static Proxy()
            {
                _proxyTypes = new ConcurrentDictionary<(Type, Type), Type>();
            }

            public static TInterface WrapInInterface<T, TInterface>([DisallowNull] T instance)
                where TInterface : class // closest we can require
            {
                var proxyType = _proxyTypes.GetOrAdd((typeof(T), typeof(TInterface)), CreateProxyType);
                // TODO: Use CreateProxyTypeConstructor up above
                return Activator.CreateInstance(proxyType, instance) as TInterface;
            }

            private static Type CreateProxyType((Type Source, Type Dest) types)
            {
	            var wrapper = new InterfaceWrapperFactory(types.Source, types.Dest);
	            return wrapper.CreateProxyType();
            }
        }
    }
}