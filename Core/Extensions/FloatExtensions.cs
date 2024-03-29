﻿namespace Jay;

/// <summary>
/// Extensions for <see cref="float"/>
/// </summary>
public static class FloatExtensions
{
    /// <summary>
    /// Is this Single equal to the specified Single?
    /// </summary>
    /// <param name="number"></param>
    /// <param name="value"></param>
    /// <returns></returns>
    public static bool IsEqual(this float number, float value)
    {
        if (EqualityComparer<float>.Default.Equals(number, value))
            return true;
        return Math.Abs(number - value) <= float.Epsilon;
    }

    /// <summary>
    /// Is this Single equal to the specified Single?
    /// </summary>
    /// <param name="number"></param>
    /// <param name="value"></param>
    /// <returns></returns>
    public static bool IsEqual(this float? number, float? value)
    {
        if (number is null && value is null)
            return true;
        if (number is null || value is null)
            return false;
        return IsEqual(number.Value, value.Value);
    }

    /// <summary>
    /// Rounds a Double value to the specified number of places.
    /// </summary>
    /// <param name="number"></param>
    /// <param name="places"></param>
    /// <returns></returns>
    public static float Round(this float number, int places)
    {
        return (float)Math.Round(number, places);
    }

    /// <summary>
    /// Rounds a Nullable Double value to the specified number of places.
    /// </summary>
    /// <param name="number"></param>
    /// <param name="places"></param>
    /// <returns></returns>
    public static float? Round(this float? number, int places)
    {
        if (number is null)
            return null;
        return (float)Math.Round(number.Value, places);
    }

    /// <summary>
    /// Returns the absolute value of a Single.
    /// </summary>
    /// <param name="number"></param>
    /// <returns></returns>
    public static float Abs(this float number)
    {
        return Math.Abs(number);
    }
    /// <summary>
    /// Returns the absolute value of a Single.
    /// </summary>
    /// <param name="number"></param>
    /// <returns></returns>
    public static float? Abs(this float? number)
    {
        if (number is null)
            return null;
        return Abs(number.Value);
    }
}