using System.Reflection;

namespace Jay.Reflection
{
    public static class FieldInfoExtensions
    {
        public static Visibility GetVisibility(this FieldInfo? fieldInfo)
        {
            Visibility visibility = default;
            if (fieldInfo is null)
                return visibility;
            if (fieldInfo.IsPrivate)
                visibility |= Visibility.Private;
            if (fieldInfo.IsFamily)
                visibility |= Visibility.Protected;
            if (fieldInfo.IsAssembly)
                visibility |= Visibility.Internal;
            if (fieldInfo.IsFamilyAndAssembly || fieldInfo.IsFamilyOrAssembly)
                visibility |= (Visibility.Protected | Visibility.Internal);
            if (fieldInfo.IsPublic)
                visibility |= Visibility.Public;
            return visibility;
        }
    }
}