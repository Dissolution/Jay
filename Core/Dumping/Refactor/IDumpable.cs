using Jay.Text;

namespace Jay.Dumping.Refactor2;

public interface IDumpable
{
    void DumpTo(TextBuilder text);

    string Dump()
    {
        using var text = TextBuilder.Borrow();
        DumpTo(text);
        return text.ToString();
    }
}