namespace Jay.Reflection.Emitting.Scratch;

public interface IEmitter<Self>
    where Self : IEmitter<Self>
{
    EmissionStream Emissions { get; }
    int ILStreamOffset { get; }
}