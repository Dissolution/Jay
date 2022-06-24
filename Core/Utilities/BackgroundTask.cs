namespace Jay;

public class BackgroundTask : IDisposable
{
    private readonly TimeSpan _interval;
    private readonly CancellationTokenSource _cts;
    private readonly Func<Task> _intervalTask;
    private Task? _timerTask;

    public BackgroundTask(TimeSpan interval, Func<Task> intervalTask)
    {
        _interval = interval;
        _cts = new CancellationTokenSource();
        _intervalTask = intervalTask;
    }
    
    public BackgroundTask(TimeSpan interval, Action intervalAction)
    {
        _interval = interval;
        _cts = new CancellationTokenSource();
        _intervalTask = () => Task.Run(intervalAction);
    }

    private async Task DoWorkAsync(CancellationToken token = default)
    {
        PeriodicTimer? timer = null;
        try
        {
            timer = new PeriodicTimer(_interval);
            while (await timer.WaitForNextTickAsync(token) &&
                !token.IsCancellationRequested)
            {
                await _intervalTask();
            }
        }
        // Ignore these exceptions
        catch (TaskCanceledException) { }
        catch (OperationCanceledException) { }
        finally
        {
            timer?.Dispose();
        }
    }
    
    
    public void Start(CancellationToken token = default)
    {
        if (_timerTask is not null)
            throw new InvalidOperationException();
        _timerTask = DoWorkAsync(token);
    }

    
    public async Task StopAsync()
    {
        if (_timerTask is null) return;
        _cts.Cancel();
        // Wait for last task to complete after cancellation
        await _timerTask;
    }

    /// <inheritdoc />
    public void Dispose()
    {
        if (_timerTask is not null)
        {
            _cts.Cancel();
            _timerTask.Dispose();
        }
        _cts.Dispose();
    }
}

