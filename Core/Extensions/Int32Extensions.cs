namespace Jay.Extensions;

public static class Int32Extensions
{
    public static int DigitCount(this int integer)
    {
        if (integer >= 0)
        {
            if (integer < 10) return 1;
            if (integer < 100) return 2;
            if (integer < 1000) return 3;
            if (integer < 10000) return 4;
            if (integer < 100000) return 5;
            if (integer < 1000000) return 6;
            if (integer < 10000000) return 7;
            if (integer < 100000000) return 8;
            if (integer < 1000000000) return 9;
            return 10;
        }
        else
        {
            if (integer > -10) return 2;
            if (integer > -100) return 3;
            if (integer > -1000) return 4;
            if (integer > -10000) return 5;
            if (integer > -100000) return 6;
            if (integer > -1000000) return 7;
            if (integer > -10000000) return 8;
            if (integer > -100000000) return 9;
            if (integer > -1000000000) return 10;
            return 11;
        }
    }
}