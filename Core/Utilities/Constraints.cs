namespace Jay;

public static class Constraints
{
    public readonly struct IsNew<T>
        where T : new()
    { }

    public readonly struct IsDisposable<T>
        where T : IDisposable
    { }

    public readonly struct IsClass<T>
        where T : class
    { }

    public readonly struct IsNewDisposable<T>
        where T : IDisposable, new()
    { }
}