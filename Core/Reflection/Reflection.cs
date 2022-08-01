using System.Linq.Expressions;
using System.Reflection;
using Jay.Expressions;
using Jay.Reflection.Exceptions;

namespace Jay.Reflection;

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

    public ConstructorInfo GetConstructor(params Type[] parameterTypes)
    {
        var ctor = typeof(T).GetConstructors(Reflect.InstanceFlags)
            .Where(ctor => MemoryExtensions.SequenceEqual<Type>(ctor.GetParameterTypes(), parameterTypes))
            .OneOrDefault();
        if (ctor is not null)
            return ctor;
        throw new ReflectionException($"Could not find {typeof(T)} constructor with parameter types: {parameterTypes}");
    }
}