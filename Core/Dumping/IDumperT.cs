using Jay.Text;

namespace Jay.Dumping;

public interface IDumper<in T> : IDumper
{
    bool IDumper.CanDump(Type type)
    {
        return type.IsAssignableTo(typeof(T));
    }

    void IDumper.Dump(TextBuilder text, object? value, DumpLevel level)
    {
        if (Dumpers.DumpNull(text, value, level)) return;
        if (value is T typed)
        {
            Dump(text, typed, level);
        }
        else
        {
            text.AppendDump(value, level);
        }
    }

    void Dump(TextBuilder text, T? value, DumpLevel level = DumpLevel.Self);
}


