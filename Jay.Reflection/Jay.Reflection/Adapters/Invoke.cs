namespace Jay.Reflection.Adapters;

public delegate TReturn Invoke<TInstance, out TReturn>(
    [Instance] ref TInstance instance, 
    params object?[] args);