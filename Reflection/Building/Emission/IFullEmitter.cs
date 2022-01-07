namespace Jay.Reflection.Emission;

public interface IFullEmitter<TEmitter> : IOpCodeEmitter<TEmitter>,
                                          IGenEmitter<TEmitter>
    where TEmitter : IFullEmitter<TEmitter>
{

}