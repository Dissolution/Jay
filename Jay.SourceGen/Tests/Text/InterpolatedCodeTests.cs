using Jay.SourceGen.Text;

namespace Jay.SourceGen.Tests.Text;

public class InterpolatedCodeTests
{
    [Fact]
    public void AdvancedFormatters()
    {
        using var codeBuilder = new CodeBuilder();
        codeBuilder.Append($"Eat At {"joe's":U}!");

        codeBuilder.Append($"{147:Casing.Upper}");
        
        Assert.Equal("Eat At JOE'S!", codeBuilder.ToString());
    }
}