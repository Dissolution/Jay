using System.Linq.Expressions;
using System.Reflection.Emit;

namespace Jay.Extensions;

/// <summary>
/// Extensions for <see cref="decimal"/>
/// </summary>
public static class DecimalExtensions
{
    private static readonly GetPlaces _getPlaces;

    static DecimalExtensions()
    {
        //return (value.flags & ~int.MinValue) >> 16;

        ParameterExpression valueParam = Expression.Parameter(typeof(decimal).MakeByRefType(), "value");
        BinaryExpression digits = Expression.RightShift(
            Expression.And(
                Expression.Field(valueParam, "flags"),
                Expression.Constant(~int.MinValue, typeof(int))),
            Expression.Constant(16, typeof(int)));
        _getPlaces = Expression.Lambda<GetPlaces>(digits, valueParam).Compile();
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
    /// Returns the absolute value of a Decimal.
    /// </summary>
    /// <param name="number"></param>
    /// <returns></returns>
    public static decimal Abs(this decimal number)
    {
        return Math.Abs(number);
    }

    /// <summary>
    /// Returns the number of places of a Decimal.
    /// </summary>
    public static int Places(this decimal number)
    {
        return _getPlaces(ref number);
    }

    public static byte Scale(this decimal number)
    {
        int[] bits = decimal.GetBits(number);
        var scale = (byte)((bits[3] >> 16) & 0x7F);
        return scale;
    }

    /// <summary>
    /// https://stackoverflow.com/a/24548881
    /// </summary>
    private delegate int GetPlaces(ref decimal m);
}