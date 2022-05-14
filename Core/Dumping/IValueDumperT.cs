using Jay.Text;

namespace Jay.Dumping;

public interface IValueDumper<in T> : IObjectDumper
{
    bool IObjectDumper.CanDump(Type type)
    {
        return type.Implements<T>();
    }

    
    void IObjectDumper.DumpObject(TextBuilder text, object? obj, DumpOptions options)
    {
        if (Dumper.DumpNull(text, obj, options)) return;
        if (obj is T value)
        {
            DumpValue(text, value, options);
        }
        else
        {
            throw Dump.GetException<InvalidOperationException>($"{GetType()} cannot dump a {obj.GetType()} value");
        }
    }

    
    void DumpValue(TextBuilder text, T? value, DumpOptions options = default);
}


