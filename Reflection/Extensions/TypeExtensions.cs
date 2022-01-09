using System.Diagnostics;

namespace Jay.Reflection;

public static class TypeExtensions
{
    public static Visibility Visibility(this Type? type)
    {
        Visibility visibility = Reflection.Visibility.None;
        if (type is null)
        {
                
        }
        else if (type.IsNested)
        {
            if (type.IsNestedPrivate)
                visibility |= Reflection.Visibility.Private;
            if (type.IsNestedFamily)
                visibility |= Reflection.Visibility.Protected;
            if (type.IsNestedAssembly)
                visibility |= Reflection.Visibility.Internal;
            if (type.IsNestedPublic)
                visibility |= Reflection.Visibility.Public;
        }
        else
        {
            if (type.IsPublic)
                visibility |= Reflection.Visibility.Public;
            if (type.IsNotPublic)
            {
                Debugger.Break();
                visibility |= Reflection.Visibility.NonPublic;
            }
        }
        return visibility;
    }

    public static bool IsStatic(this Type type)
    {
        return type.IsAbstract && type.IsSealed;
    }

    public static IEnumerable<MemberInfo> Search(this Type? type, MemberSearch memberSearch)
    {
        if (type is null)
            yield break;
        foreach (var member in type.GetMembers(Reflect.AllFlags))
        {
            if (memberSearch.Matches(member))
                yield return member;
        }
    }

    public static bool Implements<T>(this Type? type) => Implements(type, typeof(T));

    public static bool Implements(this Type? type, Type? otherType)
    {
        if (type == otherType) return true;
        if (type is null || otherType is null) return false;
        if (type.IsAssignableTo(otherType)) return true;
        // TODO: OTHER CHECKS w/INTERFACES
        return false;
    }
}