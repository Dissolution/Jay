using System.Reflection;

namespace Jay.Reflection;

public static class FieldInfoExtensions
{
    public static Visibility Visibility(this FieldInfo? fieldInfo)
    {
        Visibility visibility = Reflection.Visibility.None;
        if (fieldInfo is null)
            return visibility;
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