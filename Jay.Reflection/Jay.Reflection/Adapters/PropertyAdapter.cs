using Jay.Reflection.Caching;
using Jay.Reflection.Exceptions;

namespace Jay.Reflection.Adapters;

public static class PropertyAdapter
{
    private static SetValue<TInstance, TValue> CreateSetValueDelegate<TInstance, TValue>(PropertyInfo property)
    {
        // Find setter
        var setter = property.GetSetter();
        if (setter is null)
        {
            var backingField = property.GetBackingField();
            if (backingField is not null)
                return FieldAdapter.GetSetValueDelegate<TInstance, TValue>(backingField);
            throw new ReflectedException($"Cannot find a way to set {property}");
        }

        return RuntimeMethodAdapter.Adapt<SetValue<TInstance, TValue>>(setter);
    }

    public static SetValue<TInstance, TValue> GetSetValueDelegate<TInstance, TValue>(PropertyInfo propertyInfo)
    {
        return MemberDelegateCache.GetOrAdd(propertyInfo, CreateSetValueDelegate<TInstance, TValue>);
    }

    public static void SetValue<TInstance, TValue>(this PropertyInfo propertyInfo,
        ref TInstance instance, TValue value)
    {
        GetSetValueDelegate<TInstance, TValue>(propertyInfo)(ref instance, value);
    }
    

    private static GetValue<TInstance, TValue> CreateGetValueDelegate<TInstance, TValue>(PropertyInfo property)
    {
        // Find getter
        var getter = property.GetGetter();
        if (getter is null)
        {
            var backingField = property.GetBackingField();
            if (backingField is not null)
                return FieldAdapter.GetGetValueDelegate<TInstance, TValue>(backingField);
            throw new ReflectedException($"Cannot find a way to get {property}");
        }

        return RuntimeMethodAdapter.Adapt<GetValue<TInstance, TValue>>(getter);
    }

    public static GetValue<TInstance, TValue> GetGetValueDelegate<TInstance, TValue>(PropertyInfo propertyInfo)
    {
        return MemberDelegateCache.GetOrAdd(propertyInfo, CreateGetValueDelegate<TInstance, TValue>);
    }

    public static TValue GetValue<TInstance, TValue>(this PropertyInfo propertyInfo,
        ref TInstance instance)
    {
        return GetGetValueDelegate<TInstance, TValue>(propertyInfo)(ref instance);
    }
}