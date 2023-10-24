namespace Jay.Extensions;

public static class EventHandlerExtensions
{
    public static void Raise<TArgs>(
        this EventHandler<TArgs>? eventHandler,
        object? sender, TArgs eventArgs)
    {
        // No subscribers?
        if (eventHandler is null)
            return;

        // get all the multicast delegates
        var handlers = eventHandler.GetInvocationList();
        int handlerCount = handlers.Length;
        // none, fast exit
        if (handlerCount == 0) return;
        // execute each
        for (var i = 0; i < handlerCount; i++)
        {
            if (handlers[i] is EventHandler<TArgs> handler)
            {
                try
                {
                    handler.Invoke(sender, eventArgs);
                }
                catch
                {
                    // Swallow everything
                }
            }
        }
    }
    
    public static async Task RaiseAsync<TArgs>(
        this EventHandler<TArgs>? eventHandler,
        object? sender, TArgs eventArgs)
    {
        // No subscribers?
        if (eventHandler is null)
            return;

        // capture a local copy
        var handlers = eventHandler.GetInvocationList();
        int handlerCount = handlers.Length;
        // No subscribers?
        if (handlerCount == 0) return;
        // We know how many we're returning
        var tasks = new Task[handlerCount];
        for (var i = 0; i < handlerCount; i++)
        {
            // Use the old FromAsync pattern to convert this to a Task!
            Task task = Task.Factory.FromAsync(
                (callback, state) =>
                {
                    var handler = state.AsValid<EventHandler<TArgs>>();
                    return handler.BeginInvoke(sender, eventArgs, callback, state);
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

        // they've all been started, wait for them to finish
        await Task.WhenAll(tasks);
    }
}