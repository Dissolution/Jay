using Jay.CodeGen.Collections;

namespace Jay.CodeGen.Exceptions;

public static class Except
{
    private static readonly ConcurrentTypeMap<Delegate> _exceptionCtorCache = new();
    
    
    public static TException New<TException>(
        ref InterpolatedCode message,
        Exception? innerException = null)
        where TException : Exception
    {
        throw new NotImplementedException();
    }
    
    public static TException New<TException>(
        ref InterpolatedCode message,
        params object?[]? ctorArgs)
        where TException : Exception
    {
        throw new NotImplementedException();
    }
}