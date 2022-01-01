namespace Jay.Reflection.Emission;

public interface IFluentOpEmitter<TEmitter, TOp> : IEmitter
    where TEmitter : IFluentOpEmitter<TEmitter, TOp>
    where TOp : IOpCodeEmitter<TOp>
{
    TOp OpCode { get; }

    TEmitter LoadArgument(int index)
    {
        OpCode.Ldarg(index);
        return (TEmitter)this;
    }

    TEmitter Cast(Type sourceType, Type destType)
    {
        OpCode.Cast(sourceType, destType);
        return (TEmitter)this;
    }
}