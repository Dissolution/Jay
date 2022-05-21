namespace Jay.Dumping.Refactor;

public static class EnumExtensions
{


    public static EnumInfo<TEnum> GetInfo<TEnum>(this TEnum @enum)
        where TEnum : struct, Enum
    {
        return EnumTypeInfo<TEnum>.GetInfo(@enum);
    }
}