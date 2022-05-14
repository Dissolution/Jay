

// ReSharper disable EqualExpressionComparison

using Jay.Reflection;

namespace Jay.BenchTests;

public class BoxTests
{
    [Fact]
    public void TextObjectBoxContainsNull()
    {
        Box box = Box.Wrap((object?)null);
        Assert.True(box.ContainsNull);
    }

    [Fact]
    public void TestNullableBoxContainsNull()
    {
        Box box = Box.Wrap((int?)null);
        Assert.True(box.ContainsNull);
    }

    [Fact]
    public void TextClassBoxContainsNull()
    {
        Box box = Box.Wrap((List<int>?)null);
        Assert.True(box.ContainsNull);
    }

    [Fact]
    public void TestNullBoxEqualsNull()
    {
        Box box = Box.Wrap((object?)null);
        Assert.True(box == (object?)null);
        Assert.True(box.Equals((object?)null));
        Assert.True(box.CompareTo((object?)null) == 0);
        Assert.True(box == box);
        Assert.True(box.Equals(box));
    }
}