using System.Runtime.CompilerServices;
using Jay.Dumping;

namespace Jay;

/// <summary>
/// Represents the result of an operation as a Pass or a Failure with <see cref="Exception"/> information.
/// </summary>
public readonly struct Result : IEquatable<Result>
{
    internal const string DefaultErrorMessage = "Operation Failed";

    public static implicit operator Result(bool pass) => pass ? Pass : new Result(false, new Exception(DefaultErrorMessage));
    public static implicit operator Result(Exception? exception) => new Result(false, exception ?? new Exception(DefaultErrorMessage));
    public static implicit operator bool(Result result) => result._pass;
    public static explicit operator Exception?(Result result) => result._pass ? null : result._error ?? new Exception(DefaultErrorMessage);

    public static bool operator ==(Result x, Result y) => x._pass == y._pass;
    public static bool operator !=(Result x, Result y) => x._pass != y._pass;
    public static bool operator ==(Result x, bool y) => x._pass == y;
    public static bool operator !=(Result x, bool y) => x._pass != y;

    public static bool operator |(Result x, Result y) => x._pass || y._pass;
    public static bool operator |(Result x, bool y) => y || x._pass;
    public static bool operator &(Result x, Result y) => x._pass && y._pass;
    public static bool operator &(Result x, bool y) => y && x._pass;
    public static bool operator ^(Result x, Result y) => x._pass ^ y._pass;
    public static bool operator ^(Result x, bool y) => x._pass ^ y;

    public static bool operator true(Result result) => result._pass;
    public static bool operator false(Result result) => !result._pass;
    public static bool operator !(Result result) => !result._pass;

    public static readonly Result Pass = new Result(true, null);

    public static Result Fail(Exception? exception = default) => new Result(false, exception ?? new Exception(DefaultErrorMessage));

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

    public static Result Try<T>(Func<T>? func, [MaybeNullWhen(false)] out T? value)
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

    public static Result<T> Try<T>(Func<T>? func) => Result<T>.Try(func);
    
    public static T? Swallow<T>(Func<T?>? func, T? fallback = default)
    {
        if (func is null) return fallback;
        try
        {
            return func();
        }
        catch
        {
            return fallback;
        }
    }

    public static TOut Swallow<TIn, TOut>(TIn instance, Func<TIn, TOut> select, TOut fallback)
    {
        try
        {
            return select(instance);
        }
        catch // (Exception ex)
        {
            return fallback;
        }
    }
    
    // _pass is the field (rather than _fail) because default(Result) should be a failure
    internal readonly bool _pass;
    internal readonly Exception? _error;

    internal Result(bool pass, Exception? error)
    {
        _pass = pass;
        _error = error;
    }

    public Result<T> Failed<T>()
    {
        if (!_pass)
        {
            return new Result<T>(false, default, _error);
        }
        throw Dump.GetException<InvalidOperationException>($"Cannot returned a Failed Result<{typeof(T)}> from a non-failed Result");
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void ThrowIfFailed()
    {
        if (!_pass)
        {
            throw (_error ?? new Exception(DefaultErrorMessage));
        }
    }

    public bool Failed() => !_pass;

    public bool Failed([NotNullWhen(true)] out Exception? error)
    {
        if (_pass)
        {
            error = null;
            return false;
        }

        error = (_error ?? new Exception(DefaultErrorMessage));
        return true;
    }

    public bool Equals(Result result) => result._pass == _pass;

    public bool Equals(bool pass) => pass == _pass;

    public override bool Equals(object? obj)
    {
        if (obj is Result result)
            return Equals(result);
        if (obj is bool pass)
            return Equals(pass);
        return false;
    }

    public override int GetHashCode() => _pass ? 1 : 0;

    public override string ToString()
    {
        if (_pass)
            return "Pass";
        if (_error is null)
            return "Fail";
        return $"Fail: {_error}";
    }
}