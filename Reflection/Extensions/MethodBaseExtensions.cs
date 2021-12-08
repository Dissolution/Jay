using System.Reflection;

namespace Jay.Reflection
{
    public static class MethodBaseExtensions
    {
        public static Access Access(this MethodBase? method)
        {
            Access access = Reflection.Access.None;
            if (method is null)
                return access;
            if (method.IsPrivate)
                access |= Reflection.Access.Private;
            if (method.IsFamily)
                access |= Reflection.Access.Protected;
            if (method.IsAssembly)
                access |= Reflection.Access.Internal;
            if (method.IsPublic)
                access |= Reflection.Access.Public;
            return access;
        }
    }
}