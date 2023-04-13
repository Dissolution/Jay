using System.Numerics;
using Jay.Utilities;

namespace Jay.BenchTests;

public class RadixTests
{
    [Fact]
    public void TestPredefinedBases()
    {
        var radishes = new int[] { 2, 8, 10, 16, 62, 64 };
        foreach (var radix in radishes)
        {
            bool got = Radix.TryGetDefaultChars((ulong)radix, out string? chars);
            Assert.True(got);
            Assert.NotNull(chars);
            Assert.Equal(radix, chars.Length);
            var uniqueCount = chars.Distinct().Count();
            Assert.Equal(radix, uniqueCount);
        }
    }

    [Fact]
    public void TestAddBase()
    {
        ulong radix = 11;
        bool got = Radix.TryGetDefaultChars(radix, out string? chars);
        Assert.False(got);
        Assert.Null(chars);
        Radix.SetDefaultChars(radix, "~!@#$%^&*()");
        got = Radix.TryGetDefaultChars(radix, out chars);
        Assert.True(got);
        Assert.NotNull(chars);
        Assert.Equal((int)radix, chars.Length);
        var uniqueCount = chars.Distinct().Count();
        Assert.Equal((int)radix, uniqueCount);
    }

    [Fact]
    public void TestConvertBase10ToULong()
    {
        foreach (string input in new string[]
                 {
                     null!,
                     string.Empty,
                     "0",
                     "147",
                     "1234567890",
                     "0987654321",
                     "eight",
                     "\n",
                 })
        {
            Radix.RadixData radixInput = new(input, 10);
            bool toRadix = Radix.TryConvert(radixInput, out var base10);
            bool toULong = ulong.TryParse(input, out var uint64);
            Assert.Equal(toRadix, toULong);
            Assert.Equal(base10, uint64);
            if (toRadix)
            {
                ulong converted = Convert.ToUInt64(value: input, fromBase: 10);
                Assert.Equal(converted, base10);
            }
        }
    }
    
    [Fact]
    public void TestConvertBase2ToULong()
    {
        var failedInputs = new string[] { null!, string.Empty, "147", };
        foreach (var input in failedInputs)
        {
            Radix.RadixData radixInput = new(input, 2);
            bool toRadix = Radix.TryConvert(radixInput, out var base10);
            Assert.False(toRadix);
        }

        var goodInputs = new string[] { "0", "11", "11111111", "1100110011001100", };
        foreach (var input in goodInputs)
        {
            Radix.RadixData radixInput = new(input, 2);
            bool toRadix = Radix.TryConvert(radixInput, out var base10);
            Assert.True(toRadix);
            
            ulong converted = Convert.ToUInt64(value: input, fromBase: 2);
            Assert.Equal(converted, base10);
        }
    }
}