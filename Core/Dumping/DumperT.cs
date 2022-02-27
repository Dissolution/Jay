using Jay.Reflection;
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
    public abstract void DumpObject(TextBuilder text, object? obj, DumpOptions options = default);
}

public abstract class Dumper<T> : Dumper, IValueDumper<T>
{
    public override bool CanDump(Type objType)
    {
        return objType.Implements<T>();
    }

    public sealed override void DumpObject(TextBuilder text, object? obj, DumpOptions options = default)
    {
        if (obj is null)
        {
            DumpNull<T>(text, default, options);
        }
        else if (obj is T typed)
        {
            DumpValue(text, typed, options);
        }
        else
        {
            throw new InvalidOperationException();
        }
    }

    /// <inheritdoc />
    public abstract void DumpValue(TextBuilder text, T? value, DumpOptions options = default);
}