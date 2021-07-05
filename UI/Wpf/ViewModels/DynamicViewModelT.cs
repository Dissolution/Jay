/*
using System;
using System.ComponentModel;
using System.Dynamic;
using System.Reflection;
using System.Runtime.CompilerServices;
using Jay.Collections;
using Jay.Comparison;

namespace Jay.UI.WPFExtensions
{
	/// <summary>
	/// A ViewModel that dynamically dispatches property access to a contained model.
	/// </summary>
	/// <typeparam name="T"></typeparam>
	public class DynamicViewModel<T> : DynamicObject, INotifyPropertyChanged
	{
		private static readonly UberDictionary<string, Type> _propertyTypes;
		private static readonly UberDictionary<string, MethodInfo> _propertyGetters;
		private static readonly UberDictionary<string, MethodInfo> _propertySetters;

		static DynamicViewModel()
		{
			_propertyTypes = new UberDictionary<string, Type>(MissingKeyBehavior.ReturnDefaultValue, name => null, TextComparer.OrdinalIgnoreCase);
			_propertyGetters = new UberDictionary<string, MethodInfo>(MissingKeyBehavior.ReturnDefaultValue, name => null, TextComparer.OrdinalIgnoreCase);
			_propertySetters = new UberDictionary<string, MethodInfo>(MissingKeyBehavior.ReturnDefaultValue, name => null, TextComparer.OrdinalIgnoreCase);

			var properties = typeof(T).GetProperties(BindingFlags.Public | BindingFlags.Instance);
			foreach (var property in properties)
			{
				var name = property.Name;
				_propertyTypes[name] = property.PropertyType;
				if (property.CanRead)
					_propertyGetters[name] = property.GetGetMethod();
				if (property.CanWrite)
					_propertySetters[name] = property.GetSetMethod();
			}
		}

		/// <summary>
		/// The Model whose properties we're dynamically accessing.
		/// </summary>
		public T Model { get; }

		/// <summary>
		/// Occurs when a property value changes.
		/// </summary>
		public event PropertyChangedEventHandler PropertyChanged;

		/// <summary>
		/// Gets or sets the value of the Model's property with the specified name.
		/// </summary>
		/// <param name="propertyName"></param>
		/// <returns></returns>
		public object this[string propertyName]
		{
			get
			{
				if (_propertyGetters.TryGetValue(propertyName, out MethodInfo getter))
					return getter.Invoke(Model, null);
				//Fail
				throw new InvalidOperationException($"Could not get Property '{propertyName}'s value");
			}
			set
			{
				if (_propertySetters.TryGetValue(propertyName, out MethodInfo setter))
				{
					//Convert
					if (Converter.TryConvert(value, _propertyTypes[propertyName], out object converted))
					{
						setter.Invoke(Model, new[] {converted});
						return;
					}
				}
				//Fail
				throw new InvalidOperationException($"Could not set Property '{propertyName}' to value '{value}'");
			}
		}

		public DynamicViewModel(T model)
		{
			this.Model = model;
		}

		[NotifyPropertyChangedInvocator]
		protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
		{
			PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
		}

		public override bool TryGetMember(GetMemberBinder binder, out object result)
		{
			//Get property's name + type
			var propertyName = binder.Name;

			if (_propertyGetters.TryGetValue(propertyName, out MethodInfo getter))
			{
				var value = getter.Invoke(Model, null);
				//Convert + return
				return Converter.TryConvert(value, _propertyTypes[propertyName], out result);
			}

			//Failed
			result = null;
			return false;
		}

		public override bool TrySetMember(SetMemberBinder binder, object value)
		{
			//Get property's name + type
			var propertyName = binder.Name;

			if (_propertySetters.TryGetValue(propertyName, out MethodInfo setter))
			{
				//Convert
				if (Converter.TryConvert(value, _propertyTypes[propertyName], out object converted))
				{
					setter.Invoke(Model, new[] { converted });
					return true;
				}
			}
			//Fail
			return false;
		}
	}
}
*/
