using Jay.Reflection.Builders;
using Jay.Reflection.Caching;
using Jay.Reflection.Emitting;
using Jay.Reflection.Emitting.Args;

namespace Jay.Reflection.Adapters;

/// <summary>
/// An adapter for interactions with <see cref="FieldInfo"/>s
/// </summary>
public static class FieldAdapter
{
    private static GetValue<TInstance, TValue> CreateGetValueDelegate<TInstance, TValue>(FieldInfo field)
    {
        return RuntimeBuilder.BuildDelegate<GetValue<TInstance, TValue>>(
            $"get_{field.DeclaringType}_{field.Name}",
            builder => builder.Emitter
                .LoadInstanceFor(builder.Parameters[0], field)
                .Ldfld(field)
                .EmitCast(field.FieldType, builder.ReturnType)
                .Ret());
    }

    public static GetValue<TInstance, TValue> GetGetValueDelegate<TInstance, TValue>(FieldInfo fieldInfo)
    {
        return MemberDelegateCache.GetOrAdd(fieldInfo, CreateGetValueDelegate<TInstance, TValue>);
    }

    public static TValue GetValue<TInstance, TValue>(this FieldInfo fieldInfo,
        ref TInstance instance)
    {
        return GetGetValueDelegate<TInstance, TValue>(fieldInfo)(ref instance);
    }
    
    
    private static SetValue<TInstance, TValue> CreateSetValueDelegate<TInstance, TValue>(FieldInfo field)
    {
        return RuntimeBuilder.BuildDelegate<SetValue<TInstance, TValue>>(
            $"set_{field.OwnerType()}_{field.Name}",
            builder => builder.Emitter
                .LoadInstanceFor(builder.Parameters[0], field)
                .EmitCast(builder.Parameters[1], field.FieldType)
                .Stfld(field)
                .Ret());
    }

    public static SetValue<TInstance, TValue> GetSetValueDelegate<TInstance, TValue>(FieldInfo field)
    {
        return MemberDelegateCache.GetOrAdd(field, CreateSetValueDelegate<TInstance, TValue>);
    }

    public static void SetValue<TInstance, TValue>(this FieldInfo fieldInfo,
        ref TInstance instance, TValue value)
    {
        GetSetValueDelegate<TInstance, TValue>(fieldInfo)(ref instance, value);
    }
}