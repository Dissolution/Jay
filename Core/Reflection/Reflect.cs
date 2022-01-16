using Jay.Reflection.Building.Adapting;
using Jay.Reflection.Extensions;
using Jay.Validation;

namespace Jay.Reflection;

public static partial class Reflect
{
    public const BindingFlags AllFlags = BindingFlags.Public | BindingFlags.NonPublic |
                                         BindingFlags.Static | BindingFlags.Instance |
                                         BindingFlags.IgnoreCase;

    public const BindingFlags PublicFlags = BindingFlags.Public |
                                         BindingFlags.Static | BindingFlags.Instance |
                                         BindingFlags.IgnoreCase;

    public const BindingFlags NonPublicFlags = BindingFlags.NonPublic |
                                         BindingFlags.Static | BindingFlags.Instance |
                                         BindingFlags.IgnoreCase;

    public const BindingFlags StaticFlags = BindingFlags.Public | BindingFlags.NonPublic |
                                         BindingFlags.Static |
                                         BindingFlags.IgnoreCase;

    public const BindingFlags InstanceFlags = BindingFlags.Public | BindingFlags.NonPublic |
                                         BindingFlags.Instance |
                                         BindingFlags.IgnoreCase;

    private static readonly DelegateMemberCache _cache = new DelegateMemberCache();

    public static TValue GetValue<TInstance, TValue>(this FieldInfo field, ref TInstance instance)
    {
        return _cache.GetOrAdd<FieldInfo, Getter<TInstance, TValue>>(field, 
            dm => dm.Emitter
            .LoadInstanceFor(dm.Parameters[0], field, out int offset)
            .Assert(() => offset == 1)
            .Ldfld(field)
            .Cast(field.FieldType, dm.ReturnType)
            .Ret())(ref instance);
    }

    public static void SetValue<TInstance, TValue>(this FieldInfo field, ref TInstance instance, TValue value)
    {
        _cache.GetOrAdd<FieldInfo, Setter<TInstance, TValue>>(field,
            dm => dm.Emitter
                    .LoadInstanceFor(dm.Parameters[0], field, out int offset)
                    .Assert(() => offset == 1)
                    .LoadAs(dm.Parameters[1], field.FieldType)
                    .Stfld(field)
                    .Ret())(ref instance, value);
    }
}