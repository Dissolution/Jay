namespace Jay.Text.Dumping;

public interface IDumper<in T> : IDumper
{
    bool IDumper.CanDump(Type type)
    {
        return type.IsAssignableTo(typeof(T));
    }

    void IDumper.Dump(TextBuilder text, object? value, DumpLevel level)
    {
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

    string Dump(T? value, DumpLevel level = DumpLevel.Self)
    {
        using var text = new TextBuilder();
        Dump(text, value, level);
        return text.ToString();
    }
}


