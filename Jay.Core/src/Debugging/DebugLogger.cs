using System.Diagnostics;

namespace Jay.Debugging;

public static class DebugLogger
{
    [Conditional("DEBUG")]
    public static void WriteLine(FormattableString text)
    {
        string format = text.Format;
        if (text.ArgumentCount == 0)
        {
            Debug.WriteLine($@"{DateTime.Now:HH\:mm\:ss\.ff}: {format}");
        }
        else
        {
            var args = text.GetArguments();
            // Process
            Debugger.Break();
            Debug.WriteLine($@"{DateTime.Now:HH\:mm\:ss\.ff}: {string.Format(format, args)}");
        }
    }
}
