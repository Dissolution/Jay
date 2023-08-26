namespace Jay.Reflection;

public enum ParameterAccess
{
    Default,
    In,
    Ref,
    Out,
}

public static class ParameterAccessExtensions
{
    public static string ToCode(this ParameterAccess access)
    {
        return access switch
        {
            ParameterAccess.Default => string.Empty,
            ParameterAccess.In => "in",
            ParameterAccess.Ref => "ref",
            ParameterAccess.Out => "out",
            _ => throw new ArgumentOutOfRangeException(nameof(access), access, null),
        };
    }
}