using System.Reflection;
using Jay.Reflection.Emission;

namespace Jay.Reflection;

public static class FieldInfoExtensions
{
    public static Visibility Access(this FieldInfo? fieldInfo)
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

    public static StaticGetter<TValue> CreateStaticGetter<TValue>(this FieldInfo fieldInfo)
    {
        Validation.IsStatic(fieldInfo);
        var dynMethod = RuntimeBuilder.CreateDynamicMethod<StaticGetter<TValue>>($"get_{fieldInfo.OwnerType()}.{fieldInfo.Name}");
        dynMethod.Emitter.Ldsfld(fieldInfo)
                         .Cast(fieldInfo.FieldType, typeof(TValue))
                         .Ret();
        return dynMethod.CreateDelegate();
    }

    public static StructGetter<TStruct, TValue> CreateStructGetter<TStruct, TValue>(this FieldInfo fieldInfo)
        where TStruct : struct
    {
        Validation.IsValue(fieldInfo?.FieldType);
        var dynMethod = RuntimeBuilder.CreateDynamicMethod<StaticGetter<TValue>>($"get_{fieldInfo.OwnerType()}_{fieldInfo.Name}");
        dynMethod.Emitter
            .Ldarg(0)
            .Ldfld(fieldInfo)
            .Cast(fieldInfo.FieldType, typeof(TValue))
            .Ret();
        return dynMethod.CreateDelegate();
    }

    public static ObjectGetter<TValue> CreateObjectGetter<TValue>(this FieldInfo fieldInfo)
    {
        throw new NotImplementedException();
    }

    public static ClassGetter<TClass, TValue> CreateClassGetter<TClass, TValue>(this FieldInfo fieldInfo)
        where TClass : class
    {
        throw new NotImplementedException();
    }

    public static TValue? GetValue<TStruct, TValue>(this FieldInfo fieldInfo,
                                                      ref TStruct instance)
        where TStruct : struct
    {
        var getter = CreateStructGetter<TStruct, TValue>(fieldInfo);
        return getter(ref instance);
    }

    public static TValue? GetValue<TValue>(this FieldInfo fieldInfo,
                                                    object? instance)
    {
        var getter = CreateObjectGetter<TValue>(fieldInfo);
        return getter(instance);
    }

    public static TValue? GetValue<TClass, TValue>(this FieldInfo fieldInfo,
                                                   TClass? instance)
        where TClass : class
    {
        var getter = CreateClassGetter<TClass, TValue>(fieldInfo);
        return getter(instance);
    }

    public static TValue? GetValue<TValue>(this FieldInfo fieldInfo)
    {
        var getter = CreateStaticGetter<TValue>(fieldInfo);
        return getter();
    }



    public static StaticSetter<TValue> CreateStaticSetter<TValue>(this FieldInfo fieldInfo)
    {
        throw new NotImplementedException();
    }

    public static StructSetter<TStruct, TValue> CreateStructSetter<TStruct, TValue>(this FieldInfo fieldInfo)
        where TStruct : struct
    {
        throw new NotImplementedException();
    }

    public static ObjectSetter<TValue> CreateObjectSetter<TValue>(this FieldInfo fieldInfo)
    {
        throw new NotImplementedException();
    }

    public static ClassSetter<TClass, TValue> CreateClassSetter<TClass, TValue>(this FieldInfo fieldInfo)
        where TClass : class
    {
        throw new NotImplementedException();
    }

    public static void SetValue<TStruct, TValue>(this FieldInfo fieldInfo,
                                                 ref TStruct instance,
                                                 TValue? value)
        where TStruct : struct
    {
        var setter = CreateStructSetter<TStruct, TValue>(fieldInfo);
        setter(ref instance, value);
    }

    public static void SetValue<TValue>(this FieldInfo fieldInfo,
                                           object? instance,
                                           TValue? value)
    {
        var setter = CreateObjectSetter<TValue>(fieldInfo);
        setter(instance, value);
    }

    public static void SetValue<TClass, TValue>(this FieldInfo fieldInfo,
                                                   TClass? instance,
                                                   TValue? value)
        where TClass : class
    {
        var setter = CreateClassSetter<TClass, TValue>(fieldInfo);
        setter(instance, value);
    }

    public static void SetValue<TValue>(this FieldInfo fieldInfo,
                                        TValue? value)
    {
        var setter = CreateStaticSetter<TValue>(fieldInfo);
        setter(value);
    }
}