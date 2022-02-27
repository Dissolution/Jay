using System.Diagnostics;
using Jay.Reflection;
using Jay.Text;

namespace Jay.Dumping;

public interface IValueDumper<in T> : IObjectDumper
{
    bool IObjectDumper.CanDump(Type type)
    {
        return type.Implements<T>();
    }
    
    void DumpValue(TextBuilder text, T? value, DumpOptions options = default);
}


