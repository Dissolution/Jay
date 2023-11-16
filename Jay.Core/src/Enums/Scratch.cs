namespace Jay.Enums.Scratch;

public static class EnumTypeInfo<TEnum>
    where TEnum : struct, Enum
{
    public static Type EnumType {get; }
    public static string Name {get; }
    public static Attribute[] Attributes {get; }
    public static bool IsFlags {get;}

    public static IReadOnlyList<string> MemberNames {get;}
    public static IReadOnlyList<TEnum> Members {get;}

    static EnumTypeInfo()
    {
        EnumType = typeof(TEnum);
        Name = EnumType.Name;
        Attributes = Attribute.GetCustomAttributes(EnumType);
        IsFlags = Attributes.OfType<FlagsAttribute>().Any();

        #if NET6_0_OR_GREATER
        MemberNames = Enum.GetNames<TEnum>();
        Members = Enum.GetValues<TEnum>();
        #else
        MemberNames = Enum.GetNames(EnumType);
        Members = Enum.GetValues(EnumType).Cast<TEnum>().ToList();
        #endif
    }

}

