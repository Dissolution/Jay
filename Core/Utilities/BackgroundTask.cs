#if NET6_0_OR_GREATER

namespace Jay.Utilities;

public sealed class BackgroundTask : IDisposable
{
    private const uint Timer_MaxSupportedTimeout = 0xFFFFFFFE;

    private readonly TimeSpan _interval;
    private readonly CancellationTokenSource _cts;
    private readonly Func<Task> _intervalTask;
    private Task? _timerTask;

    public TimeSpan Interval => _interval;
    public bool HasStarted => _timerTask is not null;

    public BackgroundTask(TimeSpan interval, Func<Task> intervalTask)
    {
        if (interval.TotalMilliseconds is < 1 or > Timer_MaxSupportedTimeout)
            throw new ArgumentOutOfRangeException(nameof(interval), interval,
                $"Interval must be greater than Zero and less than or equal to {TimeSpan.FromMilliseconds(Timer_MaxSupportedTimeout)}");
        _interval = interval;
        _cts = new CancellationTokenSource();
        _intervalTask = intervalTask;
    }

    public BackgroundTask(TimeSpan interval, Action intervalAction)
    {
        if (interval.TotalMilliseconds is < 1 or > Timer_MaxSupportedTimeout)
            throw new ArgumentOutOfRangeException(nameof(interval), interval,
                $"Interval must be greater than Zero and less than or equal to {TimeSpan.FromMilliseconds(Timer_MaxSupportedTimeout)}");
        _interval = interval;
        _cts = new CancellationTokenSource();
        _intervalTask = () => Task.Run(intervalAction);
    }

    private async Task DoWorkAsync(CancellationToken token)
    {
        PeriodicTimer? timer = null;
        try
        {
            timer = new PeriodicTimer(_interval);
            while (await timer.WaitForNextTickAsync(token) && !token.IsCancellationRequested)
            {
                await _intervalTask();
            }
        }
        // Ignore these exceptions
        catch (TaskCanceledException) { }
        catch (OperationCanceledException) { }
        // Always cleanup the timer
        finally
        {
            timer?.Dispose();
        }
    }

    /// <summary>
    /// Starts this <see cref="BackgroundTask"/> executing its provided interval action
    /// </summary>
    /// <exception cref="InvalidOperationException"></exception>
    public void Start()
    {
        if (HasStarted)
            throw new InvalidOperationException("This BackgroundTask has already been started");
        _timerTask = DoWorkAsync(_cts.Token);
    }

    /// <summary>
    /// Stops this <see cref="BackgroundTask"/> if it is running
    /// </summary>
    public async Task StopAsync()
    {
        // If we haven't started, we cannot stop
        if (!HasStarted) return;
        // Send cancellation
        _cts.Cancel();
        // Wait for the last task to complete after cancellation
        await _timerTask!;
    }

    /// <inheritdoc />
    public void Dispose()
    {
        if (_timerTask is not null)
        {
            _cts.Cancel();
            _timerTask.Dispose();
            _timerTask = null;
        }
        _cts.Dispose();
    }
}

#endif