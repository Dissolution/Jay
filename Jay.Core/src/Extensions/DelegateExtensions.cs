namespace Jay.Extensions;

public static class EventHandlerExtensions
{
    public static Task[] InvokeAllAsTasks<TArgs>(this EventHandler<TArgs>? eventHandler,
        object? sender,
        TArgs args)
    {
        // No subscribers?
        if (eventHandler is null) return Array.Empty<Task>();

        // capture a local copy
        var handlers = eventHandler.GetInvocationList();
        int count = handlers.Length;
        // No subscribers?
        if (count == 0) return Array.Empty<Task>();
        // We know how many we're returning
        var tasks = new Task[count];
        for (var i = 0; i < count; i++)
        {
            // Use the old FromAsync pattern to convert this to a Task!
            Task task = Task.Factory.FromAsync(
                (callback, state) =>
                {
                    var handler = state.AsValid<EventHandler<TArgs>>();
                    return handler.BeginInvoke(sender, args, callback, state);
                },
                result =>
                {
                    var handler = result.AsyncState.AsValid<EventHandler<TArgs>>();
                    try
                    {
                        handler.EndInvoke(result);
                    }
                    catch
                    {
                        // Swallow everything
                    }
                },
                handlers[i]);
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
        var handlers = eventHandler.GetInvocationList();
        for (var i = 0; i < handlers.Length; i++)
        {
            var handler = handlers[i].AsValid<EventHandler<TArgs>>();
            handler.BeginInvoke(sender, args, EndInvoke<TArgs>, handler);
        }
    }

    private static void EndInvoke<TArgs>(IAsyncResult result)
    {
        var handler = result.AsyncState.AsValid<EventHandler<TArgs>>();
        try
        {
            handler.EndInvoke(result);
        }
        catch
        {
            // Swallow everything
        }
    }
}