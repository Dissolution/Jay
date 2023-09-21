namespace Jay.Reflection.Adapters;

public delegate void RemoveHandler<TInstance, in THandler>(
    [Instance] ref TInstance instance, 
    THandler eventHandler)
    where THandler : Delegate;