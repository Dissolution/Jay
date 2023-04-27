using System.Reflection;

namespace Jay.Utilities;

/// <summary>
/// A helper class to ensure that only one instance of an application is run at a time.
/// </summary>
public sealed class SingleInstanceApplication : IDisposable
{
    private static string CreateName()
    {
        return Assembly.GetExecutingAssembly()
                       .GetName()
                       .FullName;
    }

    private readonly string _name;
    private readonly Mutex _mutex;
    private bool _first;

    /// <summary>
    /// Is this the first instance of this application?
    /// </summary>
    public bool FirstInstance => _first;

    /// <summary>
    /// Gets the Name this Single Instance Mutex is registered under
    /// </summary>
    public string MutexName => _name;

    /// <summary>
    /// Construct a new <see cref="SingleInstanceApplication"/> with the specified name used for debugging purposes.
    /// </summary>
    /// <param name="name"></param>
    public SingleInstanceApplication(string? name, bool mustBeFirst = true)
    {
        _name = name ?? CreateName();
        _mutex = new Mutex(true, name, out _first);
        if (!_first && mustBeFirst)
            throw new InvalidOperationException("There may only be a single instance of this application running at a time.");
    }
    /// <inheritdoc />
    ~SingleInstanceApplication()
    {
        Dispose();
    }

    /// <summary>
    /// Wait until we can be the only instance running.
    /// </summary>
    /// <returns></returns>
    public bool WaitForSingleInstance()
    {
        var waited = _mutex.WaitOne();
        if (waited)
            _first = true;
        return waited;
    }

    /// <summary>
    /// Wait until we can be the only instance running.
    /// </summary>
    /// <returns></returns>
    public bool WaitForSingleInstance(TimeSpan timeout)
    {
        var waited = _mutex.WaitOne(timeout);
        if (waited)
            _first = true;
        return waited;
    }

    /// <inheritdoc />
    public void Dispose()
    {
        _mutex.Dispose();
        GC.SuppressFinalize(this);
    }

    /// <inheritdoc />
    public override string ToString()
    {
        return $"Single Instance Application '{_name}'";
    }
}