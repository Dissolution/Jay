namespace Jay.Reflection.Emission;

public static class InstructionStreamExtensions
{
    public static OpCodeInstruction? FindWithOffset(this InstructionStream<OpCodeInstruction> instructions, int offset)
    {
        if (offset < 0 || instructions.Count == 0)
            return null;
        foreach (var node in instructions)
        {
            if (node.Offset == offset)
                return node;
            if (node.Offset > offset)
                return null;
        }
        return null;
    }
}