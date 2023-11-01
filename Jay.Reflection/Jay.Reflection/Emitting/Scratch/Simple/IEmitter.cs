namespace Jay.Reflection.Emitting.Scratch.Simple;

public interface IEmitter<Self>
    where Self : IEmitter<Self>
{
    EmissionStream Emissions { get; }
    int ILStreamOffset { get; }
}