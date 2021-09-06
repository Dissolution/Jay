using System;

namespace Jay.Operators
{
	/// <summary>
	/// Support for Operators on Generic values.
	/// </summary>
	/// <exception cref="InvalidOperationException">Thrown if the generic type does not support the specified operation.</exception>
	public static class Operator
	{
		/// <summary>
		/// Determines if the supplied value is not <c>null</c>.
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="value"></param>
		/// <returns></returns>
		public static bool NotNull<T>(T value)
		{
			return Operator<T>.NotNull(value);
		}

		/// <summary>
		/// Is the supplied value equivalent to <c>true</c>?
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="value"></param>
		/// <returns></returns>
		public static bool IsTrue<T>(T value)
		{
			return Operator<T>.IsTrue(value);
		}
		
		/// <summary>
		/// Is the supplied value equivalent to <c>false</c>?
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="value"></param>
		/// <returns></returns>
		public static bool IsFalse<T>(T value) => Operator<T>.IsFalse(value);

		/// <summary>
		/// Gets the default value for a type.
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <returns></returns>
		public static T? Default<T>() => Operator<T>.Default;

		/// <summary>
		/// Gets the zero value for a type.
		/// This is often the same as <see cref="Default{T}"/> except for <see cref="Nullable{T}"/> values.
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <returns></returns>
		public static T? Zero<T>() => Operator<T>.Zero;

		/// <summary>
		/// Convert a value to a return type using implicit or explicit conversion.
		/// </summary>
		/// <typeparam name="TValue"></typeparam>
		/// <typeparam name="TReturn"></typeparam>
		/// <param name="value"></param>
		/// <returns></returns>
		public static TReturn Convert<TValue, TReturn>(TValue value) => Operator<TValue, TReturn>.Convert(value);

		/// <summary>
		/// Perform unary negation (-) on a value.
		/// </summary>
		/// <typeparam name="TValue"></typeparam>
		/// <param name="value"></param>
		/// <returns></returns>
		public static TValue Negate<TValue>(TValue value) => Operator<TValue>.Negate(value);

		/// <summary>
		/// Perform unary negation and assignment (-=) on a value.
		/// </summary>
		/// <typeparam name="TValue"></typeparam>
		/// <param name="value"></param>
		/// <returns></returns>
		public static void Negate<TValue>(ref TValue value) => value = Operator<TValue>.Negate(value);

		/// <summary>
		/// Perform unary NOT (!) on a value.
		/// </summary>
		/// <typeparam name="TValue"></typeparam>
		/// <param name="value"></param>
		/// <returns></returns>
		public static TValue Not<TValue>(TValue value) => Operator<TValue>.Not(value);

		/// <summary>
		/// Perform unary NOT and assignment (!=) on a value.
		/// </summary>
		/// <typeparam name="TValue"></typeparam>
		/// <param name="value"></param>
		/// <returns></returns>
		public static void Not<TValue>(ref TValue value) => value = Operator<TValue>.Negate(value);

		/// <summary>
		/// Perform unary bitwise complement (~) on a value.
		/// </summary>
		/// <typeparam name="TValue"></typeparam>
		/// <param name="value"></param>
		/// <returns></returns>
		public static TValue BitwiseComplement<TValue>(TValue value) => Operator<TValue>.BitwiseComplement(value);

		/// <summary>
		/// Perform unary bitwise complement and assignment (~=) on a value.
		/// </summary>
		/// <typeparam name="TValue"></typeparam>
		/// <param name="value"></param>
		/// <returns></returns>
		public static void BitwiseComplement<TValue>(ref TValue value) => value = Operator<TValue>.BitwiseComplement(value);

		/// <summary>
		/// Perform an increment (++) on a value.
		/// </summary>
		/// <typeparam name="TValue"></typeparam>
		/// <param name="value"></param>
		/// <returns></returns>
		public static TValue Increment<TValue>(TValue value) => Operator<TValue>.Increment(value);

