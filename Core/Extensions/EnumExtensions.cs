using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

// ReSharper disable EntityNameCapturedOnly.Global

namespace Jay
{
    public static class EnumExtensions
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int FlagCount<TEnum>(this TEnum @enum)
            where TEnum : unmanaged, Enum
            => Enums<TEnum>.FlagCount(@enum);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void AddFlag<TEnum>(this ref TEnum @enum, TEnum flag)
            where TEnum : unmanaged, Enum
            => @enum = Enums<TEnum>.Combine(@enum, flag);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void RemoveFlag<TEnum>(this ref TEnum @enum, TEnum flag)
            where TEnum : unmanaged, Enum
            => @enum = Enums<TEnum>.And(@enum, Enums<TEnum>.Not(flag));

        public static TEnum WithFlag<TEnum>(this TEnum @enum, TEnum flag)
            where TEnum : unmanaged, Enum
            => Enums<TEnum>.Combine(@enum, flag);
        
        public static TEnum WithoutFlag<TEnum>(this TEnum @enum, TEnum flag)
            where TEnum : unmanaged, Enum
            => Enums<TEnum>.And(@enum, Enums<TEnum>.Not(flag));
    }
}