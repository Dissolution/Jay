using System.Reflection;

namespace Jay.Reflection;

public static class FieldInfoExtensions
{
    public static Access Access(this FieldInfo? fieldInfo)
    {
        Access access = Reflection.Access.None;
        if (fieldInfo is null)
            return access;
        if (fieldInfo.IsPrivate)
            access |= Reflection.Access.Private;
        if (fieldInfo.IsFamily)
            access |= Reflection.Access.Protected;
        if (fieldInfo.IsAssembly)
            access |= Reflection.Access.Internal;
        if (fieldInfo.IsPublic)
            access |= Reflection.Access.Public;
        return access;
    }

    public static StaticGetter<TValue> CreateStaticGetter<TValue>(this FieldInfo fieldInfo)
    {
        throw new NotImplementedException();
    }

    public static StructGetter<TStruct, TValue> CreateStructGetter<TStruct, TValue>(this FieldInfo fieldInfo)
        where TStruct : struct
    {
        throw new NotImplementedException();
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