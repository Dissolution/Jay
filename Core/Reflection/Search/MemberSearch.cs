using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Linq.Expressions;
using System.Reflection;
using Jay.Reflection.Expressions;

namespace Jay.Reflection.Search;

public static class MemberSearch
{
    public static Result TryFind<TMember>(Expression memberExpression,
                                          [NotNullWhen(true)] out TMember? member)
        where TMember : MemberInfo
    {
        member = memberExpression.ExtractMember<TMember>();
        if (member is null)
        {
            Debugger.Break();
            return new MissingMemberException();
        }
        return true;
    }

    public static bool HasDefaultConstructor(this Type type, [NotNullWhen(true)] out ConstructorInfo? ctor)
    {
        ctor = type.GetConstructor(Reflect.InstanceFlags, Type.EmptyTypes);
        return ctor is not null;
    }
}