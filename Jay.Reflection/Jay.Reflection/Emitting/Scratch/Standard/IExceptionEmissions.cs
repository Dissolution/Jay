using Jay.Reflection.Emitting.Scratch.Simple;

namespace Jay.Reflection.Emitting.Scratch.Standard;

public interface IExceptionEmissions<out Self>
    where Self : IEmitter<Self>
{
    /// <summary>
    /// Emits the instructions to throw an <see cref="ArithmeticException"/> if the value on the stack is not a finite number.
    /// </summary>
    /// <see href="http://docs.microsoft.com/en-us/dotnet/api/system.reflection.emit.opcodes.ckfinite"/>
    Self Ckfinite();

    /// <summary>
    /// Throws the <see cref="Exception"/> currently on the stack.
    /// </summary>
    /// <exception cref="NullReferenceException">If the <see cref="Exception"/> <see cref="object"/> on the stack is <see langword="null"/>.</exception>
    /// <see href="http://docs.microsoft.com/en-us/dotnet/api/system.reflection.emit.opcodes.throw"/>
    Self Throw();

    Self ThrowException(Type exceptionType);

    /// <summary>
    /// Emits the instructions to throw an <see cref="Exception"/>.
    /// </summary>
    /// <typeparam name="TException">The <see cref="Type"/> of <see cref="Exception"/> to throw.</typeparam>
    /// <see href="https://docs.microsoft.com/en-us/dotnet/api/system.reflection.emit.ilgenerator.throwexception?view=netcore-3.0"/>
    Self ThrowException<TException>()
        where TException : Exception, new();

    Self ThrowException<TException>(params object?[] exceptionArgs)
        where TException : Exception;
}