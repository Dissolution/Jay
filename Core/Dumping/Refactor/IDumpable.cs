using Jay.Text;

namespace Jay.Dumping.Refactor;

public interface IDumpable
{
    /// <summary>
    /// Dumps this value to a <paramref name="textBuilder"/> with <paramref name="options"/>
    /// </summary>
    /// <param name="textBuilder"></param>
    /// <param name="options"></param>
    void DumpTo(TextBuilder textBuilder, DumpOptions? options = default);
}