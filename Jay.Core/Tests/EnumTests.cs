using System.Reflection;
using System.Security.Cryptography;
using Jay.Enums;
using Bogus;

namespace Jay.Tests;

public class EnumTests
{
    [Fact]
    public void GetFlags()
    {
        BindingFlags e = BindingFlags.Public | BindingFlags.Static | BindingFlags.Instance;
        var flags = FlagsEnumExtensions.GetFlags(e);
        Assert.Equal(3, flags.Length);
        Assert.Contains(BindingFlags.Public, flags);
        Assert.Contains(BindingFlags.Static, flags);
        Assert.Contains(BindingFlags.Instance, flags);
    }
    
    [Fact]
    public void GetFlags_Intense()
    {
        var allBindingFlags = Enum.GetValues(typeof(BindingFlags)).Cast<BindingFlags>().Skip(1).ToList();
        var r = new Randomizer();
        
        for (var i = 0; i < 100; i++)
        {
            var count = r.Number(0, allBindingFlags.Count);
            var randomFlags = r.ListItems(allBindingFlags, count);
            BindingFlags f = default;
            foreach (var flag in randomFlags)
            {
                f |= flag;
            }

            var flags = FlagsEnumExtensions.GetFlags(f);
            Assert.Equal(count, flags.Length);
            foreach (var flag in randomFlags)
            {
                Assert.Contains(flag, flags);
            }
        }
    }
}