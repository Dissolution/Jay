using System;
using System.Diagnostics;
using System.Reflection;
using Jay.Reflection.Emission;

namespace Jay.Reflection
{
    public static class MemberInfoExtensions
    {
        public static bool IsStatic(this MemberInfo? memberInfo)
        {
            return memberInfo switch
                   {
                       FieldInfo fieldInfo => fieldInfo.IsStatic,
                       PropertyInfo propertyInfo => propertyInfo.IsStatic(),
                       EventInfo eventInfo => eventInfo.IsStatic(),
                       MethodBase methodBase => methodBase.IsStatic,
                       _ => false
                   };
        }
        
        public static Visibility GetVisibility(this MemberInfo? memberInfo)
        {
            return memberInfo switch
                   {
                       FieldInfo fieldInfo => fieldInfo.GetVisibility(),
                       PropertyInfo propertyInfo => propertyInfo.GetVisibility(),
                       EventInfo eventInfo => eventInfo.GetVisibility(),
                       MethodBase methodBase => methodBase.GetVisibility(),
                       _ => default
                   };
        }
        
        public static Type InstanceType(this MemberInfo memberInfo)
        {
            Type? instanceType = memberInfo.ReflectedType;
            if (instanceType is null)
            {
                instanceType = memberInfo.DeclaringType;
                if (instanceType is null)
                {
                    return typeof(void);
                }
            }
            return instanceType;
        }
        
        internal static ArgumentType GetInstanceAdapterType(this MemberInfo memberInfo)
        {
            Type? instanceType = memberInfo.ReflectedType;
            if (instanceType is null)
            {
                instanceType = memberInfo.DeclaringType;
                if (instanceType is null)
                {
                    return new ArgumentType(typeof(void));
                }
            }

            if (instanceType.IsValueType)
            {
                return new ArgumentType(instanceType.MakeByRefType());
            }

            return new ArgumentType(instanceType);
        }
        
        public static Type GetReturnType(this MemberInfo? member)
        {
            if (member is null)
                return typeof(void);
            if (member is FieldInfo fieldInfo)
                return fieldInfo.FieldType;
            if (member is PropertyInfo propertyInfo)
                return propertyInfo.PropertyType;
            if (member is EventInfo eventInfo)
                return eventInfo.EventHandlerType ?? typeof(MulticastDelegate);
            if (member is ConstructorInfo constructorInfo)
                return constructorInfo.DeclaringType ?? typeof(void);
            if (member is MethodInfo methodInfo)
                return methodInfo.ReturnType;
            if (member is Type type)
                return type;
            Debugger.Break();
            return typeof(void);
        }
    }
}