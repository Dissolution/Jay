using Jay.Text;

namespace Jay.Dumping;

public sealed class DelegateDumper<T>: IDumper<T>
{
    private readonly Action<TextBuilder, T?, DumpLevel> _dumpAction;
        
    public DelegateDumper(Action<TextBuilder, T?, DumpLevel> dumpAction)
    {
        _dumpAction = dumpAction;
    }

    public void DumpValue(TextBuilder text, T? value, DumpLevel level = DumpLevel.Default)
    {
        _dumpAction(text, value, level);
    }
}