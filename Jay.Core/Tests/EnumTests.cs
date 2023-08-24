using System.Reflection;
using Jay.Enums;

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
}