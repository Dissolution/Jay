using System;
using System.Reflection.Emit;

namespace Jay.Reflection.Emission
{
    public interface ITryBuilder<out TBuilder>
        where TBuilder : IFluentILStream<TBuilder>
    {
        Label EndLabel { get; }

        ITryBuilder<TBuilder> Catch<TException>(Action<TBuilder> catchBlock)
            where TException : Exception;

        ITryBuilder<TBuilder> Catch(Type exceptionType, Action<TBuilder> catchBlock);

        ITryBuilder<TBuilder> Finally(Action<TBuilder> finallyBlock);
    }
}