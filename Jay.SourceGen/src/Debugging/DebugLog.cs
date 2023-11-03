using Jay.SourceGen.Text;

namespace Jay.SourceGen.Debugging;

public static class DebugLog
{
    public static void Log(ref InterpolatedCode code)
    {
        string message = CodeBuilder.New
            .Append('[')
            .Append(DateTime.Now, "HH:mm:ss.ff")
            .Append("]: ")
            .Append(ref code)
            .ToStringAndDispose();
        Debug.WriteLine(message);
    }
}