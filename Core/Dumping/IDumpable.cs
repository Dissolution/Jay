using Jay.Text;

namespace Jay.Dumping;

public interface IDumpable
{
    void DumpTo(TextBuilder text);

    string Dump()
    {
        using var text = TextBuilder.Borrow();
        DumpTo(text);
        return text.ToString();
    }

    string? ToString() => Dump();
}