using System.Reflection;
using Jay.Collections;
using Jay.Reflection.Exceptions;

// ReSharper disable IdentifierTypo
// ReSharper disable InconsistentNaming

namespace Jay.Reflection.Internal;

internal static class MethodInfoCache
{
    private static readonly Lazy<MethodInfo> _runtimeHelpers_GetUninitializedObject_Method;
    private static readonly Lazy<MethodInfo> _type_GetTypeFromHandle_Method;
    private static readonly Lazy<MethodInfo> _multicastDelegate_GetInvocationList_Method;
    private static readonly Lazy<MethodInfo> _runtimeHelpers_IsReferenceOrContainsReferences_Method;
    private static readonly Lazy<MethodInfo> _delegate_Combine_Method;
    private static readonly Lazy<MethodInfo> _delegate_Remove_Method;
    
    private static readonly ConcurrentTypeDictionary<bool> _runtimeHelpers_IsReferenceOrContainsReferences_TypeCache;

    public static MethodInfo RuntimeHelpers_GetUninitializedObject =>
        _runtimeHelpers_GetUninitializedObject_Method.Value;

    public static MethodInfo Type_GetTypeFromHandle => 
        _type_GetTypeFromHandle_Method.Value;

    public static MethodInfo MulticastDelegate_GetInvocationList =>
        _multicastDelegate_GetInvocationList_Method.Value;

    public static MethodInfo Delegate_Combine => _delegate_Combine_Method.Value;
    public static MethodInfo Delegate_Remove => _delegate_Remove_Method.Value;

    static MethodInfoCache()
    {
        _runtimeHelpers_GetUninitializedObject_Method = new Lazy<MethodInfo>(() =>
        {
            var method = typeof(RuntimeHelpers).GetMethod(
                                                          nameof(RuntimeHelpers.GetUninitializedObject),
                                                          BindingFlags.Public | BindingFlags.Static,
                                                          new Type[1] { typeof(Type) });
            if (method is null)
                throw new RuntimeException($"Cannot find {nameof(RuntimeHelpers)}.{nameof(RuntimeHelpers.GetUninitializedObject)}({nameof(Type)})");
            return method;
        });
        _type_GetTypeFromHandle_Method = new Lazy<MethodInfo>(() =>
        {
            var method = typeof(Type).GetMethod(
                                                nameof(Type.GetTypeFromHandle),
                                                BindingFlags.Public | BindingFlags.Static,
                                                new Type[1] { typeof(RuntimeTypeHandle) });
            if (method is null)
                throw new RuntimeException($"Cannot find {nameof(Type)}.{nameof(Type.GetTypeFromHandle)}({nameof(RuntimeTypeHandle)})");
            return method;
        });
        _multicastDelegate_GetInvocationList_Method = new Lazy<MethodInfo>(() =>
        {
            var method = typeof(MulticastDelegate).GetMethod(
                                                             nameof(Delegate.GetInvocationList),
                                                             BindingFlags.Public | BindingFlags.Instance,
                                                             Type.EmptyTypes);
            if (method is null)
                throw new RuntimeException($"Cannot fine {nameof(MulticastDelegate)}.{nameof(Delegate.GetInvocationList)}()");
            return method;
        });
        _runtimeHelpers_IsReferenceOrContainsReferences_Method = new Lazy<MethodInfo>(() =>
        {
            var method = typeof(RuntimeHelpers).GetMethod(
                                                          nameof(RuntimeHelpers.IsReferenceOrContainsReferences),
                                                          BindingFlags.Public | BindingFlags.Static,
                                                          Type.EmptyTypes);
            if (method is null)
                throw new RuntimeException(
                                           $"Cannot fine {nameof(RuntimeHelpers)}.{nameof(RuntimeHelpers.IsReferenceOrContainsReferences)}<>()");
            return method;
        });
        _delegate_Combine_Method = new Lazy<MethodInfo>(() =>
        {
            var method = typeof(Delegate).GetMethod(
                                                    nameof(Delegate.Combine),
                                                    BindingFlags.Public | BindingFlags.Static,
                                                    new Type[2] { typeof(Delegate), typeof(Delegate) });
            if (method is null)
                throw new RuntimeException($"Cannot fine {nameof(Delegate)}.{nameof(Delegate.Combine)}({nameof(Delegate)},{nameof(Delegate)})");
            return method;
        });
        _delegate_Remove_Method = new Lazy<MethodInfo>(() =>
        {
            var method = typeof(Delegate).GetMethod(nameof(Delegate.Remove),
                                                    BindingFlags.Public | BindingFlags.Static,
                                                    new Type[2] { typeof(Delegate), typeof(Delegate) });
            if (method is null)
                throw new RuntimeException($"Cannot find {nameof(Delegate)}.{nameof(Delegate.Remove)}({nameof(Delegate)},{nameof(Delegate)})");
            return method;
        });
        
        _runtimeHelpers_IsReferenceOrContainsReferences_TypeCache = new();
    }

    public static bool IsReferenceOrContainsReferences(Type type)
    {
        return _runtimeHelpers_IsReferenceOrContainsReferences_TypeCache.GetOrAdd(type, 
            t => (bool)_runtimeHelpers_IsReferenceOrContainsReferences_Method.Value
                                                                             .MakeGenericMethod(t)
                                                                             .Invoke(null, null)!);
    }

    public static bool IsNonReferenced(Type type) => !IsReferenceOrContainsReferences(type);
}