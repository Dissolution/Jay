namespace Jay.Reflection.Deconstruction;

internal sealed class ThisParameter : ParameterInfo
{
    public ThisParameter(MemberInfo member)
    {
        MemberImpl = member;
        ClassImpl = member.DeclaringType;
        NameImpl = "this";
        PositionImpl = 0;
    }
}