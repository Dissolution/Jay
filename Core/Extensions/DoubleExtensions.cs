namespace Jay.Extensions;

/// <summary>
/// Extensions for <see cref="double"/>
/// </summary>
public static class DoubleExtensions
{
    /// <summary>
    /// Is this Double equal to the specified Double?
    /// </summary>
    public static bool IsEqual(this double number, double value, double epsilon = double.Epsilon)
    {
        return Math.Abs(number - value) <= epsilon;
    }

    /// <summary>
    /// Is this Double equal to the specified Double?
    /// </summary>
    /// <param name="number"></param>
    /// <param name="value"></param>
    /// <returns></returns>
    public static bool IsEqual(this double? number, double? value)
    {
        if (number is null && value is null)
            return true;
        if (number is null || value is null)
            return false;
        return number.Value.IsEqual(value.Value);
    }

    /// <summary>
    /// Rounds a Double value to the specified number of places.
    /// </summary>
    /// <param name="number"></param>
    /// <param name="places"></param>
    /// <returns></returns>
    public static double Round(this double number, int places)
    {
        return Math.Round(number, places);
    }

    /// <summary>
    /// Rounds a Nullable Double value to the specified number of places.
    /// </summary>
    /// <param name="number"></param>
    /// <param name="places"></param>
    /// <returns></returns>
    public static double? Round(this double? number, int places)
    {
        if (number is null)
            return null;
        return Math.Round(number.Value, places);
    }

    /// <summary>
    /// Returns the absolute value of a Double.
    /// </summary>
    /// <param name="number"></param>
    /// <returns></returns>
    public static double Abs(this double number)
    {
        return Math.Abs(number);
    }
    /// <summary>
    /// Returns the absolute value of a Double.
    /// </summary>
    /// <param name="number"></param>
    /// <returns></returns>
    public static double? Abs(this double? number)
    {
        if (number is null)
            return null;
        return number.Value.Abs();
    }
}