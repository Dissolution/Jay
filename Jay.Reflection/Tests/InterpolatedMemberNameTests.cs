using Jay.Reflection.Builders;

namespace Jay.Reflection.Tests;

public class InterpolatedMemberNameTests
{
    public static string Render(ref InterpolatedMemberName name)
    {
        return name.ToStringAndDispose();
    }
    
    
    [Fact]
    public void Works()
    {
        string a = Render($"ABC");
        Assert.Equal("ABC", a);
        string b = Render($"Eat {'@'} Joe's");
        Assert.Equal("Eat @ Joe's", b);
        string c = Render($"{1.0} + {1.0} != {2.0}");
        Assert.Equal("1 + 1 != 2", c);
    }
}