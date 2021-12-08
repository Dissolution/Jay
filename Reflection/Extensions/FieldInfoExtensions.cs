using System.Reflection;

namespace Jay.Reflection
{
    public static class FieldInfoExtensions
    {
        public static Access Access(this FieldInfo? fieldInfo)
        {
            Access access = Reflection.Access.None;
            if (fieldInfo is null)
                return access;
            if (fieldInfo.IsPrivate)
                access |= Reflection.Access.Private;
            if (fieldInfo.IsFamily)
                access |= Reflection.Access.Protected;
            if (fieldInfo.IsAssembly)
                access |= Reflection.Access.Internal;
            if (fieldInfo.IsPublic)
                access |= Reflection.Access.Public;
            return access;
        }
    }
}