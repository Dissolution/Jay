using Jay.Text;

namespace Jay.Dumping;

public interface IDumper
{
    bool CanDump(Type objType);

    void DumpObject(TextBuilder text, object? value, DumpLevel level = DumpLevel.Default);
}