using BenchmarkDotNet.Attributes;

namespace Jay.BenchTests;

public class MathTests
{
    private const int Multiplier = 1_000_000;

    [Fact]
    public void PowersOfTwo()
    {
        for (var exp = 0; exp < 32; exp++)
        {
            int leftShift = 1 << exp;
            int pow = (int)Math.Pow(2.0d, (double)exp);
            Assert.Equal(pow, leftShift);
        }
    }

    [Benchmark]
    public long MathPow()
    {
        long total = 0L;
        for (var i = 0; i < Multiplier; i++)
        {
            total = 0L;
            for (var exp = 0; exp < 32; exp++)
            {
                int pow = (int)Math.Pow(2.0d, (double)exp);
                total += pow;
            }
        }
        return total;
    }

    [Benchmark]
    public long LeftShift()
    {
        long total = 0L;
        for (var i = 0; i < Multiplier; i++)
        {
            total = 0L;
            for (var exp = 0; exp < 32; exp++)
            {
                int pow = 1 << exp;
                total += pow;
            }
        }
        return total;
    }
}