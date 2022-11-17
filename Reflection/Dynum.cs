/*using System.Numerics;
using Jay.Reflection.Extensions;

namespace Jay.Reflection;

public struct Dynum<TEnum> :
    IEqualityOperators<Dynum<TEnum>, TEnum, bool>,
    IComparisonOperators<Dynum<TEnum>, TEnum, int>,
    IBitwiseOperators<Dynum<TEnum>>

IEquatable<TEnum>
    where TEnum : struct, Enum
{
    public static bool IsFlags { get; } = typeof(TEnum).HasAttribute<FlagsAttribute>();
}*/