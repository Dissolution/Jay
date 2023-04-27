namespace Jay.Concurrency;

/// <summary>
/// Utilities for working with <see cref="ValueTask"/> and <see cref="ValueTask{T}"/>
/// </summary>
public static class TaskHelper
{
    public static void Consume(Task task)
    {
        if (task.IsCompleted)
            return;
        task.GetAwaiter().GetResult();
    }

    public static TResult Consume<TResult>(Task<TResult> task)
    {
        if (task.IsCompleted)
            return task.Result;
        return task.GetAwaiter().GetResult();
    }
#if NET6_0_OR_GREATER
    public static void Consume(ValueTask valueTask)
    {
        if (valueTask.IsCompleted)
            return;
        valueTask.AsTask().GetAwaiter().GetResult();
    }

    public static TResult Consume<TResult>(ValueTask<TResult> valueTask)
    {
        if (valueTask.IsCompleted)
            return valueTask.Result;
        return valueTask.AsTask()
            .GetAwaiter()
            .GetResult();
    }
#endif
}