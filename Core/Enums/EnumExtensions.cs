using System.Diagnostics;
using System.Numerics;
using InlineIL;
using Jay.Reflection;
using static InlineIL.IL;

namespace Jay.Enums;

public static class EnumExtensions
{
    public static EnumMemberInfo<TEnum> GetInfo<TEnum>(this TEnum @enum)
        where TEnum : struct, Enum
    {
        return EnumInfo.For<TEnum>(@enum);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool IsDefault<TEnum>(this TEnum @enum)
        where TEnum : struct, Enum
    {
        Emit.Ldarg(nameof(@enum));
        Emit.Ldc_I4_0();
        Emit.Ceq();
        return Return<bool>();
    }


   


    public static TEnum[] GetFlags<TEnum>(this TEnum @enum)
        where TEnum : struct, Enum
    {
        throw new NotImplementedException();
        /*

        // Values are sorted, so if the incoming value is 0, we can check to see whether
        // the first entry matches it, in which case we can return its name; otherwise,
        // we can just return "0".
        if (@enum.IsDefault())
        {
                return values.Length > 0 && values[0] == 0 ?
                    names[0] :
                    "0";
            }

            // With a ulong result value, regardless of the enum's base type, the maximum
            // possible number of consistent name/values we could have is 64, since every
            // value is made up of one or more bits, and when we see values and incorporate
            // their names, we effectively switch off those bits.
            Span<int> foundItems = stackalloc int[64];

            // Walk from largest to smallest. It's common to have a flags enum with a single
            // value that matches a single entry, in which case we can just return the existing
            // name string.
            int index = values.Length - 1;
            while (index >= 0)
            {
                if (values[index] == resultValue)
                {
                    return names[index];
                }

                if (values[index] < resultValue)
                {
                    break;
                }

                index--;
            }

            // Now look for multiple matches, storing the indices of the values
            // into our span.
            int resultLength = 0, foundItemsCount = 0;
            while (index >= 0)
            {
                ulong currentValue = values[index];
                if (index == 0 && currentValue == 0)
                {
                    break;
                }

                if ((resultValue & currentValue) == currentValue)
                {
                    resultValue -= currentValue;
                    foundItems[foundItemsCount++] = index;
                    resultLength = checked(resultLength + names[index].Length);
                }

                index--;
            }

            // If we exhausted looking through all the values and we still have
            // a non-zero result, we couldn't match the result to only named values.
            // In that case, we return null and let the call site just generate
            // a string for the integral value.
            if (resultValue != 0)
            {
                return null;
            }

            // We know what strings to concatenate.  Do so.

            Debug.Assert(foundItemsCount > 0);
            const int SeparatorStringLength = 2; // ", "
            string result = string.FastAllocateString(checked(resultLength + (SeparatorStringLength * (foundItemsCount - 1))));

            Span<char> resultSpan = new Span<char>(ref result.GetRawStringData(), result.Length);
            string name = names[foundItems[--foundItemsCount]];
            name.CopyTo(resultSpan);
            resultSpan = resultSpan.Slice(name.Length);
            while (--foundItemsCount >= 0)
            {
                resultSpan[0] = EnumSeparatorChar;
                resultSpan[1] = ' ';
                resultSpan = resultSpan.Slice(2);

                name = names[foundItems[foundItemsCount]];
                name.CopyTo(resultSpan);
                resultSpan = resultSpan.Slice(name.Length);
            }
            Debug.Assert(resultSpan.IsEmpty);

            return result;
        }
        */
    }


    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool HasFlag<TEnum>(this TEnum @enum, TEnum flag)
        where TEnum : unmanaged, Enum
    {
        // (e & flag) == flag
        Emit.Ldarg(nameof(flag));
        Emit.Dup();
        Emit.Ldarg(nameof(@enum));
        Emit.And();
        Emit.Ceq();
        return Return<bool>();
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static int FlagCount<TEnum>(this TEnum @enum)
        where TEnum : unmanaged, Enum
    {
        Emit.Ldarg(nameof(@enum));
        Emit.Conv_U8();
        Emit.Call(MethodRef.Method(typeof(BitOperations), nameof(BitOperations.PopCount), typeof(ulong)));
        return Return<int>();
    }
        
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool HasAnyFlags<TEnum>(this TEnum @enum, TEnum firstFlag)
        where TEnum : unmanaged, Enum
    {
        return HasFlag(@enum, firstFlag);
    }
        
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool HasAnyFlags<TEnum>(this TEnum @enum, TEnum firstFlag, TEnum secondFlag)
        where TEnum : unmanaged, Enum
    {
        return HasFlag(@enum, firstFlag) || HasFlag(@enum, secondFlag);
    }
        
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool HasAnyFlags<TEnum>(this TEnum @enum, TEnum firstFlag, TEnum secondFlag, TEnum thirdFlag)
        where TEnum : unmanaged, Enum
    {
        return HasFlag(@enum, firstFlag) || HasFlag(@enum, secondFlag) || HasFlag(@enum, thirdFlag);
    }
        
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool HasAnyFlags<TEnum>(this TEnum @enum, params TEnum[] flags)
        where TEnum : unmanaged, Enum
    {
        return flags.Any(flag => HasFlag(@enum, flag));
    }
        
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool HasAllFlags<TEnum>(this TEnum @enum, TEnum firstFlag)
        where TEnum : unmanaged, Enum
    {
        return HasFlag(@enum, firstFlag);
    }
        
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool HasAllFlags<TEnum>(this TEnum @enum, TEnum firstFlag, TEnum secondFlag)
        where TEnum : unmanaged, Enum
    {
        return HasFlag(@enum, firstFlag) && HasFlag(@enum, secondFlag);
    }
        
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool HasAllFlags<TEnum>(this TEnum @enum, TEnum firstFlag, TEnum secondFlag, TEnum thirdFlag)
        where TEnum : unmanaged, Enum
    {
        return HasFlag(@enum, firstFlag) && HasFlag(@enum, secondFlag) && HasFlag(@enum, thirdFlag);
    }
        
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool HasAllFlags<TEnum>(this TEnum @enum, params TEnum[] flags)
        where TEnum : unmanaged, Enum
    {
        return flags.All(flag => HasFlag(@enum, flag));
    }
      
        
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void AddFlag<TEnum>(this ref TEnum @enum, TEnum flag)
        where TEnum : unmanaged, Enum
    {
        Emit.Ldarg(nameof(@enum));
        Emit.Ldarg(nameof(@enum));
        Emit.Ldobj<TEnum>();
        Emit.Ldarg(nameof(flag));
        Emit.Or();
        Emit.Stobj<TEnum>();
    }
        
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void AddFlags<TEnum>(this ref TEnum @enum, TEnum firstFlag)
        where TEnum : unmanaged, Enum
    {
        Emit.Ldarg(nameof(@enum));
        Emit.Ldarg(nameof(@enum));
        Emit.Ldobj<TEnum>();
        Emit.Ldarg(nameof(firstFlag));
        Emit.Or();
        Emit.Stobj<TEnum>();
    }
        
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void AddFlags<TEnum>(this ref TEnum @enum, TEnum firstFlag, TEnum secondFlag)
        where TEnum : unmanaged, Enum
    {
        Emit.Ldarg(nameof(@enum));
        Emit.Ldarg(nameof(@enum));
        Emit.Ldobj<TEnum>();
        Emit.Ldarg(nameof(firstFlag));
        Emit.Or();
        Emit.Ldarg(nameof(secondFlag));
        Emit.Or();
        Emit.Stobj<TEnum>();
    }
        
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void AddFlags<TEnum>(this ref TEnum @enum, TEnum firstFlag, TEnum secondFlag, TEnum thirdFlag)
        where TEnum : unmanaged, Enum
    {
        Emit.Ldarg(nameof(@enum));
        Emit.Ldarg(nameof(@enum));
        Emit.Ldobj<TEnum>();
        Emit.Ldarg(nameof(firstFlag));
        Emit.Or();
        Emit.Ldarg(nameof(secondFlag));
        Emit.Or();
        Emit.Ldarg(nameof(thirdFlag));
        Emit.Or();
        Emit.Stobj<TEnum>();
    }
        
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void AddFlags<TEnum>(this ref TEnum @enum, params TEnum[] flags)
        where TEnum : unmanaged, Enum
    {
        foreach (var flag in flags)
        {
            AddFlag(ref @enum, flag);
        }
    }
        
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void RemoveFlag<TEnum>(this ref TEnum @enum, TEnum flag)
        where TEnum : unmanaged, Enum
    {
        // e &= ~flag
        Emit.Ldarg(nameof(@enum));
        Emit.Ldarg(nameof(@enum));
        Emit.Ldobj<TEnum>();
        Emit.Ldarg(nameof(flag));
        Emit.Not();
        Emit.And();
        Emit.Stobj<TEnum>();
    }
        
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void RemoveFlags<TEnum>(this ref TEnum @enum, TEnum firstFlag)
        where TEnum : unmanaged, Enum
    {
        Emit.Ldarg(nameof(@enum));
        Emit.Ldarg(nameof(@enum));
        Emit.Ldobj<TEnum>();
        Emit.Ldarg(nameof(firstFlag));
        Emit.Not();
        Emit.And();
        Emit.Stobj<TEnum>();
    }
        
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void RemoveFlags<TEnum>(this ref TEnum @enum, TEnum firstFlag, TEnum secondFlag)
        where TEnum : unmanaged, Enum
    {
        Emit.Ldarg(nameof(@enum));
        Emit.Ldarg(nameof(@enum));
        Emit.Ldobj<TEnum>();
        Emit.Ldarg(nameof(firstFlag));
        Emit.Not();
        Emit.And();
        Emit.Ldarg(nameof(secondFlag));
        Emit.Not();
        Emit.And();
        Emit.Stobj<TEnum>();
    }
        
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void RemoveFlags<TEnum>(this ref TEnum @enum, TEnum firstFlag, TEnum secondFlag, TEnum thirdFlag)
        where TEnum : unmanaged, Enum
    {
        Emit.Ldarg(nameof(@enum));
        Emit.Ldarg(nameof(@enum));
        Emit.Ldobj<TEnum>();
        Emit.Ldarg(nameof(firstFlag));
        Emit.Not();
        Emit.And();
        Emit.Ldarg(nameof(secondFlag));
        Emit.Not();
        Emit.And();
        Emit.Ldarg(nameof(thirdFlag));
        Emit.Not();
        Emit.And();
        Emit.Stobj<TEnum>();
    }
        
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void RemoveFlags<TEnum>(this ref TEnum @enum, params TEnum[] flags)
        where TEnum : unmanaged, Enum
    {
        foreach (var flag in flags)
        {
            RemoveFlag(ref @enum, flag);
        }
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static int CompareTo<TEnum>(this TEnum @enum, TEnum other)
        where TEnum : unmanaged, Enum
    {
        Emit.Ldarg(nameof(@enum));
        Emit.Ldarg(nameof(other));
        Emit.Ceq();
        Emit.Brfalse("notEqual");
        Emit.Ldc_I4(0);
        Emit.Ret();
        MarkLabel("notEqual");
        Emit.Ldarg(nameof(@enum));
        Emit.Ldarg(nameof(other));
        Emit.Clt();
        Emit.Brfalse("greaterThan");
        Emit.Ldc_I4(-1);
        Emit.Ret();
        MarkLabel("greaterThan");
        Emit.Ldc_I4(1);
        Emit.Ret();
        throw Unreachable();
    }
}