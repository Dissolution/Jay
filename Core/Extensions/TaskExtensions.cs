namespace Jay;

public static class TaskExtensions
{
    public static async Task WithTimeout(this Task task, TimeSpan timeout)
    {
        // 👇 Use a CancellationTokenSource, pass the token to Task.Delay
        using var cts = new CancellationTokenSource();
        var timeoutTask = Task.Delay(timeout, cts.Token);

        var completedTask = await Task.WhenAny(task, timeoutTask);
        cts.Cancel(); // 👈 Cancel the delay / task
        if (completedTask == task)
        {
            
        }
        else
        {
            throw new TimeoutException($"Task timed out after {timeout}");
        }
    }

    public static async Task<T> WithTimeout<T>(this Task<T> task, TimeSpan timeout)
    {
        // 👇 Use a CancellationTokenSource, pass the token to Task.Delay
        using var cts = new CancellationTokenSource();
        var timeoutTask = Task.Delay(timeout, cts.Token);

        var completedTask = await Task.WhenAny(task, timeoutTask);
        cts.Cancel(); // 👈 Cancel the delay / task
        
        if (completedTask == task)
        {
            return await task.ConfigureAwait(false);
        }
        else
        {
            throw new TimeoutException($"Task timed out after {timeout}");
        }
    }
}