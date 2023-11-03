namespace Jay.SourceGen;

[Flags]
public enum Visibility
{
    None = 0,
    
    Instance = 1 << 0,
    Static = 1 << 1,
    
    Private = 1 << 2,
    Protected = 1 << 3,
    Internal = 1 << 4,
    
    NonPublic = Private | Protected | Internal,
    Public = 1 << 5,
    
    Any = Instance | Static | Public | NonPublic,
}