using System.ComponentModel.DataAnnotations;
using System.Reflection;
using Jay.Reflection.Adapting;
using Jay.Reflection.Emission;

namespace Jay.Reflection;

public static class PropertyInfoExtensions
{
    public static MethodInfo? GetGetter(this PropertyInfo? propertyInfo)
    {
        return propertyInfo?.GetGetMethod(false) ??
               propertyInfo?.GetGetMethod(true);
    }
        
    public static MethodInfo? GetSetter(this PropertyInfo? propertyInfo)
    {
        return propertyInfo?.GetSetMethod(false) ??
               propertyInfo?.GetSetMethod(true);
    }
        
    public static Visibility Visibility(this PropertyInfo? propertyInfo)
    {
        Visibility visibility = Reflection.Visibility.None;
        if (propertyInfo is null)
            return visibility;
        visibility |= propertyInfo.GetGetter().Visibility();
        visibility |= propertyInfo.GetSetter().Visibility();
        return visibility;
    }

    public static bool IsStatic(this PropertyInfo? propertyInfo)
    {
        if (propertyInfo is null)
            return false;
        return propertyInfo.GetGetter().IsStatic() ||
               propertyInfo.GetSetter().IsStatic();
    }

    public static StaticGetter<TValue> CreateStaticGetter<TValue>(this PropertyInfo property)
    {
        ArgumentNullException.ThrowIfNull(property);
        var getter = GetGetter(property);
        Validation.IsStatic(getter);
        return RuntimeBuilder.CreateDelegate<StaticGetter<TValue>>(
            $"get_{property.OwnerType()}.{property.Name}", method =>
            {
                method.Emitter
                      .Call(getter)
                      .Cast(getter.ReturnType, typeof(TValue))
                      .Ret();
            });
    }

    public static StructGetter<TStruct, TValue> CreateStructGetter<TStruct, TValue>(this PropertyInfo property)
        where TStruct : struct
    {
        ArgumentNullException.ThrowIfNull(property);
        var result = property.TryGetInstanceType(out var instanceType);
        result.ThrowIfFailed();
        Validation.IsValue(instanceType);
        var getter = GetGetter(property);
        Validation.IsInstance(getter);
        return RuntimeBuilder.CreateDelegate<StructGetter<TStruct, TValue>>(
            $"get_{instanceType}_{property.Name}", method =>
            {
                method.Emitter
                      .LoadAs(method.Parameters[0], instanceType)
                      .Call(getter)
                      .Cast(getter.ReturnType, typeof(TValue))
                      .Ret();
            });
    }

    public static ClassGetter<TClass, TValue> CreateClassGetter<TClass, TValue>(this FieldInfo fieldInfo)
        where TClass : class
    {
        var result = fieldInfo.TryGetInstanceType(out var instanceType);
        result.ThrowIfFailed();
        Validation.IsClass(instanceType, nameof(fieldInfo));
        return RuntimeBuilder.CreateDelegate<ClassGetter<TClass, TValue>>(
            $"get_{instanceType}_{fieldInfo.Name}", method =>
            {
                method.Emitter
                      .LoadAs(method.Parameters[0], instanceType)
                      .Ldfld(fieldInfo)
                      .Cast(fieldInfo.FieldType, typeof(TValue))
                      .Ret();
            });
    }

    public static TValue? GetValue<TStruct, TValue>(this FieldInfo fieldInfo, ref TStruct instance)
        where TStruct : struct
    {
        var getter = DelegateMemberCache.Instance
                                        .GetOrAdd(fieldInfo, CreateStructGetter<TStruct, TValue>);
        return getter(ref instance);
    }

    public static TValue? GetValue<TClass, TValue>(this FieldInfo fieldInfo,
                                                   TClass? instance)
        where TClass : class
    {
        var getter = DelegateMemberCache.Instance
            .GetOrAdd(fieldInfo, CreateClassGetter<TClass, TValue>);
        return getter(instance);
    }

    public static TValue? GetStaticValue<TValue>(this FieldInfo fieldInfo)
    {
        var getter = DelegateMemberCache.Instance
            .GetOrAdd(fieldInfo, CreateStaticGetter<TValue>);
        return getter();
    }



    public static StaticSetter<TValue> CreateStaticSetter<TValue>(this FieldInfo fieldInfo)
    {
        Validation.IsStatic(fieldInfo);
        return RuntimeBuilder.CreateDelegate<StaticSetter<TValue>>(
            $"set_{fieldInfo.OwnerType()}.{fieldInfo.Name}", method =>
            {
                method.Emitter
                      .LoadAs(method.Parameters[0], fieldInfo.FieldType)
                      .Stsfld(fieldInfo)
                      .Ret();
            });
    }

    public static StructSetter<TStruct, TValue> CreateStructSetter<TStruct, TValue>(this FieldInfo fieldInfo)
        where TStruct : struct
    {
        var result = fieldInfo.TryGetInstanceType(out var instanceType);
        result.ThrowIfFailed();
        Validation.IsValue(instanceType, nameof(fieldInfo));
        return RuntimeBuilder.CreateDelegate<StructSetter<TStruct, TValue>>(
            $"get_{instanceType}_{fieldInfo.Name}", method =>
            {
                method.Emitter
                      .LoadAs(method.Parameters[0], instanceType)
                      .LoadAs(method.Parameters[1], fieldInfo.FieldType)
                      .Stfld(fieldInfo)
                      .Ret();
            });
    }

    public static ClassSetter<TClass, TValue> CreateClassSetter<TClass, TValue>(this FieldInfo fieldInfo)
        where TClass : class
    {
        var result = fieldInfo.TryGetInstanceType(out var instanceType);
        result.ThrowIfFailed();
        Validation.IsClass(instanceType, nameof(fieldInfo));
        return RuntimeBuilder.CreateDelegate<ClassSetter<TClass, TValue>>(
            $"get_{instanceType}_{fieldInfo.Name}", method =>
            {
                method.Emitter
                      .LoadAs(method.Parameters[0], instanceType)
                      .LoadAs(method.Parameters[1], fieldInfo.FieldType)
                      .Stfld(fieldInfo)
                      .Ret();
            });
    }

    public static void SetValue<TStruct, TValue>(this FieldInfo fieldInfo,
                                                 ref TStruct instance,
                                                 TValue? value)
        where TStruct : struct
    {
        var setter = DelegateMemberCache.Instance
                                        .GetOrAdd(fieldInfo, CreateStructSetter<TStruct, TValue>);
        setter(ref instance, value);
    }


    public static void SetValue<TClass, TValue>(this FieldInfo fieldInfo,
                                                   TClass? instance,
                                                   TValue? value)
        where TClass : class
    {
        var setter = DelegateMemberCache.Instance
                                        .GetOrAdd(fieldInfo, CreateClassSetter<TClass, TValue>);
        setter(instance, value);
    }

    public static void SetStaticValue<TValue>(this FieldInfo fieldInfo,
                                        TValue? value)
    {
        var setter = DelegateMemberCache.Instance
                                        .GetOrAdd(fieldInfo, CreateStaticSetter<TValue>);
        setter(value);
    }
}