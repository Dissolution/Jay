using System.Collections.Concurrent;
using Jay.Reflection.Exceptions;

// I use _ in 'constant' member names for readability
// ReSharper disable InconsistentNaming

namespace Jay.Reflection.Caching;

public static class MemberCache
{
    internal static class Methods
    {
        public static MethodInfo Type_GetTypeFromHandle { get; } =
            Searching.MemberSearch.FindMethod<Type>(new(
                nameof(Type.GetTypeFromHandle),
                Visibility.Public | Visibility.Static,
                typeof(Type),
                typeof(RuntimeTypeHandle)));

        public static MethodInfo Delegate_GetInvocationList { get; } =
            Searching.MemberSearch.FindMethod<Delegate>(new(
                nameof(Delegate.GetInvocationList), 
                Visibility.Public | Visibility.Instance, 
                typeof(Delegate[])));


        public static MethodInfo RuntimeHelpers_GetUninitializedObject { get; } =
            Searching.MemberSearch.FindMethod(typeof(RuntimeHelpers), new(
                    nameof(RuntimeHelpers.GetUninitializedObject),
                    Visibility.Public | Visibility.Static,
                    typeof(object),
                    typeof(Type)));

        public static MethodInfo Object_GetType { get; } =
            Searching.MemberSearch.FindMethod<object>(new(
                    nameof(object.GetType),
                    Visibility.Public | Visibility.Instance,
                    typeof(Type)));
    }
    
    private static readonly ConcurrentDictionary<string, MemberInfo> _stringMemberCache = new();

    public static TMember GetOrAdd<TMember>(string key, TMember newMember)
        where TMember : MemberInfo
    {
        var memberInfo = _stringMemberCache.GetOrAdd(key, newMember);
        if (memberInfo is TMember member) return member;
        throw new JayflectException($"There is an {memberInfo.GetType()} defined for \"{key}\" when a {typeof(TMember)} was requested");
    }
    
    public static TMember GetOrAdd<TMember>(string key, Func<TMember> newMember)
        where TMember : MemberInfo
    {
        var memberInfo = _stringMemberCache.GetOrAdd(key, _ => newMember());
        if (memberInfo is TMember member) return member;
        throw new JayflectException($"There is an {memberInfo.GetType()} defined for \"{key}\" when a {typeof(TMember)} was requested");
    }
}