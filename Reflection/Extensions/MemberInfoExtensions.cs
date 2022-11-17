namespace Jay.Reflection.Extensions;

public static class MemberInfoExtensions
{
    public static bool HasAttribute<TAttribute>(this MemberInfo member)
        where TAttribute : Attribute
    {
        return Attribute.IsDefined(member, typeof(TAttribute));
    }
}