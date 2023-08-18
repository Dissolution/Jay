namespace Jay.Extensions;

public static class Int32Extensions
{
    public static int DigitCount(this int integer) => integer switch
    {
        >= 0 => integer switch
        {
            < 10 => 1,
            < 100 => 2,
            < 1000 => 3,
            < 10000 => 4,
            < 100000 => 5,
            < 1000000 => 6,
            < 10000000 => 7,
            < 100000000 => 8,
            < 1000000000 => 9,
            _ => 10,
        },
        > -10 => 2,
        > -100 => 3,
        > -1000 => 4,
        > -10000 => 5,
        > -100000 => 6,
        > -1000000 => 7,
        > -10000000 => 8,
        > -100000000 => 9,
        > -1000000000 => 10,
        _ => 11
    };
}