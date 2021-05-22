using System;
using System.Linq.Expressions;
using Jay.Expressions;

namespace Jay.Operators
{
	internal static class Operator<TValue, TArg>
	{
		private static Func<TValue, TArg, TValue>? _or;
		public static Func<TValue, TArg, TValue> Or => _or ??= ExpressionBuilder.CreateFunction<TValue, TArg, TValue>(Expression.Or);

		private static Func<TValue, TArg, TValue>? _and;
		public static Func<TValue, TArg, TValue> And => _and ??= ExpressionBuilder.CreateFunction<TValue, TArg, TValue>(Expression.And);

		private static Func<TValue, TArg, TValue>? _xor;
		public static Func<TValue, TArg, TValue> Xor => _xor ??= ExpressionBuilder.CreateFunction<TValue, TArg, TValue>(Expression.ExclusiveOr);

		private static Func<TValue, TArg, TValue>? _add;
		public static Func<TValue, TArg, TValue> Add => _add ??= ExpressionBuilder.CreateFunction<TValue, TArg, TValue>(Expression.Add);

		private static Func<TValue, TArg, TValue>? _subtract;
		public static Func<TValue, TArg, TValue> Subtract => _subtract ??= ExpressionBuilder.CreateFunction<TValue, TArg, TValue>(Expression.Subtract);

		private static Func<TValue, TArg, TValue>? _multiply;
		public static Func<TValue, TArg, TValue> Multiply => _multiply ??= ExpressionBuilder.CreateFunction<TValue, TArg, TValue>(Expression.Multiply);

		private static Func<TValue, TArg, TValue>? _divide;
		public static Func<TValue, TArg, TValue> Divide => _divide ??= ExpressionBuilder.CreateFunction<TValue, TArg, TValue>(Expression.Divide);

		private static Func<TValue, TArg, TValue>? _modulo;
		public static Func<TValue, TArg, TValue> Modulo => _modulo ??= ExpressionBuilder.CreateFunction<TValue, TArg, TValue>(Expression.Modulo);

		private static Func<TValue, TArg, bool>? _equal;
		public static Func<TValue, TArg, bool> Equal => _equal ??= ExpressionBuilder.CreateFunction<TValue, TArg, bool>(Expression.Equal);

		private static Func<TValue, TArg, bool>? _notEqual;
		public static Func<TValue, TArg, bool> NotEqual => _notEqual ??= ExpressionBuilder.CreateFunction<TValue, TArg, bool>(Expression.NotEqual);

		private static Func<TValue, TArg, bool>? _lessThan;
		public static Func<TValue, TArg, bool> LessThan => _lessThan ??= ExpressionBuilder.CreateFunction<TValue, TArg, bool>(Expression.LessThan);

		private static Func<TValue, TArg, bool>? _lessThanOrEqual;
		public static Func<TValue, TArg, bool> LessThanOrEqual => _lessThanOrEqual ??= ExpressionBuilder.CreateFunction<TValue, TArg, bool>(Expression.LessThanOrEqual);

		private static Func<TValue, TArg, bool>? _greaterThan;
		public static Func<TValue, TArg, bool> GreaterThan => _greaterThan ??= ExpressionBuilder.CreateFunction<TValue, TArg, bool>(Expression.GreaterThan);

		private static Func<TValue, TArg, bool>? _greaterThanOrEqual;
		public static Func<TValue, TArg, bool> GreaterThanOrEqual => _greaterThanOrEqual ??= ExpressionBuilder.CreateFunction<TValue, TArg, bool>(Expression.GreaterThanOrEqual);

		private static Func<TValue, TArg>? _convert;
		public static Func<TValue, TArg> Convert => _convert ??= ExpressionBuilder.CreateFunction<TValue, TArg>(body => Expression.Convert(body, typeof(TArg)));

		static Operator()
		{
			
		}
	}
}
