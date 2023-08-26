// ReSharper disable InconsistentNaming

using Jay.Reflection.Searching;
using Jay.Utilities;

namespace Jay.Reflection.Caching;

public static class MemberCache
{
    public static class Methods
    {
        public static MethodInfo Type_GetTypeFromHandle { get; }
        
        public static MethodInfo Delegate_GetInvocationList { get; }
        
        public static MethodInfo RuntimeHelpers_GetUninitializedObject { get; }

        public static MethodInfo Object_GetType { get; }
        
        static Methods()
        {
            Type_GetTypeFromHandle = MemberSearch.One<Type, MethodInfo>(nameof(Type.GetTypeFromHandle));
            Delegate_GetInvocationList = MemberSearch.One<Delegate, MethodInfo>(nameof(Delegate.GetInvocationList));
            RuntimeHelpers_GetUninitializedObject = MemberSearch.One<MethodInfo>(typeof(RuntimeHelpers), nameof(Scary.GetUninitializedObject));
            Object_GetType = MemberSearch.One<object, MethodInfo>(nameof(object.GetType));
        }
    }
}