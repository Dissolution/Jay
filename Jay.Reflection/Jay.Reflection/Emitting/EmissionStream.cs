namespace Jay.Reflection.Emitting;

public class EmissionStream : LinkedList<EmissionLine>
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

    public override string ToString()
    {
        return TextBuilder.New
            .Enumerate(this, (tb, node) => tb
                .Append($"IL_{node.Offset:x4}: ")
                .Append(node.Emission)
                .NewLine())
            .ToStringAndDispose();
    }
}