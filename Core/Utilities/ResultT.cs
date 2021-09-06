using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;

namespace Jay
{
    /// <summary>
    /// Represents the result of an operation as a Pass or a Failure with <see cref="Exception"/> information.
    /// </summary>
    /// <remarks>
    /// For determining fields, default(Result) == default(bool) == false
    /// </remarks>
    public readonly struct Result<T> : IEquatable<Result<T>>,
                                       IEquatable<T?>,
                                       IEnumerable<T?>
    {
        public static implicit operator Result<T>(T? value) => new Result<T>(true, value, null);
        public static implicit operator Result<T>(Exception? exception) => new Result<T>(false, default(T), exception);

        public static implicit operator bool(Result<T> result) => result.Pass;
        public static bool operator true(Result<T> result) => result.Pass;
        public static bool operator false(Result<T> result) => !result.Pass;
        public static explicit operator T?(Result<T> result) => result.GetValue();
        public static explicit operator Exception(Result<T> result) => result.GetException();

        public static bool operator ==(Result<T> x, Result<T> y) => x.Pass == y.Pass;
        public static bool operator !=(Result<T> x, Result<T> y) => x.Pass != y.Pass;
        public static bool operator ==(Result<T> x, Result y) => x.Pass == y.Pass;
        public static bool operator !=(Result<T> x, Result y) => x.Pass != y.Pass;
        public static bool operator ==(Result<T> x, bool y) => x.Pass == y;
        public static bool operator !=(Result<T> x, bool y) => x.Pass != y;

        public static bool operator |(Result<T> x, Result<T> y) => x.Pass || y.Pass;
        public static bool operator |(Result<T> x, Result y) => x.Pass || y.Pass;
        public static bool operator |(Result<T> x, bool y) => x.Pass || y;
        public static bool operator &(Result<T> x, Result<T> y) => x.Pass && y.Pass;
        public static bool operator &(Result<T> x, Result y) => x.Pass && y.Pass;
        public static bool operator &(Result<T> x, bool y) => x.Pass && y;
        public static bool operator ^(Result<T> x, Result<T> y) => x.Pass ^ y.Pass;
        public static bool operator ^(Result<T> x, Result y) => x.Pass ^ y.Pass;
        public static bool operator ^(Result<T> x, bool y) => x.Pass ^ y;
        public static bool operator !(Result<T> result) => !result.Pass;

        /// <summary>
        /// Returns a passing <see cref="Result{T}"/> with the given <paramref name="value"/>.
        /// </summary>
        /// <param name="value">The passing value.</param>
        /// <returns>A passing <see cref="Result{T}"/>.</returns>
        public static Result<T> Passed(T? value) => new Result<T>(true, value, null);
        /// <summary>
        /// Returns a failing <see cref="Result{T}"/> with the given <paramref name="exception"/>.
        /// </summary>
        /// <param name="exception">The failing <see cref="Exception"/>.</param>
        /// <returns>A failed <see cref="Result{T}"/>.</returns>
        public static Result<T> Failed(Exception? exception) => new Result<T>(false, default(T), exception);

        /// <summary>
        /// Try to execute the given <paramref name="func"/>, attempt to store its result in <paramref name="value"/>,
        /// and return a <see cref="Result"/>.
        /// </summary>
        /// <param name="func">The function to try to execute.</param>
        /// <param name="value">The return value of <paramref name="func"/> or <see langword="default{T}"/> if an error occurred.</param>
        /// <returns>
        /// A passing <see cref="Result"/> if the <paramref name="func"/> executed successfully or a failed one containing
        /// the caught error if it did not.
        /// </returns>
        public static Result Try(Func<T?>? func, out T? value)
            => Result.Try<T>(func, out value);
        
        /// <summary>
        /// Try to execute the given <paramref name="func"/> and return a <see cref="Result{T}"/>.
        /// </summary>
        /// <param name="func">The function to try to execute.</param>
        /// <returns>
        /// A passing <see cref="Result{T}"/> containing <paramref name="func"/>'s return value or
        /// a failing <see cref="Result{T}"/> containing the captured <see cref="Exception"/>.
        /// </returns>
        public static Result<T> Try(Func<T?>? func)
        {
            if (func is null)
            {
                return new ArgumentNullException(nameof(func));
            }
            try
            {
                return func.Invoke();
            }
            catch (Exception ex)
            {
                return ex;
            }
        }
    
        /// <summary>
        /// Does this <see cref="Result{T}"/> represent a pass (true) or fail (false)?
        /// </summary>
        internal readonly bool Pass;
        internal readonly T? Value;
        internal readonly Exception? Exception;

        internal Result(bool pass, T? value, Exception? exception)
        {
            this.Pass = pass;
            this.Value = value;
            this.Exception = exception;
        }

        internal T? GetValue()
        {
            if (Pass)
                return Value;
            throw new InvalidOperationException("A failed Result has no Value");
        }

        internal Exception GetException()
        {
            return Exception ?? new Exception("Operation Failed");
        }

        public Result TryGetValue(out T? value)
        {
            value = Value;
            return new Result(Pass, Exception);
        }
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void ThrowIfFailed()
        {
            if (!Pass)
                throw GetException();
        }
        
        public bool TryGetError([MaybeNullWhen(false)] out Exception error)
        {
            if (!Pass)
            {
                error = GetException();
                return true;
            }
            else
            {
                error = null;
                return false;
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool Equals(Result<T> result)
        {
            if (Pass)
            {
                if (result.Pass)
                {
                    return EqualityComparer<T>.Default.Equals(Value, result.Value);
                }
                else
                {
                    return false;
                }
            }
            else
            {
                return result.Pass == false;
            }
        }
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool Equals(Result result) => result.Pass == Pass;
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool Equals(bool pass) => pass == Pass;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool Equals(T? value) => Pass && EqualityComparer<T>.Default.Equals(Value, value);

        public IEnumerator<T?> GetEnumerator()
        {
            if (Pass)
                yield return Value;
        }
        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

        public override bool Equals(object? obj)
        {
            if (obj is Result<T> resultT)
                return Equals(resultT);
            if (obj is Result result)
                return Equals(result);
            if (obj is bool pass)
                return Equals(pass);
            if (obj is T value)
                return Equals(value);
            return false;
        }

        public override int GetHashCode()
        {
            if (Pass)
            {
                return Hasher.Create(1, Value);
            }
            else
            {
                return 0;
            }
        }

        public override string ToString()
        {
            if (Pass)
                return bool.TrueString;
            if (Exception is null)
                return bool.FalseString;
            return Exception.ToString();
        }
    }
}