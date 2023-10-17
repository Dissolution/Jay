namespace Jay.Text.Tests.TextBuilderTests;

public class InterpolatedTextBuilderTests
{
    [Fact]
    public void InterpolatedWrite()
    {
        using var text = new TextBuilder();
        Assert.Equal(0, text.Length);
        
        string abc = "ABC";
        text.Append($"{abc}");
        Assert.Equal(3, text.Length);
        Assert.Equal(abc, text.ToString());
        
        text.Clear();
        Assert.Equal(0, text.Length);
        text.Append($"{DateTime.Now:s}");
        Assert.Equal(19, text.Length);

        text.Clear();
        Assert.Equal(0, text.Length);
        Action<TextBuilder> tba = DoThing;
        text.Append($"HEY {tba} YA!");
        Assert.Equal(11, text.Length);
    }

    protected static void DoThing(TextBuilder textBuilder)
    {
        textBuilder.Write("ABC");
    }
}