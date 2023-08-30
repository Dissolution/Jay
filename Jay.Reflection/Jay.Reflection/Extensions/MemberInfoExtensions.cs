using System.Diagnostics;

namespace Jay.Reflection.Extensions;

public static class MemberInfoExtensions
{
    public static string GetFullName(this MemberInfo member)
    {
        if (member is Type type)
            return type.FullName!;
        return $"{member.DeclaringType!.Namespace}.{member.Name}";
    }
    
    public static Type OwnerType(this MemberInfo memberInfo)
    {
        return memberInfo.ReflectedType ?? memberInfo.DeclaringType ?? memberInfo.Module.GetType();
    }

    public static bool HasInstanceType(this MemberInfo member)
    {
        return member.TryGetInstanceType(out _);
    }
    
    public static Type? InstanceType(this MemberInfo member)
    {
        member.TryGetInstanceType(out var instanceType);
        return instanceType;
    }
    
    public static bool TryGetInstanceType(this MemberInfo member, [NotNullWhen(true)] out Type? instanceType)
    {
        if (member.IsStatic())
        {
            // static members have no instance
            instanceType = null;
            return false;
        }
        
        // Check first for reflected base type, then declared
        instanceType = member.ReflectedType ?? member.DeclaringType;
        if (instanceType is null)
        {
            // global module
            instanceType = null;
            return false;
        }
        
        #if DEBUG
        if (instanceType.IsStatic() || instanceType.IsByRef)
        {
            // Possible?
            Debugger.Break();
        }
        #endif
        
        // We found a needed instance type
        return true;
    }

    public static Visibility GetVisibility(this MemberInfo? memberInfo)
    {
        if (memberInfo is FieldInfo fieldInfo)
            return fieldInfo.Visibility();
        if (memberInfo is PropertyInfo propertyInfo)
            return propertyInfo.Visibility();
        if (memberInfo is EventInfo eventInfo)
            return eventInfo.Visibility();
        if (memberInfo is MethodBase methodBase)
            return methodBase.Visibility();
        if (memberInfo is Type type)
            return type.Visibility();
        return Visibility.None;
    }

    public static bool IsStatic(this MemberInfo? memberInfo)
    {
        return memberInfo switch
        {
            FieldInfo fieldInfo => fieldInfo.IsStatic,
            PropertyInfo propertyInfo => propertyInfo.IsStatic(),
            EventInfo eventInfo => eventInfo.IsStatic(),
            MethodBase methodBase => methodBase.IsStatic,
            Type type => type.IsStatic(),
            _ => false,
        };
    }

    public static bool HasAttribute<TAttribute>(this MemberInfo member)
        where TAttribute : Attribute
    {
        return Attribute.GetCustomAttribute(member, typeof(TAttribute), true) is not null;
    }

    public static int GenericTypeCount(this MemberInfo member)
    {
        return member switch
        {
            MethodBase method => method.GetGenericArguments().Length,
            Type type => type.GetGenericArguments().Length,
            _ => 0,
        };
    }
}