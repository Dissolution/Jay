using Jay.Text;

namespace Jay.Dumping;

public abstract class Dumper : IObjectDumper
{
    internal static bool DumpNull<T>(TextBuilder text, [AllowNull, NotNullWhen(false)] T value, DumpOptions options)
    {
        if (value is null)
        {
            if (options.Detailed)
            {
                text.Append('(')
                    .AppendDump(typeof(T))
                    .Write(')');
            }
            text.Write("null");
            return true;
        }
        return false;
    }

    /// <inheritdoc />
    public abstract bool CanDump(Type objType);

    /// <inheritdoc />
    public virtual void DumpObject(TextBuilder text, object? obj, DumpOptions options = default)
    {
        
    }
}

public abstract class Dumper<T> : Dumper, IValueDumper<T>
{
    public override bool CanDump(Type objType)
    {
        return objType.Implements<T>();
    }

    public sealed override void DumpObject(TextBuilder text, object? obj, DumpOptions options = default)
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

    /// <inheritdoc />
    public abstract void DumpValue(TextBuilder text, T? value, DumpOptions options = default);
}