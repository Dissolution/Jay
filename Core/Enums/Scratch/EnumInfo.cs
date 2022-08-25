using System.Diagnostics;
using System.Reflection;
using Jay.Reflection;
using Jay.Reflection.Building;
using Jay.Reflection.Exceptions;
using static InlineIL.IL;
using TypeCache = Jay.Reflection.Internal.TypeCache;

namespace Jay.Enums.Scratch;


public abstract class EnumTypeInfo
{
    public Type Type { get; }
    public Attribute[] Attributes { get; }
    public bool HasFlags { get; }
    public int Size { get; }

    protected EnumTypeInfo(Type enumType)
    {
        Debug.Assert(enumType != null);
        Debug.Assert(enumType.IsEnum);
        Debug.Assert(enumType.IsValueType);
        Debug.Assert(TypeCache.IsUnmanaged(enumType));
       
        this.Type = enumType;
        this.Attributes = Attribute.GetCustomAttributes(enumType);
        this.HasFlags = Attributes.OfType<FlagsAttribute>().Any();
        this.Size = TypeCache.SizeOf(enumType)!.Value;
    }
}

public static class EnumExtensions
{
    public static TEnum WithFlag<TEnum>(this TEnum @enum, TEnum flag)
        where TEnum : struct, Enum
    {
        return EnumTypeInfo<TEnum>.Combine(@enum, flag);
    }

    public static TEnum WithFlags<TEnum>(this TEnum @enum, TEnum flag)
        where TEnum : struct, Enum
    {
        return EnumTypeInfo<TEnum>.Combine(@enum, flag);
    }

    public static TEnum WithFlags<TEnum>(this TEnum @enum, TEnum firstFlag, TEnum secondFlag)
        where TEnum : struct, Enum
    {
        return EnumTypeInfo<TEnum>.Combine(@enum, firstFlag, secondFlag);
    }

    public static TEnum WithFlags<TEnum>(this TEnum @enum, TEnum firstFlag, TEnum secondFlag, TEnum thirdFlag)
        where TEnum : struct, Enum
    {
        return EnumTypeInfo<TEnum>.Combine(@enum, firstFlag, secondFlag, thirdFlag);
    }

    public static TEnum WithFlags<TEnum>(this TEnum @enum, params TEnum[] flags)
        where TEnum : struct, Enum
    {
        TEnum e = @enum;
        foreach (var flag in flags)
        {
            e = e.Or(flag);
        }
        return e;
    }

    public static TEnum Or<TEnum>(this TEnum @enum, TEnum flag)
        where TEnum : struct, Enum
    {
        return EnumIL<TEnum>.Or(@enum, flag);
    }
}

public static class EnumIL<TEnum>
    where TEnum : struct, Enum
{
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static ulong ToUInt64(TEnum @enum)
    {
        Emit.Ldarg(nameof(@enum));
        Emit.Conv_U8();
        return Return<ulong>();
    }


    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static TEnum Not(TEnum @enum)
    {
        Emit.Ldarg(nameof(@enum));
        Emit.Not();
        return Return<TEnum>();
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static TEnum Neg(TEnum @enum)
    {
        Emit.Ldarg(nameof(@enum));
        Emit.Neg();
        return Return<TEnum>();
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static TEnum Or(TEnum left, TEnum right)
    {
        Emit.Ldarg(nameof(left));
        Emit.Ldarg(nameof(right));
        Emit.Or();
        return Return<TEnum>();
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static TEnum And(TEnum left, TEnum right)
    {
        Emit.Ldarg(nameof(left));
        Emit.Ldarg(nameof(right));
        Emit.And();
        return Return<TEnum>();
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static TEnum Xor(TEnum left, TEnum right)
    {
        Emit.Ldarg(nameof(left));
        Emit.Ldarg(nameof(right));
        Emit.Xor();
        return Return<TEnum>();
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool Equals(TEnum left, TEnum right)
    {
        Emit.Ldarg(nameof(left));
        Emit.Ldarg(nameof(right));
        Emit.Ceq();
        return Return<bool>();
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static TEnum ShiftLeft(TEnum @enum, int count)
    {
        Emit.Ldarg(nameof(@enum));
        Emit.Ldarg(nameof(count));
        Emit.Shl();
        return Return<TEnum>();
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static TEnum ShiftRight(TEnum @enum, int count)
    {
        Emit.Ldarg(nameof(@enum));
        Emit.Ldarg(nameof(count));
        Emit.Shr_Un();
        return Return<TEnum>();
    }
}

public class EnumTypeInfo<TEnum> : EnumTypeInfo, 
                                   IEqualityComparer<TEnum>,
                                   IComparer<TEnum> 
    where TEnum : struct, Enum
{
    internal static readonly EnumTypeInfo<TEnum> Instance = new();

    

    public static IEqualityComparer<TEnum> EqualityComparer => Instance;
    public static IComparer<TEnum> Comparer => Instance;
    
    public EnumTypeInfo()
        : base(typeof(TEnum))
    {
      
    }

   

    private static readonly Lazy<Func<TEnum, ulong>> _getULong =
        new(() => RuntimeBuilder.CreateDelegate<Func<TEnum, ulong>>(runtimeMethod =>
        {
            // Find the private method that does this
            var method = typeof(TEnum).GetMethod("ToUInt64",
                BindingFlags.NonPublic | BindingFlags.Instance,
                Type.EmptyTypes);
            if (method is null)
                throw new RuntimeException($"Cannot find {typeof(TEnum)}.ToUInt64()");
            Debug.Assert(method.ReturnType == typeof(ulong));
            runtimeMethod.Emitter
                         .Ldarg_0()
                         .Call(method)
                         .Ret();
        }));

    public static ulong GetUInt64(TEnum @enum)
    {
        return _getULong.Value(@enum);
    }


    public static TEnum Combine(TEnum first)
    {
        return first;
    }

    public static TEnum Combine(TEnum first, TEnum second)
    {
        return first.Or(second);
    }

    public static TEnum Combine(TEnum first, TEnum second, TEnum third)
    {
        return first.Or(second).Or(third);
    }

    public static TEnum Combine(params TEnum[] flags)
    {
        var e = default(TEnum);
        for (var i = 0; i < flags.Length; i++)
        {
            e = e.Or(flags[i]);
        }

        return e;
    }




    bool IEqualityComparer<TEnum>.Equals(TEnum left, TEnum right)
    {
        return Equals(left, right);
    }

    int IEqualityComparer<TEnum>.GetHashCode(TEnum @enum)
    {
        throw new NotImplementedException();
    }

    int IComparer<TEnum>.Compare(TEnum left, TEnum right)
    {
        throw new NotImplementedException();
    }
}


public class EnumMemberInfo<TEnum>
    where TEnum: struct, Enum 
{
    protected readonly FieldInfo _enumMemberField;

    public EnumTypeInfo<TEnum> EnumType => EnumTypeInfo<TEnum>.Instance;
    public string Name { get; }
    public Attribute[] Attributes { get; }
    public TEnum Member { get; }

    protected EnumMemberInfo(FieldInfo enumMemberField)
    {
        Debug.Assert(typeof(TEnum) == enumMemberField.DeclaringType);
        _enumMemberField = enumMemberField;
        
        this.Name = enumMemberField.Name;
        this.Attributes = Attribute.GetCustomAttributes(enumMemberField);
        this.Member = enumMemberField.GetValue<Types.Static, TEnum>(ref Types.Static.Instance);
    }

    internal ulong ToULong()
    {
        return EnumIL<TEnum>.ToUInt64(Member);
    }
}