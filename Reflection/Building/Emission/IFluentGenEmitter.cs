namespace Jay.Reflection.Emission;

public interface IFluentGenEmitter<TEmitter, TGen> : IEmitter
    where TEmitter : IFluentGenEmitter<TEmitter, TGen>
    where TGen : IGenEmitter<TGen>
{
    TGen Generator { get; }

    // TEmitter Try(Action<TEmitter> tryBlock, 
    //              Action<ICatchBlockBuilder<TEmitter>> catchBlocks, 
    //              Action<TEmitter> finallyBlock);

    TEmitter Scoped(Action<TEmitter> scopedEmission)
    {
        Generator.BeginScope();
        scopedEmission((TEmitter)this);
        Generator.EndScope();
        return (TEmitter)this;
    }
}