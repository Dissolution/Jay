namespace Jay.Reflection.Emitting.Scratch.Simple;

public interface IExceptionEmitter<out Self>
    where Self : ISmartEmitter<Self>
{
    Self Load<TException>()
        where TException : Exception, new();

    Self Load<TException>(params object[] exceptionCtorArgs)
        where TException : Exception;

    Self Load(Type exceptionType, params object[] exceptionCtorArgs);

    Self Throw();

    Self ThrowNew<TException>()
        where TException : Exception, new();

    Self ThrowNew<TException>(params object[] exceptionCtorArgs)
        where TException : Exception;

    Self ThrowNew(Type exceptionType, params object[] exceptionCtorArgs);
}