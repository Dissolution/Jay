using Jay.Dumping;

namespace Jay.Reflection;

[Flags]
public enum Visibility
{
    None = 0,
    [DumpAs("private")]
    Private = 1 << 0,
    [DumpAs("protected")]
    Protected = 1 << 1,
    [DumpAs("internal")]
    Internal = 1 << 2,
    [DumpAs("public")]
    Public = 1 << 3,
    [DumpAs("protected internal")]
    NonPublic = Private | Protected | Internal,
    [DumpAs("")]
    Any = Private | Protected | Internal | Public,
}