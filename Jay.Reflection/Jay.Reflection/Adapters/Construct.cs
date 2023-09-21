namespace Jay.Reflection.Adapters;

public delegate TInstance Construct<out TInstance>(
    params object?[] args);