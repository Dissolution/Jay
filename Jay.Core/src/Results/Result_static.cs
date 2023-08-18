#if NET7_0_OR_GREATER
using System.Numerics;
#endif

namespace Jay;

/* Static stuff
 *
 * As a rule, when we create a failed Result we want to have an Exception at that point.
 * Even if we have to create one, we at least capture a proper stack trace.
 * This is the overhead of Result.Fail
 *
 * We want the end user to use Result almost exactly as if it was bool.
 */

public readonly partial struct Result
#if NET7_0_OR_GREATER
    : IEqualityOperators<Result, Result, bool>,
        IEqualityOperators<Result, bool, bool>,
        IBitwiseOperators<Result, Result, bool>,
        IBitwiseOperators<Result, bool, bool>
#endif
{
    internal const string DefaultErrorMessage = "Operation Failed";

    /* We want Result to implicitly convert between bool and Exception
     * for each of use.
     * Example:
     *
     * return true;     //Ok
     * return new InvalidOperationException();  // Error(ex)
     */

    public static implicit operator Result(bool ok)
    {
        return ok ? Ok : Error(null);
    }
    public static implicit operator Result(Exception? error)
    {
        return Error(error);
    }
    public static implicit operator bool(Result result)
    {
        return result._ok;
    }

    public static bool operator true(Result result)
    {
        return result._ok;
    }
    public static bool operator false(Result result)
    {
        return !result._ok;
    }
    public static bool operator !(Result result)
    {
        return !result._ok;
    }
    public static bool operator ~(Result value)
    {
        throw new NotSupportedException("Cannot apply ~ to Result");
    }

    public static bool operator ==(Result x, Result y)
    {
        return x._ok == y._ok;
    }
    public static bool operator ==(Result result, bool pass)
    {
        return pass == result._ok;
    }
    public static bool operator ==(bool pass, Result result)
    {
        return pass == result._ok;
    }
    public static bool operator !=(Result x, Result y)
    {
        return x._ok != y._ok;
    }
    public static bool operator !=(Result result, bool pass)
    {
        return pass != result._ok;
    }
    public static bool operator !=(bool pass, Result result)
    {
        return pass != result._ok;
    }

    public static bool operator |(Result x, Result y)
    {
        return x._ok || y._ok;
    }
    public static bool operator |(Result result, bool pass)
    {
        return pass || result._ok;
    }
    public static bool operator |(bool pass, Result result)
    {
        return pass || result._ok;
    }
    public static bool operator &(Result x, Result y)
    {
        return x._ok && y._ok;
    }
    public static bool operator &(Result result, bool pass)
    {
        return pass && result._ok;
    }
    public static bool operator &(bool pass, Result result)
    {
        return pass && result._ok;
    }
    public static bool operator ^(Result x, Result y)
    {
        return x._ok ^ y._ok;
    }
    public static bool operator ^(Result result, bool pass)
    {
        return pass ^ result._ok;
    }
    public static bool operator ^(bool pass, Result result)
    {
        return pass ^ result._ok;
    }

    /// <summary>
    /// A successful <see cref="Result" />
    /// </summary>
    public static readonly Result Ok = new(true, null);

    /// <summary>
    /// Create a failing <see cref="Result" /> with <paramref name="error" />
    /// </summary>
    /// <param name="error">
    /// Additional <see cref="Exception" /> information to attach to the <see cref="Result" />, may be <c>null<c /> to avoid attaching any
    /// </param>
    /// <returns>A failed <see cref="Result" /></returns>
    public static Result Error(Exception? error)
    {
        return new(false, error ?? new Exception(DefaultErrorMessage));
    }
}