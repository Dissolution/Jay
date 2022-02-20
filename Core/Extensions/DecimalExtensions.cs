using Jay.Reflection;
using Jay.Reflection.Building;
using Jay.Validation;

namespace Jay;

/// <summary>
/// Extensions for <see cref="decimal"/>
/// </summary>
public static class DecimalExtensions
{
    /// <summary>
    /// https://stackoverflow.com/a/24548881
    /// </summary>
    private delegate int GetPlaces(ref decimal m);

    private static readonly GetPlaces _getPlaces;

    static DecimalExtensions()
    {
        var decimalFlagsField = typeof(decimal).GetField("_flags", Reflect.InstanceFlags)
                                               .ThrowIfNull();
        _getPlaces = RuntimeBuilder.CreateDelegate<GetPlaces>("GetPlacesImpl", dm =>
        {
            dm.Emitter
              .Ldarg(0)
              .Ldfld(decimalFlagsField)
              .Ldc_I4(int.MinValue)
              .Not()
              .And()
              .Ldc_I4(16)
              .Shr()
              .Ret();
        });

        //return (value.flags & ~int.MinValue) >> 16;

        // var valueParam = Expression.Parameter(typeof(decimal).MakeByRefType(), "value");
        // var digits = Expression.RightShift(
        //     Expression.And(
        //         Expression.Field(valueParam, "flags"),
        //         Expression.Constant(~int.MinValue, typeof(int))),
        //     Expression.Constant(16, typeof(int)));
        // _getPlaces = Expression.Lambda<GetPlaces>(digits, valueParam).Compile();
    }

    /// <summary>
    /// Rounds a <see cref="decimal"/> value to the specified number of places.
    /// </summary>
    /// <param name="number"></param>
    /// <param name="places"></param>
    /// <returns></returns>
    public static decimal Round(this decimal number, int places)
    {
        return Math.Round(number, places);
    }

    /// <summary>
    /// Rounds a Nullable Decimal value to the specified number of places.
    /// </summary>
    /// <param name="number"></param>
    /// <param name="places"></param>
    /// <returns></returns>
    public static decimal? Round(this decimal? number, int places)
    {
        if (number is null)
            return null;
        return Math.Round(number.Value, places);
    }

    /// <summary>
    /// Returns the absolute value of a Decimal.
    /// </summary>
    /// <param name="number"></param>
    /// <returns></returns>
    public static decimal Abs(this decimal number)
    {
        return Math.Abs(number);
    }
    /// <summary>
    /// Returns the absolute value of a Decimal.
    /// </summary>
    /// <param name="number"></param>
    /// <returns></returns>
    public static decimal? Abs(this decimal? number)
    {
        if (number is null)
            return null;
        return Abs(number.Value);
    }

    /// <summary>
    /// Returns the number of places of a Decimal.
    /// </summary>
    /// <param name="number"></param>
    /// <returns></returns>
    public static int Places(this decimal number)
    {
        return _getPlaces(ref number);
    }
    /// <summary>
    /// Returns the number of places of a Decimal.
    /// </summary>
    /// <param name="number"></param>
    /// <returns></returns>
    public static int Places(this decimal? number)
    {
        return number is null ? 0 : Places(number.Value);
    }

    public static byte Scale(this decimal number)
    {
        int[] bits = decimal.GetBits(number);
        byte scale = (byte)((bits[3] >> 16) & 0x7F);
        return scale;
    }
}