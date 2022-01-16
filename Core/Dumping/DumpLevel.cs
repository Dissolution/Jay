namespace Jay.Dumping;

[Flags]
public enum DumpLevel
{
    Self = 0,
    Details = 1 << 0,
    Surroundings = 1 << 1,
}