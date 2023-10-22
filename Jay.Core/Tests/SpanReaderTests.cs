using Jay.Memory;

namespace Jay.Tests;

public class SpanReaderTests
{

    [Fact]
    public void TestRefDiscard()
    {
        ReadOnlySpan<char> text = "ABCDEFGHIJKLMNOPQRSTUVWXYZ".AsSpan();
        SpanReader<char> textIterator = new(text);
        Assert.Equal(0, textIterator.ReadCount);
        Assert.Equal(26, textIterator.UnreadItems.Length);

        var aTaken = textIterator.TakeWhile(ch => ch == '.');
        Assert.Equal(0, aTaken.Length);

        var bTaken = textIterator.TakeWhile(ch => ch == 'A');
        Assert.Equal(1, bTaken.Length);
        Assert.Equal('A', bTaken[0]);
        Assert.Equal(1,textIterator.ReadCount);
        Assert.Equal(25, textIterator.UnreadItems.Length);

        var cTaken = textIterator.TakeWhile(ch => ch != 'F');
        Assert.Equal(4, cTaken.Length);
        Assert.Equal("BCDE", cTaken.ToString());
        Assert.Equal(5, textIterator.ReadCount);
    }
}