using System;

namespace Jay.Debugging.Dumping.Temp
{
    [Flags]
    public enum DumpOptions
    {
        Surface = 0,
        
        Type = 1 << 0,
    }
}