namespace Jay.Reflection.Emission;

public interface IEmitter
{
    InstructionStream<Instruction> Instructions { get; }
}