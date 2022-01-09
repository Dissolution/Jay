namespace Jay.Reflection.Emission;

public class InstructionStream : LinkedList<Instruction>
{
    public Instruction? FindWithOffset(int offset)
    {
        if (offset < 0 || this.Count == 0)
            return null;
        foreach (var node in this)
        {
            if (node.Offset == offset)
                return node;
            if (node.Offset > offset)
                return null;
        }
        return null;
    }

}