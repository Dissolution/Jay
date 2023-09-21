namespace Jay.Reflection.Emitting.Scratch;

public interface ITryCatchFinallyEmissions<out Self>
    where Self : IEmitter<Self>
{
    /// <summary>
    /// Begins an exception block for a non-filtered exception.
    /// </summary>
    /// <param name="label">
    /// The <see cref="Label"/> for the end of the block.
    /// This will leave you in the correct place to execute <see langword="finally"/> blocks or to finish the <see langword="try"/>.
    /// </param>
    /// <see href="https://docs.microsoft.com/en-us/dotnet/api/system.reflection.emit.ilgenerator.beginexceptionblock"/>
    Self BeginExceptionBlock(
        out EmitterLabel label,
        [CallerArgumentExpression(nameof(label))]
        string? labelName = null);

    /// <summary>
    /// Begins a <see langword="catch"/> block.
    /// </summary>
    /// <param name="exceptionType">The <see cref="Type"/> of <see cref="Exception"/>s to catch.</param>
    /// <exception cref="ArgumentNullException">If <paramref name="exceptionType"/> is <see langword="null"/>.</exception>
    /// <exception cref="ArgumentException">If <paramref name="exceptionType"/> is not an <see cref="Exception"/> type.</exception>
    /// <exception cref="ArgumentException">The catch block is within a filtered exception.</exception>
    /// <exception cref="NotSupportedException">The stream being emitted is not currently in an exception block.</exception>
    /// <see href="https://docs.microsoft.com/en-us/dotnet/api/system.reflection.emit.ilgenerator.begincatchblock"/>
    Self BeginCatchBlock(Type exceptionType);

    /// <summary>
    /// Begins a <see langword="catch"/> block.
    /// </summary>
    /// <typeparam name="TException">The <see cref="Type"/> of <see cref="Exception"/>s to catch.</typeparam>
    /// <exception cref="ArgumentException">The catch block is within a filtered exception.</exception>
    /// <exception cref="NotSupportedException">The stream being emitted is not currently in an exception block.</exception>
    /// <see href="https://docs.microsoft.com/en-us/dotnet/api/system.reflection.emit.ilgenerator.begincatchblock"/>
    Self BeginCatchBlock<TException>() where TException : Exception;

    /// <summary>
    /// Begins a <see langword="finally"/> block in the stream.
    /// </summary>
    /// <exception cref="NotSupportedException">The stream being emitted is not currently in an exception block.</exception>
    /// <see href="https://docs.microsoft.com/en-us/dotnet/api/system.reflection.emit.ilgenerator.beginfinallyblock"/>
    Self BeginFinallyBlock();

    /// <summary>
    /// Begins an exception block for a filtered exception.
    /// </summary>
    /// <exception cref="NotSupportedException">The stream being emitted is not currently in an exception block.</exception>
    /// <exception cref="NotSupportedException">This <see cref="Emission.IILGenerator{TGenerator}"/> belongs to a <see cref="DynamicMethod"/>.</exception>
    /// <see href="https://docs.microsoft.com/en-us/dotnet/api/system.reflection.emit.ilgenerator.beginexceptfilterblock"/>
    Self BeginExceptFilterBlock();

    /// <summary>
    /// Begins an exception fault block in the stream.
    /// </summary>
    /// <exception cref="NotSupportedException">The stream being emitted is not currently in an exception block.</exception>
    /// <exception cref="NotSupportedException">This <see cref="Emission.IILGenerator{TGenerator}"/> belongs to a <see cref="DynamicMethod"/>.</exception>
    /// <see href="https://docs.microsoft.com/en-us/dotnet/api/system.reflection.emit.ilgenerator.beginfaultblock"/>
    Self BeginFaultBlock();

    /// <summary>
    /// Ends an exception block.
    /// </summary>
    /// <exception cref="InvalidOperationException">If this operation occurs in an unexpected place in the stream.</exception>
    /// <exception cref="NotSupportedException">If the stream being emitted is not currently in an exception block.</exception>
    /// <see href="https://docs.microsoft.com/en-us/dotnet/api/system.reflection.emit.ilgenerator.endexceptionblock"/>
    Self EndExceptionBlock();

    /// <summary>
    /// Transfers control from the filter clause of an exception back to the Common Language Infrastructure (CLI) exception handler.
    /// </summary>
    /// <see href="https://docs.microsoft.com/en-us/dotnet/api/system.reflection.emit.opcodes.endfilter?view=netcore-3.0"/>
    Self Endfilter();

    /// <summary>
    /// Transfers control from the fault or finally clause of an exception block back to the Common Language Infrastructure (CLI) exception handler.
    /// </summary>
    /// <remarks>Note that the Endfault and Endfinally instructions are aliases - they correspond to the same opcode.</remarks>
    /// <see href="http://docs.microsoft.com/en-us/dotnet/api/system.reflection.emit.opcodes.endfinally"/>
    Self Endfault();

    /// <summary>
    /// Transfers control from the fault or finally clause of an exception block back to the Common Language Infrastructure (CLI) exception handler.
    /// </summary>
    /// <see href="http://docs.microsoft.com/en-us/dotnet/api/system.reflection.emit.opcodes.endfinally"/>
    Self Endfinally();

    /// <summary>
    /// Rethrows the current exception.
    /// </summary>
    /// <exception cref="NotSupportedException">The stream being emitted is not currently in an <see langword="catch"/> block.</exception>
    /// <see href="http://docs.microsoft.com/en-us/dotnet/api/system.reflection.emit.opcodes.rethrow"/>
    Self Rethrow();
}