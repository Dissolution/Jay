using Jay.Collections.Iteration;

namespace Jay.Tests;

public class SpanIteratorTests
{

    [Fact]
    public void TestRefDiscard()
    {
        ReadOnlySpan<char> text = "ABCDEFGHIJKLMNOPQRSTUVWXYZ".AsSpan();
        SpanIterator<char> textIterator = new(text);
        Assert.Equal(0, textIterator.Position);
        Assert.Equal(26, textIterator.Remaining.Length);

        textIterator.TakeWhile(ch => ch == '.', out var aTaken);
        Assert.Equal(0, aTaken.Length);

        textIterator.TakeWhile(ch => ch == 'A', out var bTaken);
        Assert.Equal(1, bTaken.Length);
        Assert.Equal('A', bTaken[0]);
        Assert.Equal(1,textIterator.Position);
        Assert.Equal(25, textIterator.Remaining.Length);

        textIterator.TakeWhile(ch => ch != 'F', out var cTaken);
        Assert.Equal(4, cTaken.Length);
        Assert.Equal("BCDE", cTaken.ToString());
        Assert.Equal(5, textIterator.Position);
    }
    
    [Fact]
    public void TestRefs()
    {
        ReadOnlySpan<char> text = "ABCDEFGHIJKLMNOPQRSTUVWXYZ".AsSpan();
        SpanIterator<char> textIterator = new(text);
        Assert.Equal(0, textIterator.Position);
        Assert.Equal(26, textIterator.Remaining.Length);

        ref SpanIterator<char> a = ref textIterator.TakeWhile(ch => ch == '.', out var aTaken);
        Assert.Equal(textIterator.Position, a.Position);
        Assert.Equal(textIterator.Remaining.Length, a.Remaining.Length);
        Assert.Equal(0, aTaken.Length);

        ref SpanIterator<char> b = ref textIterator.TakeWhile(ch => ch == 'A', out var bTaken);
        Assert.Equal(textIterator.Position, a.Position);
        Assert.Equal(textIterator.Remaining.Length, a.Remaining.Length);
        Assert.Equal(textIterator.Position, b.Position);
        Assert.Equal(textIterator.Remaining.Length, b.Remaining.Length);
        Assert.Equal(1, bTaken.Length);
        Assert.Equal('A', bTaken[0]);
        Assert.Equal(1,textIterator.Position);
        Assert.Equal(25, textIterator.Remaining.Length);

        ref SpanIterator<char> c = ref textIterator.TakeWhile(ch => ch != 'F', out var cTaken);
        Assert.Equal(textIterator.Position, a.Position);
        Assert.Equal(textIterator.Remaining.Length, a.Remaining.Length);
        Assert.Equal(textIterator.Position, b.Position);
        Assert.Equal(textIterator.Remaining.Length, b.Remaining.Length);
        Assert.Equal(textIterator.Position, c.Position);
        Assert.Equal(textIterator.Remaining.Length, c.Remaining.Length);
        Assert.Equal(4, cTaken.Length);
        Assert.Equal("BCDE", cTaken.ToString());
        Assert.Equal(5, textIterator.Position);
    }

    [Fact]
    public void TestFluent()
    {
        ReadOnlySpan<char> text = "ABCDEFGHIJKLMNOPQRSTUVWXYZ".AsSpan();
        SpanIterator<char> textIterator = new(text);
        textIterator
            .TakeWhile(ch => ch != 'N', out var takenA)
            .Take(2, out var firstTwo)
            .Take(3, out var nextThree)
            .Take(4, out var lastFour);
        Assert.Equal(13, takenA.Length);
        Assert.Equal(2, firstTwo.Length);
        Assert.Equal("NO", firstTwo.ToString());
        Assert.Equal(3, nextThree.Length);
        Assert.Equal("PQR", nextThree.ToString());
        Assert.Equal(4, lastFour.Length);
        Assert.Equal("STUV", lastFour.ToString());

        textIterator.TakeWhile(_ => true, out var theRest);
        Assert.Equal(0, textIterator.Remaining.Length);
        Assert.Equal("WXYZ", theRest.ToString());
    }
}