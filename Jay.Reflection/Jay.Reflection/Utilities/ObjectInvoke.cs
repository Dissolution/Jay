using Jay.Reflection.Adapters;

namespace Jay.Reflection.Utilities;

internal delegate object? ObjectInvoke([Instance] object? instance, params object?[] args);