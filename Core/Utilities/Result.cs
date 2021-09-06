using System;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;

namespace Jay
{
    /// <summary>
    /// Represents the result of an operation as a Pass or a Failure with <see cref="Exception"/> information.
    /// </summary>
    /// <remarks>
    /// For determining fields, default(Result) == default(bool) == false
    /// </remarks>
    public readonly struct Result : IEquatable<Result>
    {
        public static implicit operator Result(bool pass) => pass ? True : new Result(false, new Exception("Invalid Operation"));
        public static implicit operator Result(Exception? exception) => new Result(false, exception ?? new Exception("Invalid Operation"));

        public static implicit operator bool(Result result) => result.Pass;
        public static bool operator true(Result result) => result.Pass;
        public static bool operator false(Result result) => !result.Pass;
        public static explicit operator Exception(Result result) => result.Exception ?? new Exception("Invalid Operation");

        public static bool operator ==(Result x, Result y) => x.Pass == y.Pass;
        public static bool operator !=(Result x, Result y) => x.Pass != y.Pass;
        public static bool operator ==(Result x, bool y) => x.Pass == y;
        public static bool operator !=(Result x, bool y) => x.Pass != y;

        public static bool operator |(Result x, Result y) => x.Pass || y.Pass;
        public static bool operator |(Result x, bool y) => x.Pass || y;
        public static bool operator &(Result x, Result y) => x.Pass && y.Pass;
        public static bool operator &(Result x, bool y) => x.Pass && y;
        public static bool operator ^(Result x, Result y) => x.Pass ^ y.Pass;
        public static bool operator ^(Result x, bool y) => x.Pass ^ y;
        public static bool operator !(Result result) => !result.Pass;
        
        internal static readonly Result True = new Result(true, null);
            
        public static Result Failed(Exception? exception) => new Result(false, exception);
        
        public static Result Try(Action? action)
        {
            if (action is null)
            {
                return new ArgumentNullException(nameof(action));
            }

            try
            {
                action.Invoke();
                return True;
            }
            catch (Exception ex)
            {
                return ex;
            }
        }
        
        public static Result Try<T>(Func<T?>? func, out T? value)
        {
            if (func is null)
            {
                value = default;
                return new ArgumentNullException(nameof(func));
            }
            try
            {
                value = func.Invoke();
                return True;
            }
            catch (Exception ex)
            {
                value = default;
                return ex;
            }
        }

        public static Result<T> Try<T>(Func<T?>? func)
            => Result<T>.Try(func);

        [return: MaybeNull]
        public static TResult Swallow<TResult>(Func<TResult?>? function, TResult? defaultResult = default)
        {
            if (function is null)
                return defaultResult;
            try
            {
                return function();
            }
            catch // (Exception ex)
            {
                return defaultResult;
            }
        }
        
        public static Result Dispose(IDisposable? disposable)
        {
            try
            {
                disposable?.Dispose();
                return True;
            }
            catch (Exception ex)
            {
                return ex;
            }
        }
        
        public static Result Dispose<T>(T? value)
        {
            if (value is IDisposable disposable)
            {
                try
                {
                    disposable.Dispose();
                    return True;
                }
                catch (Exception ex)
                {
                    return ex;
                }
            }
            // else if (value is IAsyncDisposable asyncDisposable)
            // {
            //     try
            //     {
            //         asyncDisposable.DisposeAsync().AsTask().GetAwaiter().GetResult();
            //         return True;
            //     }
            //     catch (Exception ex)
            //     {
            //         return ex;
            //     }
            // }
            else
            {
                return True;
            }
        }
        
        internal readonly bool Pass;
        internal readonly Exception? Exception;

        internal Result(bool pass, Exception? exception)
        {
            this.Pass = pass;
            this.Exception = exception;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void ThrowIfFailed()
        {
            if (!Pass)
                throw Exception ?? new Exception("Invalid Operation");
        }
        
        public bool TryGetError([MaybeNullWhen(false)] out Exception error)
        {
            if (!Pass)
            {
                error = Exception ?? new Exception("Invalid Operation")!;
                return true;
            }
            else
            {
                error = null;
                return false;
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool Equals(Result result) => result.Pass == Pass;
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool Equals(bool pass) => pass == Pass;

        public override bool Equals(object? obj)
        {
            if (obj is Result result)
                return Equals(result);
            if (obj is bool pass)
                return Equals(pass);
            return false;
        }

        public override int GetHashCode() => Pass ? 1 : 0;

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