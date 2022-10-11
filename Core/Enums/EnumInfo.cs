using System.Diagnostics;
using InlineIL;
using Jay.Collections;

namespace Jay.Enums;

public static class EnumInfo
{
    private static readonly ConcurrentTypeDictionary<EnumTypeInfo> _enumTypeInfoCache;

    static EnumInfo()
    {
        _enumTypeInfoCache = new();
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static TEnum FromUInt64<TEnum>(ulong value)
        where TEnum : struct, Enum
    {
        IL.Emit.Ldarg(nameof(value));
        return IL.Return<TEnum>();
    }

    private static EnumTypeInfo CreateEnumTypeInfo(Type enumType)
    {
        Debug.Assert(enumType is not null);
        Debug.Assert(enumType.IsEnum);
        Debug.Assert(enumType.IsValueType);
        return (Activator.CreateInstance(typeof(EnumTypeInfo<>).MakeGenericType(enumType)) as EnumTypeInfo)!;
    }

    public static EnumTypeInfo For(Type enumType)
    {
        if (!enumType.IsEnum)
            throw new ArgumentException("You must pass a valid enum type", nameof(enumType));
        return _enumTypeInfoCache.GetOrAdd(enumType, CreateEnumTypeInfo);
    }

    public static EnumTypeInfo<TEnum> For<TEnum>()
        where TEnum : struct, Enum
    {
        return (_enumTypeInfoCache.GetOrAdd<TEnum>(CreateEnumTypeInfo) as EnumTypeInfo<TEnum>)!;
    }

}