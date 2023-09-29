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
    internal const string DEFAULT_ERROR_MESSAGE = "Operation Failed";

    /// <summary>
    /// Maps a <see cref="bool"/> <paramref name="ok"/> into a <see cref="Result"/>
    /// </summary>
    /// <param name="ok">
    /// If <c>true</c>, returns <see cref="Ok"/><br/>
    /// If <c>false</c>, returns <see cref="Error"/> but does not capture an <see cref="Exception"/>
    /// </param>
    public static implicit operator Result(bool ok) => ok ? Ok : new(false, null);
    
    /// <summary>
    /// Maps an <see cref="Exception"/> <paramref name="error"/> into a <see cref="Result"/>
    /// </summary>
    /// <param name="error">
    /// The attached <see cref="Exception"/><br/>
    /// if <c>null</c>, a new <see cref="Exception"/> will be captured at this stack position
    /// </param>
    public static implicit operator Result(Exception? error) => Error(error);
    
    public static implicit operator bool(Result result) => result._ok;

    public static bool operator true(Result result) => result._ok;
    public static bool operator false(Result result) => !result._ok;
    public static bool operator !(Result result) => !result._ok;
    public static bool operator ~(Result value) => throw new NotSupportedException("Cannot apply ~ to Result");

    public static bool operator ==(Result x, Result y) => x._ok == y._ok;
    public static bool operator ==(Result result, bool pass) => pass == result._ok;
    public static bool operator ==(bool pass, Result result) => pass == result._ok;
    public static bool operator !=(Result x, Result y) => x._ok != y._ok;
    public static bool operator !=(Result result, bool pass) => pass != result._ok;
    public static bool operator !=(bool pass, Result result) => pass != result._ok;
    public static bool operator |(Result x, Result y) => x._ok || y._ok;
    public static bool operator |(Result result, bool pass) => pass || result._ok;
    public static bool operator |(bool pass, Result result) => pass || result._ok;
    public static bool operator &(Result x, Result y) => x._ok && y._ok;
    public static bool operator &(Result result, bool pass) => pass && result._ok;
    public static bool operator &(bool pass, Result result) => pass && result._ok;
    public static bool operator ^(Result x, Result y) => x._ok ^ y._ok;
    public static bool operator ^(Result result, bool pass) => pass ^ result._ok;
    public static bool operator ^(bool pass, Result result) => pass ^ result._ok;

    /// <summary>
    /// A successful <see cref="Result" />
    /// </summary>
    public static readonly Result Ok = new(true, null);

    /// <summary>
    /// A failed <see cref="Result"/> containing an attached <paramref name="error"/>
    /// </summary>
    /// <param name="error">
    /// An attached <see cref="Exception"/><br/>
    /// If <c>null</c>, a new <see cref="Exception"/> will be created at this callsite<br/>
    /// If you do not want to capture an <see cref="Exception"/>, use the implicit cast from <c>false</c> instead
    /// </param>
    /// <returns>
    /// An <see cref="Error"/>
    /// </returns>
    public static Result Error(Exception? error)
    {
        return new(false, error ?? new Exception(DEFAULT_ERROR_MESSAGE));
    }
}