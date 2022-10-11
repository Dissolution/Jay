using System.Diagnostics;
using Jay.Dumping;

namespace Jay.Debugging;

public static class Hold
{
    static Hold()
    {

    }
    
    [DebuggerHidden]
    [Conditional("DEBUG")]
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static void Stop()
    {
        if (Debugger.IsAttached)
            Debugger.Break();
    }

    [Conditional("DEBUG")]
    public static void DumpBreak<T>(T? value)
    {
        string dump = Dumper.Dump<T>(value);
        Stop();
    }
    
    [Conditional("DEBUG")]
    public static void Debug<T>(T? value){ }
}