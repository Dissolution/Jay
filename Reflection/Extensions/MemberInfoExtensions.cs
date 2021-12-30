using System;
using System.Reflection;

namespace Jay.Reflection;

public static class MemberInfoExtensions
{
    public static Type? OwnerType(this MemberInfo? memberInfo)
    {
        return memberInfo?.ReflectedType ??
               memberInfo?.DeclaringType;
    }

    public static bool TryGetInstanceType(this MemberInfo? memberInfo, out Type? instanceType)
    {
        var ownerType = memberInfo.OwnerType();
        if (ownerType is null)
        {
            instanceType = null;
            return false;
        }

        instanceType = ownerType;
        return !instanceType.IsStatic();
    }

    public static Visibility Access(this MemberInfo? memberInfo)
    {
        if (memberInfo is FieldInfo fieldInfo)
            return fieldInfo.Access();
        if (memberInfo is PropertyInfo propertyInfo)
            return propertyInfo.Access();
        if (memberInfo is EventInfo eventInfo)
            return eventInfo.Access();
        if (memberInfo is ConstructorInfo constructorInfo)
            return constructorInfo.Access();
        if (memberInfo is MethodBase methodBase)
            return methodBase.Access();
        if (memberInfo is Type type)
            return type.Access();
        return Reflection.Visibility.None;
    }
}