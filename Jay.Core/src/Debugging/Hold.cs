using System.Diagnostics;

namespace Jay.Debugging;

/// <summary>
/// Provides a reference to values at DEBUG time<br/>
/// So that they are not compiled out nor triggering compiler warnings
/// </summary>
public static class Hold
{
    [Conditional("DEBUG")]
    public static void Onto<T>(T _) { }
    [Conditional("DEBUG")]
    public static void Onto<T1, T2>(T1 _1, T2 _2) { }
    [Conditional("DEBUG")]
    public static void Onto<T1, T2, T3>(T1 _1, T2 _2, T3 _3) { }
    [Conditional("DEBUG")]
    public static void Onto<T1, T2, T3, T4>(T1 _1, T2 _2, T3 _3, T4 _4) { }
    [Conditional("DEBUG")]
    public static void Onto<T1, T2, T3, T4, T5>(T1 _1, T2 _2, T3 _3, T4 _4, T5 _5) { }
    [Conditional("DEBUG")]
    public static void Onto<T1, T2, T3, T4, T5, T6>(T1 _1, T2 _2, T3 _3, T4 _4, T5 _5, T6 _6) { }
    [Conditional("DEBUG")]
    public static void Onto<T1, T2, T3, T4, T5, T6, T7>(T1 _1, T2 _2, T3 _3, T4 _4, T5 _5, T6 _6, T7 _7) { }
    [Conditional("DEBUG")]
    public static void Onto<T1, T2, T3, T4, T5, T6, T7, T8>(T1 _1, T2 _2, T3 _3, T4 _4, T5 _5, T6 _6, T7 _7, T8 _8) { }
}