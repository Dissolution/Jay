namespace Jay.Reflection.Adapters;

public delegate void RaiseHandler<TInstance>(
    [Instance] ref TInstance instance, 
    params object?[] eventArgs);