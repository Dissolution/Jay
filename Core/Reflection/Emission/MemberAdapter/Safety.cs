using System;

namespace Jay.Reflection.Emission
{
    [Flags]
    public enum Safety
    {
        Safe = 0,
        AllowInToRef = 1 << 0,
        AllowOutToRef = 1 << 1,
        AllowInToOut = 1 << 2,
        AllowReturnDefaultFromVoid = 1 << 3,
        IgnoreExtraParams = 1 << 4,
        AllowPopToVoid = 1 << 5,
    }
}