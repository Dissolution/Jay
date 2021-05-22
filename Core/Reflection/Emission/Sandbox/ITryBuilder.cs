using System;
using System.Collections;
using System.Reflection.Emit;

namespace Jay.Reflection.Emission.Sandbox
{
    public interface ITryBuilder<TEmitter>
        where TEmitter : IFluentILEmitter<TEmitter>
    {
        Label EndLabel { get; }

        ITryBuilder<TEmitter> Catch<TException>(Action<TEmitter> catchBlock)
            where TException : Exception;

        ITryBuilder<TEmitter> Catch(Type exceptionType, Action<TEmitter> catchBlock);

        ITryBuilder<TEmitter> Finally(Action<TEmitter> finallyBlock);
    }
}