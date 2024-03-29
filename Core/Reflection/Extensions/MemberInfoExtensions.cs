﻿using System.Reflection;

namespace Jay.Reflection;

public static class MemberInfoExtensions
{
    public static Type? OwnerType(this MemberInfo? memberInfo)
    {
        return memberInfo?.ReflectedType ??
               memberInfo?.DeclaringType;
    }

    internal static Result TryGetInstanceType(this MemberInfo? memberInfo, [NotNullWhen(true)] out Type? instanceType)
    {
        if (memberInfo is null)
        {
            instanceType = default;
            return new ArgumentNullException(nameof(memberInfo));
        }

        if (memberInfo.IsStatic())
        {
            instanceType = default;
            return new ArgumentException("The given member is static or belongs to a static instance",
                                         nameof(memberInfo));
        }

        instanceType = memberInfo.OwnerType();
        if (instanceType is null)
        {
            return new ArgumentException("The given member does not have a ReflectedType nor DeclaringType",
                                         nameof(memberInfo));
        }

        if (instanceType.IsStatic())
        {
            return new ArgumentException("The given member is static or belongs to a static instance",
                                         nameof(memberInfo));
        }

        // We want ref instance for structs
        if (instanceType.IsValueType)
        {
            instanceType = instanceType.MakeByRefType();
        }
        return true;
    }

    public static Visibility Visibility(this MemberInfo? memberInfo)
    {
        if (memberInfo is FieldInfo fieldInfo)
            return fieldInfo.Visibility();
        if (memberInfo is PropertyInfo propertyInfo)
            return propertyInfo.Visibility();
        if (memberInfo is EventInfo eventInfo)
            return eventInfo.Visibility();
        if (memberInfo is ConstructorInfo constructorInfo)
            return constructorInfo.Visibility();
        if (memberInfo is MethodBase methodBase)
            return methodBase.Visibility();
        if (memberInfo is Type type)
            return type.Visibility();
        return Reflection.Visibility.None;
    }

    public static bool IsStatic(this MemberInfo? memberInfo)
    {
        if (memberInfo is FieldInfo fieldInfo)
            return fieldInfo.IsStatic;
        if (memberInfo is PropertyInfo propertyInfo)
            return propertyInfo.IsStatic();
        if (memberInfo is EventInfo eventInfo)
            return eventInfo.IsStatic();
        if (memberInfo is MethodBase methodBase)
            return methodBase.IsStatic;
        if (memberInfo is Type type)
            return type.IsStatic();
        return false;
    }
}