using System.Diagnostics;
using Jay.Reflection;
using Jay.Text;

namespace Jay.Dumping;

public interface IDumper<in T> : IDumper
{
    bool IDumper.CanDump(Type type)
    {
        return type.Implements<T>();
    }

    void IDumper.DumpObject(TextBuilder text, object? value, DumpLevel level)
    {
        if (Dump.DumpNull(text, value, level)) return;
        if (value is T typed)
        {
            DumpValue(text, typed, level);
        }
        else
        {
            Debugger.Break();
            // Push handling back to Dump
            text.AppendDump(value, level);
        }
    }

    void DumpValue(TextBuilder text, T? value, DumpLevel level = DumpLevel.Default);
}