		/// <summary>
		/// Perform an increment and assignment (++=) on a value.
		/// </summary>
		/// <typeparam name="TValue"></typeparam>
		/// <param name="value"></param>
		public static void Increment<TValue>(ref TValue value) => value = Operator<TValue>.Increment(value);

		/// <summary>
		/// Perform an decrement (--) on a value.
		/// </summary>
		/// <typeparam name="TValue"></typeparam>
		/// <param name="value"></param>
		/// <returns></returns>
		public static TValue Decrement<TValue>(TValue value) => Operator<TValue>.Decrement(value);

		/// <summary>
		/// Perform an decrement and assignment (--=) on a value.
		/// </summary>
		/// <typeparam name="TValue"></typeparam>
		/// <param name="value"></param>
		/// <returns></returns>
		public static void Decrement<TValue>(ref TValue value) => value = Operator<TValue>.Decrement(value);

		/// <summary>
		/// Perform binary OR (|) on a value and an argument.
		/// </summary>
		/// <typeparam name="TValue"></typeparam>
		/// <typeparam name="TArg"></typeparam>
		/// <param name="value"></param>
		/// <param name="arg"></param>
		/// <returns></returns>
		public static TValue Or<TValue, TArg>(TValue value, TArg arg) => Operator<TValue, TArg>.Or(value, arg);

		/// <summary>
		/// Perform binary OR and assignment (|=) on a value and an argument.
		/// </summary>
		/// <typeparam name="TValue"></typeparam>
		/// <typeparam name="TArg"></typeparam>
		/// <param name="value"></param>
		/// <param name="arg"></param>
		/// <returns></returns>
		public static void Or<TValue, TArg>(ref TValue value, TArg arg) => value = Operator<TValue, TArg>.Or(value, arg);

		/// <summary>
		/// Perform binary AND (&amp;) on a value and an argument.
		/// </summary>
		/// <typeparam name="TValue"></typeparam>
		/// <typeparam name="TArg"></typeparam>
		/// <param name="value"></param>
		/// <param name="arg"></param>
		/// <returns></returns>
		public static TValue And<TValue, TArg>(TValue value, TArg arg) => Operator<TValue, TArg>.And(value, arg);

		/// <summary>
		/// Perform binary AND and assignment (&amp;=) on a value and an argument.
		/// </summary>
		/// <typeparam name="TValue"></typeparam>
		/// <typeparam name="TArg"></typeparam>
		/// <param name="value"></param>
		/// <param name="arg"></param>
		/// <returns></returns>
		public static void And<TValue, TArg>(ref TValue value, TArg arg) => value = Operator<TValue, TArg>.And(value, arg);

		/// <summary>
		/// Perform binary XOR (^) on a value and an argument.
		/// </summary>
		/// <typeparam name="TValue"></typeparam>
		/// <typeparam name="TArg"></typeparam>
		/// <param name="value"></param>
		/// <param name="arg"></param>
		/// <returns></returns>
		public static TValue Xor<TValue, TArg>(TValue value, TArg arg) => Operator<TValue, TArg>.Xor(value, arg);

		/// <summary>
		/// Perform binary XOR and assignment (^=) on a value and an argument.
		/// </summary>
		/// <typeparam name="TValue"></typeparam>
		/// <typeparam name="TArg"></typeparam>
		/// <param name="value"></param>
		/// <param name="arg"></param>
		/// <returns></returns>
		public static void Xor<TValue, TArg>(ref TValue value, TArg arg) => value = Operator<TValue, TArg>.Xor(value, arg);

		/// <summary>
		/// Perform addition (+) on a value and an argument.
		/// </summary>
		/// <typeparam name="TValue"></typeparam>
		/// <typeparam name="TArg"></typeparam>
		/// <param name="value"></param>
		/// <param name="arg"></param>
		/// <returns></returns>
		public static TValue Add<TValue, TArg>(TValue value, TArg arg) => Operator<TValue, TArg>.Add(value, arg);

