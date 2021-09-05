using System;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;

namespace Jay
{
    /// <summary>
    /// Represents the result of an operation as a Pass or a Failure with <see cref="Exception"/> information.
    /// </summary>
    public readonly struct Result : IEquatable<Result>, IEquatable<bool>
    {
        public static implicit operator bool(Result result) => result._pass;
        public static implicit operator Result(bool pass) => pass ? Pass : new Result(false, new Exception("Operation Failed"));
        public static implicit operator Result(Exception? error) => new Result(false, error ?? new Exception("Operation Failed"));
        public static implicit operator Exception(Result result) => result._error ?? new Exception("Operation Failed");
        
        public static bool operator ==(Result x, Result y) => x._pass == y._pass;
        public static bool operator !=(Result x, Result y) => x._pass != y._pass;
        public static bool operator ==(Result result, bool pass) => pass == result._pass;
        public static bool operator !=(Result result, bool pass) => pass != result._pass;

        public static bool operator |(Result x, Result y) => x._pass || y._pass;
        public static bool operator |(Result result, bool pass) => pass || result._pass;
        public static bool operator &(Result x, Result y) => x._pass && y._pass;
        public static bool operator &(Result result, bool pass) => pass && result._pass;
        public static bool operator ^(Result x, Result y) => x._pass ^ y._pass;
        public static bool operator ^(Result result, bool pass) => pass ^ result._pass;

        public static Result operator !(Result result) => result._pass ? new Result(false, new Exception("Operation Failed")) : Pass;
        public static bool operator true(Result result) => result._pass;
        public static bool operator false(Result result) => !result._pass;
        
        internal static readonly Result Pass = new Result(true, null);  
            
        /// <summary>
        /// Returns a failed <see cref="Result"/> with the given <paramref name="error"/>.
        /// </summary>
        public static Result Fail(Exception error) => new Result(false, error ?? new Exception("Operation Failed"));
        
        /// <summary>
        /// Tries to invoke the given <paramref name="action"/>, capturing any thrown <see cref="Exception"/> in the returned <see cref="Result"/>.
        /// </summary>
        public static Result Try(Action? action)
        {
            if (action is null)
            {
                return new ArgumentNullException(nameof(action));
            }
            try
            {
                action.Invoke();
                return Pass;
            }
            catch (Exception ex)
            {
                return ex;
            }
        }
        
        /// <summary>
        /// Tries to invoke the given <paramref name="func"/>, storing its return <paramref name="value"/> and capturing any thrown <see cref="Exception"/>
        /// in the returned <see cref="Result"/>.
        /// </summary>
        /// <param name="func"></param>
        /// <param name="value"></param>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public static Result Try<T>(Func<T>? func, [MaybeNullWhen(false)] out T value)
        {
            if (func is null)
            {
                value = default;
                return new ArgumentNullException(nameof(func));
            }
            try
            {
                value = func.Invoke();
                return Pass;
            }
            catch (Exception ex)
            {
                value = default;
                return ex;
            }
        }
        
        // _pass is the field (rather than _fail) because: default(Result) == default(bool) == false
        private readonly bool _pass;
        private readonly Exception? _error;

        private Result(bool pass, Exception? error)
        {
            _pass = pass;
            _error = error;
            Debug.Assert((_pass && _error is null) || (!_pass && _error is not null));
        }

        public void ThrowIfFailed()
        {
            if (!_pass)
                throw _error!;
        }

        public bool TryGetError([NotNullWhen(true)] out Exception? error)
        {
            if (_pass)
            {
                error = null;
                return false;
            }
            else
            {
                error = _error!;
                return true;
            }
        }
        
        /// <inheritdoc />
        public bool Equals(Result result) => result._pass == _pass;
        
        /// <inheritdoc />
        public bool Equals(bool pass) => pass == _pass;

        /// <inheritdoc />
        public override bool Equals(object? obj)
        {
            if (obj is Result result) return result._pass == _pass;
            if (obj is bool pass) return pass == _pass;
            return false;
        }

        /// <inheritdoc />
        public override int GetHashCode()
        {
            return _pass ? 1 : 0;
        }

        /// <inheritdoc />
        public override string ToString()
        {
            if (_pass)
                return "Pass";
            if (_error is null)
                return "Fail";
            return $"Fail: {_error}";
        }
    }
}