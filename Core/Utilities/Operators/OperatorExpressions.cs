//using System;
//using System.Linq.Expressions;

//namespace Jay.Operators
//{
//	internal static class OperatorExpressions
//	{
//		public static Func<TValue, TReturn> CreateFunction<TValue, TReturn>(
//			Func<Expression, UnaryExpression> unaryExpression)
//		{
//			return CreateFunction<TValue, TReturn>(unaryExpression, typeof(TValue), typeof(TReturn));
//		}

//		public static Func<TValue, TReturn> CreateFunction<TValue, TReturn>(
//			Func<Expression, UnaryExpression> unaryExpression,
//			Type valueType, Type returnType)
//		{
//			//Value
//			var tValue = typeof(TValue);
//			var pValue = Expression.Parameter(tValue, "value");
//			Expression left;
//			if (tValue != valueType)
//				left = Expression.Convert(pValue, valueType);
//			else
//				left = pValue;

//			//Body
//			var tReturn = typeof(TReturn);
//			Expression body;
//			try
//			{
//				var unary = unaryExpression(left);
//				if (tReturn != returnType)
//					body = Expression.Convert(unary, tReturn);
//				else
//					body = unary;
//			}
//			catch (Exception ex)
//			{
//				//Avoid capture of ex
//				var message = ex.Message;
//				return delegate { throw new InvalidOperationException(message); };
//			}

//			//Create our function
//			try
//			{
//				var lambda = Expression.Lambda(body, pValue);
//				var del = lambda.Compile();
//				return (Func<TValue, TReturn>) del;
//			}
//			catch (Exception ex)
//			{
//				//Avoid capture of ex
//				var message = ex.Message;
//				return delegate { throw new InvalidOperationException(message); };
//			}
//		}

//		public static Func<TValue, TArg, TReturn> CreateFunction<TValue, TArg, TReturn>(
//			Func<Expression, Expression, BinaryExpression> binaryExpression)
//		{
//			return CreateFunction<TValue, TArg, TReturn>(binaryExpression, typeof(TValue), typeof(TArg), typeof(TReturn));
//		}

//		public static Func<TValue, TArg, TReturn> CreateFunction<TValue, TArg, TReturn>(
//			Func<Expression, Expression, BinaryExpression> binaryExpression,
//			Type valueType, Type argType, Type returnType)
//		{
//			//Value
//			var tValue = typeof(TValue);
//			var parameterValue = Expression.Parameter(tValue, "value");
//			Expression left;
//			if (tValue != valueType)
//				left = Expression.Convert(parameterValue, valueType);
//			else
//				left = parameterValue;

//			//Arg
//			var tArg = typeof(TArg);
//			var parameterArg = Expression.Parameter(tArg, "arg");
//			Expression right;
//			if (tArg != argType)
//				right = Expression.Convert(parameterArg, argType);
//			else
//				right = parameterArg;

//			//Body
//			var tReturn = typeof(TReturn);
//			BinaryExpression bin;
//			Expression body;
//			try
//			{
//				bin = binaryExpression(left, right);
//			}
//			catch (InvalidOperationException)
//			{
//				//Might be a conversion problem, try casting TArg to TValue
//				right = Expression.Convert(right, valueType);
//				try
//				{
//					bin = binaryExpression(left, right);
//				}
//				catch (Exception ex)
//				{
//					//Avoid capture of ex
//					var message = ex.Message;
//					return delegate { throw new InvalidOperationException(message); };
//				}
//			}

//			if (tReturn != returnType)
//				body = Expression.Convert(bin, tReturn);
//			else
//				body = bin;

//			//Ready to create our function
//			try
//			{
//				var lambda = Expression.Lambda(body, parameterValue, parameterArg);
//				var del = lambda.Compile();
//				return (Func<TValue, TArg, TReturn>)del;
//			}
//			catch (Exception ex)
//			{
//				//Avoid capture of ex
//				var message = ex.Message;
//				return delegate { throw new InvalidOperationException(message); };
//			}
//		}
//	}
//}