namespace Jay;

public static class ResultStaticImport
{
    public static Result Ok() => Result.Ok;
    public static Result<T> Ok<T>(T value) => Result<T>.Ok(value);

    public static Result Error(Exception error) => Result.Error(error);
    public static Result<T> Error<T>(Exception error) => Result<T>.Error(error);
}