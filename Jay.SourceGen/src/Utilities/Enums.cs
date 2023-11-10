namespace Jay.SourceGen.Utilities;

public static class Enums
{
    public static IReadOnlyList<TEnum> GetFlags<TEnum>(this TEnum @enum)
        where TEnum : struct, Enum
    {
        var flags = new List<TEnum>(64);
        int maxBits = Unsafe.SizeOf<TEnum>() * 8;
        ulong enumValue = Convert.ToUInt64(@enum);
        //string enumValueBits = Convert.ToString((long)enumValue, 2);
        for (var shift = 0; shift < maxBits; shift++)
        {
            ulong mask = 1UL << shift;
            //string maskBits = Convert.ToString((long)mask, 2);
            if ((enumValue & mask) != 0UL)
            {
                var flag = (TEnum)Enum.ToObject(typeof(TEnum), mask);
                flags.Add(flag);
            }
        }
        return flags;
    }
}