		/// <summary>
		/// Perform addition and assignment (+=) on a value and an argument.
		/// </summary>
		/// <typeparam name="TValue"></typeparam>
		/// <typeparam name="TArg"></typeparam>
		/// <param name="value"></param>
		/// <param name="arg"></param>
		/// <returns></returns>
		public static void Add<TValue, TArg>(ref TValue value, TArg arg) => value = Operator<TValue, TArg>.Add(value, arg);

		/// <summary>
		/// Perform subtraction (-) on a value and an argument.
		/// </summary>
		/// <typeparam name="TValue"></typeparam>
		/// <typeparam name="TArg"></typeparam>
		/// <param name="value"></param>
		/// <param name="arg"></param>
		/// <returns></returns>
		public static TValue Subtract<TValue, TArg>(TValue value, TArg arg) => Operator<TValue, TArg>.Subtract(value, arg);

		/// <summary>
		/// Perform subtraction and assignment (-=) on a value and an argument.
		/// </summary>
		/// <typeparam name="TValue"></typeparam>
		/// <typeparam name="TArg"></typeparam>
		/// <param name="value"></param>
		/// <param name="arg"></param>
		/// <returns></returns>
		public static void Subtract<TValue, TArg>(ref TValue value, TArg arg) =>
			value = Operator<TValue, TArg>.Subtract(value, arg);

		/// <summary>
		/// Perform multiplication (*) on a value and an argument.
		/// </summary>
		/// <typeparam name="TValue"></typeparam>
		/// <typeparam name="TArg"></typeparam>
		/// <param name="value"></param>
		/// <param name="arg"></param>
		/// <returns></returns>
		public static TValue Multiply<TValue, TArg>(TValue value, TArg arg) => Operator<TValue, TArg>.Multiply(value, arg);

		/// <summary>
		/// Perform multiplication and assignment (*=) on a value and an argument.
		/// </summary>
		/// <typeparam name="TValue"></typeparam>
		/// <typeparam name="TArg"></typeparam>
		/// <param name="value"></param>
		/// <param name="arg"></param>
		/// <returns></returns>
		public static void Multiply<TValue, TArg>(ref TValue value, TArg arg) =>
			value = Operator<TValue, TArg>.Multiply(value, arg);

		/// <summary>
		/// Perform division (/) on a value and an argument.
		/// </summary>
		/// <typeparam name="TValue"></typeparam>
		/// <typeparam name="TArg"></typeparam>
		/// <param name="value"></param>
		/// <param name="arg"></param>
		/// <returns></returns>
		public static TValue Divide<TValue, TArg>(TValue value, TArg arg) => Operator<TValue, TArg>.Divide(value, arg);

		/// <summary>
		/// Perform division and assignment (/=) on a value and an argument.
		/// </summary>
		/// <typeparam name="TValue"></typeparam>
		/// <typeparam name="TArg"></typeparam>
		/// <param name="value"></param>
		/// <param name="arg"></param>
		/// <returns></returns>
		public static void Divide<TValue, TArg>(ref TValue value, TArg arg) => value = Operator<TValue, TArg>.Divide(value, arg);

		/// <summary>
		/// Perform modular division (%) on a value and an argument.
		/// </summary>
		/// <typeparam name="TValue"></typeparam>
		/// <typeparam name="TArg"></typeparam>
		/// <param name="value"></param>
		/// <param name="arg"></param>
		/// <returns></returns>
		public static TValue Modulo<TValue, TArg>(TValue value, TArg arg) => Operator<TValue, TArg>.Modulo(value, arg);

		/// <summary>
		/// Perform modular division and assignment (%=) on a value and an argument.
		/// </summary>
		/// <typeparam name="TValue"></typeparam>
		/// <typeparam name="TArg"></typeparam>
		/// <param name="value"></param>
		/// <param name="arg"></param>
		/// <returns></returns>
		public static void Modulo<TValue, TArg>(ref TValue value, TArg arg) => value = Operator<TValue, TArg>.Modulo(value, arg);

