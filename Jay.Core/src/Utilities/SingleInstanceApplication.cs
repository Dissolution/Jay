using System.Reflection;

namespace Jay.Utilities;

/// <summary>
/// <see cref="SingleInstanceApplication" /> is a helper class that lets any running process ensure that it
/// is the only running instance.<br />
/// Use at the top of your Program.cs with:<br />
/// <c>using var singleInstance = new SingleInstanceApplication();</c>
/// </summary>
/// <remarks>
/// A wrapper around a <see cref="Mutex" />
/// </remarks>
public sealed class SingleInstanceApplication : IDisposable
{
    private readonly Mutex _mutex;

    private bool _first;

    /// <summary>
    /// Is this the first instance of this application?
    /// </summary>
    public bool FirstInstance => _first;

    /// <summary>
    /// Gets the Name this Single Instance Mutex is registered under
    /// </summary>
    public string MutexName { get; }

    /// <summary>
    /// Construct a new <see cref="SingleInstanceApplication" />
    /// </summary>
    /// <param name="name">
    /// The name to register this application under.
    /// Defaults to the executing process's full name.
    /// </param>
    /// <param name="mustBeFirst">
    /// If <c>true</c>, this must be the only instance of this application or an <see cref="InvalidOperationException" /> will be thrown.<br />
    /// If <c>false</c>, it is okay if this is not the only instance.
    /// </param>
    public SingleInstanceApplication(string? name, bool mustBeFirst = true)
    {
        MutexName = name ?? CreateName();
        _mutex = new(true, name, out _first);
        if (!_first && mustBeFirst)
            throw new InvalidOperationException("There may only be a single instance of this application running at a time.");
    }

    /// <inheritdoc />
    public void Dispose()
    {
        _mutex.Dispose();
        GC.SuppressFinalize(this);
    }
    private static string CreateName()
    {
        return Assembly.GetExecutingAssembly()
            .GetName()
            .FullName;
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
        if (_first) return true;
        bool waited = _mutex.WaitOne();
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
        if (_first) return true;
        bool waited = _mutex.WaitOne(timeout);
        if (waited)
            _first = true;
        return waited;
    }

    /// <inheritdoc />
    public override string ToString()
    {
        return $"Single Instance Application '{MutexName}'";
    }
}