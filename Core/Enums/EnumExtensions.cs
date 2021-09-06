using System;

using static Jay.NotSafe.Unmanaged;

namespace Jay
{
    public static class EnumExtensions
    { 
        public static EnumMemberInfo<TEnum> GetInfo<TEnum>(this TEnum @enum)
            where TEnum : unmanaged, Enum
        {
            return EnumInfo<TEnum>.GetInfo(@enum);
        }
                
        public static TEnum WithoutFlag<TEnum>(this TEnum @enum, TEnum flag)
            where TEnum : unmanaged, Enum
        {
            return And(@enum, Not(flag));
        }
    }
}