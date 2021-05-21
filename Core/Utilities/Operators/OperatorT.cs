using System;
using System.Linq.Expressions;
using Jay.Expressions;

namespace Jay.Operators
{
	internal static class Operator<T>
	{
		private interface INullCheck
		{
			bool IsNotNull(T value);
		}

		private sealed class ClassNullCheck : INullCheck
		{
			public bool IsNotNull(T value) => value != null;
		}

		private sealed class StructNullCheck : INullCheck
		{
			public bool IsNotNull(T value) => true;
		}

		private static readonly Type _type = typeof(T);
		private static readonly INullCheck _nullCheck;

		public static T? Default = default;
		public static T? Zero { get; }
		
		private static Func<T, bool>? _isTrue;
		public static Func<T, bool> IsTrue => _isTrue ??= ExpressionBuilder.CreateFunction<T,bool>(Expression.IsTrue);

		private static Func<T, bool>? _isFalse;
		public static Func<T, bool> IsFalse => _isFalse ??= ExpressionBuilder.CreateFunction<T, bool>(Expression.IsFalse);

		private static Func<T, T>? _negate;
		public static Func<T, T> Negate => _negate ??= ExpressionBuilder.CreateFunction<T, T>(Expression.Negate);

		private static Func<T, T>? _not;
		public static Func<T, T> Not => _not ??= ExpressionBuilder.CreateFunction<T, T>(Expression.Not);

		private static Func<T, T>? _bitwiseComplement;
		public static Func<T, T> BitwiseComplement =>  _bitwiseComplement ??= ExpressionBuilder.CreateFunction<T, T>(Expression.OnesComplement);

		private static Func<T, T>? _increment;
		public static Func<T, T> Increment => _increment ??= ExpressionBuilder.CreateFunction<T, T>(Expression.Increment);

		private static Func<T, T>? _decrement;
		public static Func<T, T> Decrement => _decrement ??= ExpressionBuilder.CreateFunction<T, T>(Expression.Decrement);

		private static Func<T, T, T>? _or;
		public static Func<T, T, T> Or => _or ??= ExpressionBuilder.CreateFunction<T, T, T>(Expression.Or);

		private static Func<T, T, T>? _and;
		public static Func<T, T, T> And => _and ??= ExpressionBuilder.CreateFunction<T, T, T>(Expression.And);

		private static Func<T, T, T>? _xor;
		public static Func<T, T, T> Xor => _xor ??= ExpressionBuilder.CreateFunction<T, T, T>(Expression.ExclusiveOr);

		private static Func<T, T, T>? _add;
		public static Func<T, T, T> Add => _add ??= ExpressionBuilder.CreateFunction<T, T, T>(Expression.Add);

		private static Func<T, T, T>? _subtract;
		public static Func<T, T, T> Subtract => _subtract ??= ExpressionBuilder.CreateFunction<T, T, T>(Expression.Subtract);

		private static Func<T, T, T>? _multiply;
		public static Func<T, T, T> Multiply => _multiply ??= ExpressionBuilder.CreateFunction<T, T, T>(Expression.Multiply);

		private static Func<T, T, T>? _divide;
		public static Func<T, T, T> Divide => _divide ??= ExpressionBuilder.CreateFunction<T, T, T>(Expression.Divide);

		private static Func<T, T, T>? _modulo;
		public static Func<T, T, T> Modulo => _modulo ??= ExpressionBuilder.CreateFunction<T, T, T>(Expression.Modulo);

		private static Func<T, int, T>? _leftShift;
		public static Func<T, int, T> LeftShift => _leftShift ??= ExpressionBuilder.CreateFunction<T, int, T>(Expression.LeftShift);

		private static Func<T, int, T>? _rightShift;
		public static Func<T, int, T> RightShift => _rightShift ??= ExpressionBuilder.CreateFunction<T, int, T>(Expression.RightShift);

		private static Func<T, T, bool>? _equal;
		public static Func<T, T, bool> Equal => _equal ??= ExpressionBuilder.CreateFunction<T, T, bool>(Expression.Equal);

		private static Func<T, T, bool>? _notEqual;
		public static Func<T, T, bool> NotEqual => _notEqual ??= ExpressionBuilder.CreateFunction<T, T, bool>(Expression.NotEqual);

		private static Func<T, T, bool>? _lessThan;
		public static Func<T, T, bool> LessThan => _lessThan ??= ExpressionBuilder.CreateFunction<T, T, bool>(Expression.LessThan);

		private static Func<T, T, bool>? _lessThanOrEqual;
		public static Func<T, T, bool> LessThanOrEqual => _lessThanOrEqual ??= ExpressionBuilder.CreateFunction<T, T, bool>(Expression.LessThanOrEqual);

		private static Func<T, T, bool>? _greaterThan;
		public static Func<T, T, bool> GreaterThan => _greaterThan ??= ExpressionBuilder.CreateFunction<T, T, bool>(Expression.GreaterThan);

		private static Func<T, T, bool>? _greaterThanOrEqual;
		public static Func<T, T, bool> GreaterThanOrEqual => _greaterThanOrEqual ??= ExpressionBuilder.CreateFunction<T, T, bool>(Expression.GreaterThanOrEqual);

		static Operator()
		{
			Type? underlyingType = Nullable.GetUnderlyingType(_type);
			if (underlyingType != null)
			{
				Zero = (T)Activator.CreateInstance(underlyingType)!;
				_nullCheck = new StructNullCheck();
			}
			else
			{
				Zero = default;
				if (_type.IsValueType)
					_nullCheck = new StructNullCheck();
				else
					_nullCheck = new ClassNullCheck();
			}
		}

		public static bool NotNull(T value)
		{
			return _nullCheck.IsNotNull(value);
		}
	}
}
