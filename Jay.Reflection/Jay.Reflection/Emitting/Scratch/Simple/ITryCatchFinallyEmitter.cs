namespace Jay.Reflection.Emitting.Scratch.Simple;

public interface ITryCatchFinallyEmitter<out TEmitter>
    where TEmitter : ISmartEmitter<TEmitter>
{
    EmitterLabel? EndTryBlockLabel { get; }

    /// <summary>
    /// Adds a <c>catch</c> <paramref name="catchBlock"/> and continues Try/Catch/Finally building
    /// </summary>
    /// <typeparam name="TException">The <see cref="Type"/> of <see cref="Exception"/>s to be caught</typeparam>
    /// <param name="catchBlock">The instructions to emit during this <c>catch</c> block</param>
    /// <returns>This <see cref="TryCatchFinallyEmitter{TEmitter}"/></returns>
    ITryCatchFinallyEmitter<TEmitter> Catch<TException>(Action<TEmitter> catchBlock)
        where TException : Exception;

    /// <summary>
    /// Adds a <c>catch</c> <paramref name="catchBlock"/> and continues Try/Catch/Finally building
    /// </summary>
    /// <param name="exceptionType">The <see cref="Type"/> of <see cref="Exception"/>s to be caught</param>
    /// <param name="catchBlock">The instructions to emit during this <c>catch</c> block</param>
    /// <returns>This <see cref="TryCatchFinallyEmitter{TEmitter}"/></returns>
    ITryCatchFinallyEmitter<TEmitter> Catch(Type exceptionType, Action<TEmitter> catchBlock);

    /// <summary>
    /// Ends this <c>catch</c> block
    /// </summary>
    TEmitter Finally();

    /// <summary>
    /// Adds a <c>finally</c> <paramref name="finallyBlock"/> and returns back to emission
    /// </summary>
    TEmitter Finally(Action<TEmitter>? finallyBlock);

    /// <summary>
    /// Rethrows the current exception.
    /// </summary>
    /// <exception cref="NotSupportedException">The stream being emitted is not currently in an <see langword="catch"/> block.</exception>
    /// <see href="http://docs.microsoft.com/en-us/dotnet/api/system.reflection.emit.opcodes.rethrow"/>
    TEmitter ReThrow();
}