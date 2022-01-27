namespace Jay.Dumping;

[Flags]
public enum DumpLevel
{
    Default = 0,
    Detailed = 1 << 0,
}