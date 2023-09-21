namespace Jay.Reflection.Adapters;

public delegate void AddHandler<TInstance, in THandler>(
    [Instance] ref TInstance instance, 
    THandler eventHandler)
    where THandler : Delegate;