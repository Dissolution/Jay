namespace Jay.Reflection.Emitting;

/// <summary>
/// A fluent way to emit try/catch/finally blocks
/// </summary>
/// <typeparam name="TEmitter">The <see cref="FluentEmitter{T}"/> to be resumed upon <see cref="Finally()"/></typeparam>
public sealed class TCFEmitter<TEmitter>
    where TEmitter : FluentEmitter<TEmitter>
{
    private readonly TEmitter _emitter;
    private EmitLabel _endTryLabel;

    /// <summary>
    /// Gets the <see cref="EmitLabel"/> that marks the end of this try/catch block
    /// </summary>
    public EmitLabel? EndTryLabel { get; private set; }
    
    internal TCFEmitter(TEmitter emitter)
    {
        _emitter = emitter;
        _endTryLabel = default;
    }
    
    /// <summary>
    /// Starts a <c>try<c> block containing <paramref name="tryBlock"/>
    /// </summary>
    internal TCFEmitter<TEmitter> Try(Action<TEmitter> tryBlock)
    {
        _emitter.BeginExceptionBlock(out _endTryLabel);
        tryBlock(_emitter);
        return this;
    }

    /// <summary>
    /// Adds a <c>catch</c> <paramref name="catchBlock"/> and continues Try/Catch/Finally building
    /// </summary>
    /// <typeparam name="TException">The <see cref="Type"/> of <see cref="Exception"/>s to be caught</typeparam>
    /// <param name="catchBlock">The instructions to emit during this <c>catch</c> block</param>
    /// <returns>This <see cref="TCFEmitter{TEmitter}"/></returns>
    public TCFEmitter<TEmitter> Catch<TException>(Action<TEmitter> catchBlock)
        where TException : Exception 
        => Catch(typeof(TException), catchBlock);
    
    /// <summary>
    /// Adds a <c>catch</c> <paramref name="catchBlock"/> and continues Try/Catch/Finally building
    /// </summary>
    /// <param name="exceptionType">The <see cref="Type"/> of <see cref="Exception"/>s to be caught</param>
    /// <param name="catchBlock">The instructions to emit during this <c>catch</c> block</param>
    /// <returns>This <see cref="TCFEmitter{TEmitter}"/></returns>
    public TCFEmitter<TEmitter> Catch(Type exceptionType, Action<TEmitter> catchBlock)
    {
        _emitter.BeginCatchBlock(exceptionType);
        catchBlock(_emitter);
        return this;
    }

    /// <summary>
    /// Ends this <c>catch</c> block
    /// </summary>
    public TEmitter Finally() => _emitter;
    
    /// <summary>
    /// Adds a <c>finally</c> <paramref name="finallyBlock"/> and returns back to emission
    /// </summary>
    public TEmitter Finally(Action<TEmitter>? finallyBlock)
    {
        if (finallyBlock is not null)
        {
            _emitter.BeginFinallyBlock();
            finallyBlock(_emitter);
            _emitter.EndExceptionBlock();
        }
        return _emitter;
    }
}
