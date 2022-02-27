using System.Reflection;
using Jay.Reflection.Building;
using Jay.Reflection.Building.Adapting;

namespace Jay.Reflection;

public static class FieldInfoExtensions
{
    public static Visibility Visibility(this FieldInfo? fieldInfo)
    {
        Visibility visibility = Reflection.Visibility.None;
        if (fieldInfo is null)
            return visibility;
        if (fieldInfo.IsPrivate)
            visibility |= Reflection.Visibility.Private;
        if (fieldInfo.IsFamily)
            visibility |= Reflection.Visibility.Protected;
        if (fieldInfo.IsAssembly)
            visibility |= Reflection.Visibility.Internal;
        if (fieldInfo.IsPublic)
            visibility |= Reflection.Visibility.Public;
        return visibility;
    }

    public static Getter<TInstance, TValue> CreateGetter<TInstance, TValue>(this FieldInfo fieldInfo)
    {
        var result = fieldInfo.TryGetInstanceType(out var instanceType);
        result.ThrowIfFailed();
        Validation.IsValue(instanceType, nameof(fieldInfo));
        return RuntimeBuilder.CreateDelegate<Getter<TInstance, TValue>>(
                                                                            $"get_{instanceType}_{fieldInfo.Name}", method =>
                                                                            {
                                                                                method.Emitter
                                                                                      .LoadAs(method.Parameters[0], instanceType)
                                                                                      .Ldfld(fieldInfo)
                                                                                      .Cast(fieldInfo.FieldType, typeof(TValue))
                                                                                      .Ret();
                                                                            });
    }
    
    public static StaticGetter<TValue> CreateStaticGetter<TValue>(this FieldInfo fieldInfo)
    {
        Validation.IsStatic(fieldInfo);
        return RuntimeBuilder.CreateDelegate<StaticGetter<TValue>>(
            $"get_{fieldInfo.OwnerType()}.{fieldInfo.Name}", method =>
            {
                method.Emitter
                      .Ldsfld(fieldInfo)
                      .Cast(fieldInfo.FieldType, typeof(TValue))
                      .Ret();
            });
    }

    public static StructGetter<TStruct, TValue> CreateStructGetter<TStruct, TValue>(this FieldInfo fieldInfo)
        where TStruct : struct
    {
        var result = fieldInfo.TryGetInstanceType(out var instanceType);
        result.ThrowIfFailed();
        Validation.IsValue(instanceType, nameof(fieldInfo));
        return RuntimeBuilder.CreateDelegate<StructGetter<TStruct, TValue>>(
            $"get_{instanceType}_{fieldInfo.Name}", method =>
            {
                method.Emitter
                      .LoadAs(method.Parameters[0], instanceType)
                      .Ldfld(fieldInfo)
                      .Cast(fieldInfo.FieldType, typeof(TValue))
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


    public static Setter<TInstance, TValue> CreateSetter<TInstance, TValue>(this FieldInfo fieldInfo)
    {
        var result = fieldInfo.TryGetInstanceType(out var instanceType);
        result.ThrowIfFailed();
        Validation.IsValue(instanceType, nameof(fieldInfo));
        return RuntimeBuilder.CreateDelegate<Setter<TInstance, TValue>>(
                                                                            $"get_{instanceType}_{fieldInfo.Name}", method =>
                                                                            {
                                                                                method.Emitter
                                                                                      .LoadAs(method.Parameters[0], instanceType)
                                                                                      .LoadAs(method.Parameters[1], fieldInfo.FieldType)
                                                                                      .Stfld(fieldInfo)
                                                                                      .Ret();
                                                                            });
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