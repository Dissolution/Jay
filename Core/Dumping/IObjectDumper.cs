using Jay.Text;

namespace Jay.Dumping;

public interface IObjectDumper
{
    bool CanDump(Type objType);

    void DumpObject(TextBuilder text, object? obj, DumpOptions options = default);
}