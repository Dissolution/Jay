using System;

namespace Jay.Reflection
{
    [Flags]
    public enum Access
    {
        None = 0,
        Private = 1 << 0,
        Protected = 1 << 1,
        Internal = 1 << 2,
        Public = 1 << 3,
        NonPublic = Private | Protected | Internal,
        Any = Private | Protected | Internal | Public,
    }
}