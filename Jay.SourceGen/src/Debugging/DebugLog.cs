namespace Jay.SourceGen.Debugging;

public static class DebugLog
{
    public static void Log(string message)
    {
        string msg = CodeBuilder.New
            .Append('[')
            .Append(DateTime.Now, "HH:mm:ss.ff")
            .Append("]: ")
            .Append(message)
            .ToStringAndDispose();
        Debug.WriteLine(msg);
    }

    public static void Log(Exception ex, string? message = null)
    {
        string msg = CodeBuilder.New
            .Append('[')
            .Append(DateTime.Now, "HH:mm:ss.ff")
            .Append("]: ")
            .Append(message)
            .NewLine()
            .Append(ex.GetType()).NewLine()
            .Append(ex.Message)
            .ToStringAndDispose();
        Debug.WriteLine(msg);
    }
}