using System.Linq.Expressions;
using System.Reflection;
using Jay.Expressions;
using Jay.Reflection.Exceptions;

namespace Jay.Reflection;

public static partial class Reflect
{
    public const BindingFlags AllFlags = BindingFlags.Public | BindingFlags.NonPublic |
                                         BindingFlags.Static | BindingFlags.Instance |
                                         BindingFlags.IgnoreCase;

    public const BindingFlags PublicFlags = BindingFlags.Public |
                                         BindingFlags.Static | BindingFlags.Instance |
                                         BindingFlags.IgnoreCase;

    public const BindingFlags NonPublicFlags = BindingFlags.NonPublic |
                                         BindingFlags.Static | BindingFlags.Instance |
                                         BindingFlags.IgnoreCase;

    public const BindingFlags StaticFlags = BindingFlags.Public | BindingFlags.NonPublic |
                                         BindingFlags.Static |
                                         BindingFlags.IgnoreCase;

    public const BindingFlags InstanceFlags = BindingFlags.Public | BindingFlags.NonPublic |
                                         BindingFlags.Instance |
                                         BindingFlags.IgnoreCase;

    public static IEnumerable<Type> AllExportedTypes()
    {
        return AppDomain.CurrentDomain
                        .GetAssemblies()
                        .Where(assembly => !assembly.IsDynamic)
                        .SelectMany(assembly => Result.InvokeOrDefault(() => assembly.ExportedTypes, Type.EmptyTypes));
    }
}

public static partial class Reflect
{
    public static Reflection<T> On<T>() => new Reflection<T>();
    
    public static TMember Get<TMember>(Expression<Action> memberExpression)
        where TMember : MemberInfo
    {
        var member = memberExpression.ExtractMember<TMember>();
        if (member is not null)
            return member;
        throw new ReflectionException($"Could not find {typeof(TMember)} from {memberExpression}");
    }
    
    public static TMember Get<TMember>(Expression<Func<object?>> memberExpression)
        where TMember : MemberInfo
    {
        var member = memberExpression.ExtractMember<TMember>();
        if (member is not null)
            return member;
        throw new ReflectionException($"Could not find {typeof(TMember)} from {memberExpression}");
    }
}


public class Reflection<T>
{
    public TMember Get<TMember>(Expression<Action<T>> memberExpression)
        where TMember : MemberInfo
    {
        var member = memberExpression.ExtractMember<TMember>();
        if (member is not null)
            return member;
        throw new ReflectionException($"Could not find {typeof(T)} {typeof(TMember)} from {memberExpression}");
    }
    
    public TMember Get<TMember>(Expression<Func<T, object?>> memberExpression)
        where TMember : MemberInfo
    {
        var member = memberExpression.ExtractMember<TMember>();
        if (member is not null)
            return member;
        throw new ReflectionException($"Could not find {typeof(T)} {typeof(TMember)} from {memberExpression}");
    }
}