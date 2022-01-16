using Jay.Text;

namespace Jay.Dumping;

public interface IDumper
{
    bool CanDump(Type type);

    void Dump(TextBuilder text, object? value, DumpLevel level = DumpLevel.Self);

    string Dump(object? value, DumpLevel level = DumpLevel.Self)
    {
        using var text = new TextBuilder();
        Dump(text, value, level);
        return text.ToString();
    }
}