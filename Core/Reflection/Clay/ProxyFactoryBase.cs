/*
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using System.Xml;
using Jay.Collections;
using Jay.Comparison;
using Jay.Debugging;

namespace Jay.Reflection.Duck
{
	public class ProxyFactory : IProxyFactory
	{
		private static readonly IComparer<MemberInfo> _memberComparer;

		static ProxyFactory()
		{
			_memberComparer = new FuncBasedComparer<MemberInfo>((a, b) => a.MemberType.CompareTo(b.MemberType));
		}

		protected TypeBuilder _typeBuilder;
		protected FieldBuilder _baseField;
		protected readonly Dictionary<MemberInfo, object> _memberBuilders;
		protected readonly HashSet<Type> _implementedTypes;

		/// <inheritdoc />
		public Type SourceType { get; }

		/// <inheritdoc />
		public Type DestType { get; }

		public ProxyFactory(Type sourceType, Type destType)
		{
			this.SourceType = sourceType ?? throw new ArgumentNullException(nameof(sourceType));
			this.DestType = destType ?? throw new ArgumentNullException(nameof(destType));
			_memberBuilders = new Dictionary<MemberInfo, object>();
			_implementedTypes = new HashSet<Type>();
		}

		protected virtual void AddFields()
		{
			_baseField = _typeBuilder.DefineField("_base", this.SourceType, FieldAttributes.Private | FieldAttributes.InitOnly);
		}

		protected virtual MethodBuilder ImplementMethod(MethodInfo method)
		{
			var parameters = method.GetParameters();
			var parameterCount = parameters.Length;
			var parameterTypes = new Type[parameterCount];
			for (var i = 0; i < parameterCount; i++)
				parameterTypes[i] = parameters[i].ParameterType;

			//Get the method that will be called upon the base field
			var baseMethod = _baseField.FieldType.GetMethod(method.Name, parameterTypes);

			var mAttr = MethodAttributes.Public | MethodAttributes.Virtual | MethodAttributes.HideBySig | MethodAttributes.Final;
			MethodInfo destMethod = null;
			if (!DestType.IsInterface)
			{
				destMethod = this.DestType.GetMethod(method.Name, parameterTypes);
				if (destMethod is null || destMethod.IsVirtual || destMethod.IsAbstract)
				{
					//mAttr = mAttr;
				}
				else 
				{
					mAttr |= MethodAttributes.NewSlot;
				}
				//if (destMethod != null && (destMethod.Attributes & MethodAttributes.SpecialName) != 0)
				//	mAttr |= MethodAttributes.SpecialName;
			}
			else
			{
				//Nothing
				//mAttr = mAttr;
			}
		

			var builder = _typeBuilder.DefineMethod(
				method.Name,
				mAttr,
				method.ReturnType,
				parameterTypes);
			var emit = new AdvancedEmitter(builder.GetILGenerator());
			emit.Ldarg_0() //this
				.Ldfld(_baseField); //load the base field
			//Load all method parameters
			for (var i = 1; i <= parameterCount; i++)
				emit.Ldarg(i);
			//Call the base method
			emit.Callvirt(baseMethod)
				.WriteLine("Overridden")
				//return
				.Ret();
			//Override
			if ((mAttr & MethodAttributes.NewSlot) != 0)
			{
				Hold.Debug(mAttr);
			}
			else
			{
				_typeBuilder.DefineMethodOverride(builder, method);
			}
			//fin
			return builder;
		}

		protected virtual PropertyBuilder ImplementProperty(PropertyInfo property)
		{
			if (!DestType.IsInterface && DestType.GetProperty(property.Name) != null)
				return null;


			var builder = _typeBuilder.DefineProperty(property.Name, PropertyAttributes.None, property.PropertyType, null);

			var getMethod = property.GetGetMethod();
			if (getMethod != null)
			{
				var getBuilder = _memberBuilders[getMethod] as MethodBuilder;
				builder.SetGetMethod(getBuilder);
			}

			var setMethod = property.GetSetMethod();
			if (setMethod != null)
			{
				var setBuilder = _memberBuilders[setMethod] as MethodBuilder;
				builder.SetSetMethod(setBuilder);
			}

			//Fin
			return builder;
		}

		protected virtual EventBuilder ImplementEvent(EventInfo @event)
		{
			var builder = _typeBuilder.DefineEvent(@event.Name, EventAttributes.None,@event.EventHandlerType);

			var addMethod = @event.GetAddMethod();
			var addBuilder = _memberBuilders[addMethod] as MethodBuilder;
			builder.SetAddOnMethod(addBuilder);

			var removeMethod = @event.GetRemoveMethod();
			var removeBuilder = _memberBuilders[removeMethod] as MethodBuilder;
			builder.SetRemoveOnMethod(removeBuilder);

			//Fin
			return builder;
		}
		protected virtual ConstructorBuilder ImplementConstructor()
		{
			var builder = _typeBuilder.DefineConstructor(
				MethodAttributes.Public,
				CallingConventions.Standard,
				new Type[]{this.SourceType});

			var emit = new AdvancedEmitter(builder.GetILGenerator());

			//Call object constructor, as everything is an object
			//emit.Ldarg_0().Call(_objectConstructor);

			//if (!DestType.IsInterface)
			//{
			//	var destCtor = DestType.GetConstructor(Type.EmptyTypes);
			//	if (destCtor != null)
			//	{
			//		emit.Ldarg_0()
			//			.Call(destCtor);
			//	}
			//}

			//Store source in base
			emit.Ldarg_0() //this
				.Ldarg_1() //source
				.Stfld(_baseField);	//store passed source in base field
			emit.Ret();

			//Fin
			return builder;
		}

		protected virtual void ImplementMembers(Type type)
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
				.OrderBy(_memberComparer)
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

		/// <inheritdoc />
		public Type CreateProxyType()
		{
			var proxyTypeName = $"{SourceType.Name}_to_{DestType.Name}_adapter";
			if (this.DestType.IsInterface)
			{
				_typeBuilder = Clay.ModuleBuilder.DefineType(proxyTypeName, TypeAttributes.Public);
				_typeBuilder.AddInterfaceImplementation(this.DestType);
			}
			else
			{
				_typeBuilder = Clay.ModuleBuilder.DefineType(proxyTypeName, TypeAttributes.Public, this.DestType);
			}
			AddFields();
			ImplementConstructor();
			ImplementMembers(this.DestType);
			return _typeBuilder.CreateType();
		}
	}
}
*/
