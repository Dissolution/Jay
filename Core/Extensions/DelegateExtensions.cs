using System.Diagnostics;

namespace Jay;

public static class DelegateExtensions
{
    public static Task[] InvokeAllAsTasks<TArgs>(this EventHandler<TArgs>? eventHandler,
        object? sender,
        TArgs args)
    {
        // No subscribers?
        if (eventHandler is null) return Array.Empty<Task>();

        // capture a local copy
        Delegate[] handlers = eventHandler.GetInvocationList();
        var count = handlers.Length;
        // No subscribers?
        if (count == 0) return Array.Empty<Task>();
        // We know how many we're returning
        var tasks = new Task[count];
        for (var i = 0; i < count; i++)
        {
            // Use the old FromAsync pattern to convert this to a Task!
            var task = Task.Factory.FromAsync(
                beginMethod: (callback, state) =>
                {
                    Debug.Assert(state is EventHandler<TArgs>);
                    return ((EventHandler<TArgs>)state).BeginInvoke(sender, args, callback, state);
                },
                endMethod: result =>
                {
                    Debug.Assert(result.AsyncState is EventHandler<TArgs>);
                    try
                    {
                        ((EventHandler<TArgs>)result.AsyncState).EndInvoke(result);
                    }
                    catch
                    {
                        // Swallow everything
                    }
                },
                state: handlers[i]);
            tasks[i] = task;
        }

        // They've all been started
        return tasks;
    }

    public static void InvokeAllFast<TArgs>(this EventHandler<TArgs>? eventHandler,
        object? sender,
        TArgs args)
    {
        // No subscribers?
        if (eventHandler is null) return;

        // capture a local copy
        Delegate[] handlers = eventHandler.GetInvocationList();
        for (var i = 0; i < handlers.Length; i++)
        {
            var handler = handlers[i] as EventHandler<TArgs>;
            Debug.Assert(handler is not null);
            handler.BeginInvoke(sender, args, EndInvoke<TArgs>, handler);
        }
    }

    private static void EndInvoke<TArgs>(IAsyncResult result)
    {
        Debug.Assert(result.AsyncState is EventHandler<TArgs>);
        try
        {
            ((EventHandler<TArgs>)result.AsyncState).EndInvoke(result);
        }
        catch
        {
            // Swallow everything
        }
    }
}