		/// <summary>
		/// Perform a bitwise left shift (&lt;&lt;) on the supplied value.
		/// </summary>
		/// <typeparam name="TValue"></typeparam>
		/// <param name="value"></param>
		/// <param name="shift"></param>
		/// <returns></returns>
		public static TValue LeftShift<TValue>(TValue value, int shift) => Operator<TValue>.LeftShift(value, shift);
		/// <summary>
		/// Perform a bitwise left shift (&lt;&lt;) on the supplied value.
		/// </summary>
		/// <typeparam name="TValue"></typeparam>
		/// <param name="value"></param>
		/// <param name="shift"></param>
		public static void LeftShift<TValue>(ref TValue value, int shift) => value = Operator<TValue>.LeftShift(value, shift);

		/// <summary>
		/// Perform a bitwise right shift (&gt;&gt;) on the supplied value.
		/// </summary>
		/// <typeparam name="TValue"></typeparam>
		/// <param name="value"></param>
		/// <param name="shift"></param>
		/// <returns></returns>
		public static TValue RightShift<TValue>(TValue value, int shift) => Operator<TValue>.RightShift(value, shift);

		/// <summary>
		/// Perform a bitwise right shift (&gt;&gt;) on the supplied value.
		/// </summary>
		/// <typeparam name="TValue"></typeparam>
		/// <param name="value"></param>
		/// <param name="shift"></param>
		public static void RightShift<TValue>(ref TValue value, int shift) => value = Operator<TValue>.RightShift(value, shift);


		/// <summary>
		/// Compare two values for equality (==)
		/// </summary>
		/// <typeparam name="TValue"></typeparam>
		/// <typeparam name="TArg"></typeparam>
		/// <param name="value"></param>
		/// <param name="other"></param>
		/// <returns></returns>
		public static bool Equal<TValue, TArg>(TValue value, TArg other) => Operator<TValue, TArg>.Equal(value, other);

		/// <summary>
		/// Compare two values for inequality (!=)
		/// </summary>
		/// <typeparam name="TValue"></typeparam>
		/// <typeparam name="TArg"></typeparam>
		/// <param name="value"></param>
		/// <param name="other"></param>
		/// <returns></returns>
		public static bool NotEqual<TValue, TArg>(TValue value, TArg other) => Operator<TValue, TArg>.NotEqual(value, other);

		/// <summary>
		/// Determine if a value is less than (&lt;) another value.
		/// </summary>
		/// <typeparam name="TValue"></typeparam>
		/// <typeparam name="TArg"></typeparam>
		/// <param name="value"></param>
		/// <param name="other"></param>
		/// <returns></returns>
		public static bool LessThan<TValue, TArg>(TValue value, TArg other) => Operator<TValue, TArg>.LessThan(value, other);

		/// <summary>
		/// Determine if a value is less than or equal (&lt;=) another value.
		/// </summary>
		/// <typeparam name="TValue"></typeparam>
		/// <typeparam name="TArg"></typeparam>
		/// <param name="value"></param>
		/// <param name="other"></param>
		/// <returns></returns>
		public static bool LessThanOrEqual<TValue, TArg>(TValue value, TArg other) => Operator<TValue, TArg>.LessThanOrEqual(value, other);

		/// <summary>
		/// Determine if a value is greater than (&gt;) another value.
		/// </summary>
		/// <typeparam name="TValue"></typeparam>
		/// <typeparam name="TArg"></typeparam>
		/// <param name="value"></param>
		/// <param name="other"></param>
		/// <returns></returns>
		public static bool GreaterThan<TValue, TArg>(TValue value, TArg other) => Operator<TValue, TArg>.GreaterThan(value, other);

		/// <summary>
		/// Determine if a value is greater than or equal (&gt;=) another value.
		/// </summary>
		/// <typeparam name="TValue"></typeparam>
		/// <typeparam name="TArg"></typeparam>
		/// <param name="value"></param>
		/// <param name="other"></param>
		/// <returns></returns>
		public static bool GreaterThanOrEqual<TValue, TArg>(TValue value, TArg other) => Operator<TValue, TArg>.GreaterThanOrEqual(value, other);
	}
}
