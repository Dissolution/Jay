using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Reflection;

namespace Jay.Reflection;

public static class TypeExtensions
{
    public static Access Access(this Type? type)
    {
        Access access = Reflection.Access.None;
        if (type is null)
        {
                
        }
        else if (type.IsNested)
        {
            if (type.IsNestedPrivate)
                access |= Reflection.Access.Private;
            if (type.IsNestedFamily)
                access |= Reflection.Access.Protected;
            if (type.IsNestedAssembly)
                access |= Reflection.Access.Internal;
            if (type.IsNestedPublic)
                access |= Reflection.Access.Public;
        }
        else
        {
            if (type.IsPublic)
                access |= Reflection.Access.Public;
            if (type.IsNotPublic)
            {
                Debugger.Break();
                access |= Reflection.Access.NonPublic;
            }
        }
        return access;
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
}