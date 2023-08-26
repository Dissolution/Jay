namespace Jay.Reflection.Extensions;

public static class FieldInfoExtensions
{
    public static Visibility Visibility(this FieldInfo? fieldInfo)
    {
        Visibility visibility = Reflection.Visibility.None;
        if (fieldInfo is null)
            return visibility;
        if (fieldInfo.IsStatic)
            visibility |= Reflection.Visibility.Static;
        else
            visibility |= Reflection.Visibility.Instance;
        if (fieldInfo.IsPrivate)
            visibility |= Reflection.Visibility.Private;
        if (fieldInfo.IsFamily)
            visibility |= Reflection.Visibility.Protected;
        if (fieldInfo.IsAssembly)
            visibility |= Reflection.Visibility.Internal;
        if (fieldInfo.IsPublic)
            visibility |= Reflection.Visibility.Public;
        return visibility;
    }
}