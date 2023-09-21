namespace Jay;

public static class Option
{
    public static Option<T> None<T>() => Option<T>.None;
    
    public static Option<T> Some<T>(T value) => Option<T>.Some(value);
}