namespace Jay.Reflection.Emitting;

public readonly record struct EmissionLine(int? Offset, Emission Emission);

public class EmissionStream : LinkedList<EmissionLine>, IToCode
{
    public bool TryFindByOffset(int offset, out EmissionLine emissionLine)
    {
        foreach (var line in this)
        {
            if (line.Offset.TryGetValue(out var lineOffset))
            {
                // found?
                if (lineOffset == offset)
                {
                    emissionLine = line;
                    return true;
                }
                // overshoot?
                if (lineOffset > offset) break;
            }
        }
        // dne
        emissionLine = default;
        return false;
    }
    
    public void RemoveAfter(LinkedListNode<EmissionLine>? node)
    {
        if (node is null) return;
        var last = Last;
        if (last is not null && last.Value != node.Value)
        {
            RemoveLast();
        }
    }

    public void WriteCodeTo(CodeBuilder codeBuilder)
    {
        foreach (var node in this)
        {
            codeBuilder.Append($"IL_{node.Offset:x4}: ");
            codeBuilder.Append(node.Emission);
            codeBuilder.NewLine();
        }
    }

    public override string ToString() => CodeBuilder.Render(this);
}