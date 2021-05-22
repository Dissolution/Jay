using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Reflection;
using Jay.Debugging;

namespace Jay.Expressions
{
	/// <summary>
	/// A builder class for <see cref="Expression"/>s.
	/// </summary>
	public static class ExpressionBuilder
	{
		internal static Func<TValue, TReturn> CreateFunction<TValue, TReturn>(
			Func<Expression, UnaryExpression> unaryExpression,
			Type? valueType = null,
			Type? returnType = null)
		{
			var vType = typeof(TValue);
			if (valueType is null)
				valueType = vType;

			var rType = typeof(TReturn);
			if (returnType is null)
				returnType = rType;

			//Value Parameter
			var vParameter = Expression.Parameter(vType, "value");
			//Left Expression might need convert
			Expression left;
			if (vType != valueType)
				left = Expression.Convert(vParameter, valueType);
			else
				left = vParameter;

			//Body
			UnaryExpression unary;
			try
			{
				unary = unaryExpression(left);
			}
			catch (InvalidOperationException ex)
			{
				var message = ex.Message;
				return delegate { throw new InvalidOperationException(message); };
			}
			Expression body;
			if (rType != returnType)
				body = Expression.Convert(unary, rType);
			else
				body = unary;

			//Compile
			var lambda = Expression.Lambda(body, vParameter);
			var func = lambda.Compile<Func<TValue, TReturn>>();
			return func.ThrowIfNull(nameof(lambda), "Could not Compile Lambda");
		}

		internal static Func<T1, T2, TReturn> CreateFunction<T1, T2, TReturn>(
			Func<Expression, Expression, BinaryExpression> binaryExpression,
			Type? value1Type = null,
			Type? value2Type = null,
			Type? returnType = null)
		{
			var t1Type = typeof(T1);
			if (value1Type is null)
				value1Type = t1Type;

			var t2Type = typeof(T2);
			if (value2Type is null)
				value2Type = t2Type;

			var rType = typeof(TReturn);
			if (returnType is null)
				returnType = rType;

			//T1 Parameter
			var v1Parameter = Expression.Parameter(t1Type, "value1");
			//Left Expression might need convert
			Expression first;
			if (t1Type != value1Type)
				first = Expression.Convert(v1Parameter, value1Type);
			else
				first = v1Parameter;

			//T2 Parameter
			var v2Parameter = Expression.Parameter(t2Type, "value2");
			//Left Expression might need convert
			Expression second;
			if (t2Type != value2Type)
				second = Expression.Convert(v2Parameter, value2Type);
			else
				second = v2Parameter;

			//Body
			BinaryExpression binary;
			try
			{
				binary = binaryExpression(first, second);
			}
			catch (InvalidOperationException ex)
			{
				var message = ex.Message;
				return delegate { throw new InvalidOperationException(message); };
			}
			Expression body;
			if (rType != returnType)
				body = Expression.Convert(binary, rType);
			else
				body = binary;

			//Compile
			var lambda = Expression.Lambda(body, v1Parameter, v2Parameter);
			var func = lambda.Compile<Func<T1, T2, TReturn>>();
			return func.ThrowIfNull(nameof(lambda), "Could not Compile Lambda");
		}

		/// <summary>
		/// A base expression set to 'TRUE'.
		/// </summary>
		/// <returns></returns>
		public static Expression<Func<bool>> True()
		{
			return () => true;
		}

		/// <summary>
		/// A base expression set to 'TRUE'.
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <returns></returns>
		public static Expression<Func<T, bool>> True<T>()
		{
			return f => true;
		}

		/// <summary>
		/// A base expression set to 'FALSE'.
		/// </summary>
		/// <returns></returns>
		public static Expression<Func<bool>> False()
		{
			return () => false;
		}

		/// <summary>
		/// A base expression set to 'FALSE'.
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <returns></returns>
		public static Expression<Func<T, bool>> False<T>()
		{
			return f => false;
		}

		// /// <summary>
		// /// Am expression which is the result of the first OR the second expression.
		// /// </summary>
		// /// <typeparam name="T"></typeparam>
		// /// <param name="expr1"></param>
		// /// <param name="expr2"></param>
		// /// <returns></returns>
		// public static Expression<Func<T, bool>> Or<T>(this Expression<Func<T, bool>> expr1, Expression<Func<T, bool>> expr2)
		// {
		// 	if (expr1 is null)
		// 		throw new ArgumentNullException(nameof(expr1));
		// 	if (expr2 is null)
		// 		throw new ArgumentNullException(nameof(expr2));
		//
		// 	var invokedExpr = Expression.Invoke(expr2, expr1.Parameters);
		// 	return Expression.Lambda<Func<T, bool>>
		// 		  (Expression.OrElse(expr1.Body, invokedExpr), expr1.Parameters);
		// }
		//
		// /// <summary>
		// /// An expression which is the result of the first AND the second expression.
		// /// </summary>
		// /// <typeparam name="T"></typeparam>
		// /// <param name="expr1"></param>
		// /// <param name="expr2"></param>
		// /// <returns></returns>
		// public static Expression<Func<T, bool>> And<T>(this Expression<Func<T, bool>> expr1, Expression<Func<T, bool>> expr2)
		// {
		// 	if (expr1 is null)
		// 		throw new ArgumentNullException(nameof(expr1));
		// 	if (expr2 is null)
		// 		throw new ArgumentNullException(nameof(expr2));
		//
		// 	var invokedExpr = Expression.Invoke(expr2, expr1.Parameters);
		// 	return Expression.Lambda<Func<T, bool>>
		// 		  (Expression.AndAlso(expr1.Body, invokedExpr), expr1.Parameters);
		// }
		//
		// /// <summary>
		// /// A predicate which is NOT the result of the specified predicate.
		// /// </summary>
		// /// <typeparam name="T"></typeparam>
		// /// <param name="expr"></param>
		// /// <returns></returns>
		// public static Expression<Func<T, bool>> Not<T>(this Expression<Func<T, bool>> expr)
		// {
		// 	if (expr is null)
		// 		throw new ArgumentNullException(nameof(expr));
		//
		// 	return Expression.Lambda<Func<T, bool>>(Expression.Not(expr.Body), expr.Parameters);
		// }
	}
}
