namespace Jay;

// ReSharper disable once InconsistentNaming
public static class Import_Results
{
    public static Result Ok() => Result.Ok();
    public static Result<TValue> Ok<TValue>(TValue value) => Result<TValue>.Ok(value);

    public static Result Error(Exception error) => Result.Error(error);
    public static Result<TValue> Error<TValue>([NotNull] Exception error) => Result<TValue>.Error(error);
    public static Result<TValue, TException> Error<TValue, TException>([NotNull] TException error)
        where TException : Exception
        => Result<TValue, TException>.Error(error);
}