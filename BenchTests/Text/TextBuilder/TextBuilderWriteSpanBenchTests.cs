using System.Diagnostics.CodeAnalysis;

using BenchmarkDotNet.Attributes;

namespace Jay.BenchTests.Text;

//[ShortRunJob]
[SuppressMessage("Usage", "xUnit1013:Public method should be marked as test")]
public class TextBuilderWriteSpanBenchTests
{
    public TextBuilderWriteSpanBenchTests()
    {

    }

    [GlobalSetup]
    public void GlobalSetup()
    {

    }

    [Fact]
    public void CanWriteA()
    {
        TextBuilder text = stackalloc char[TextBuilder.MinCapacity];
        text.WriteSpanA("abc 123");
        Assert.Equal('a', text[0]);
        Assert.Equal('b', text[1]);
        Assert.Equal('c', text[2]);
        Assert.Equal(' ', text[3]);
        Assert.Equal('1', text[4]);
        Assert.Equal('2', text[5]);
        Assert.Equal('3', text[6]);
        Assert.Equal(7, text.Length);
    }

    [Benchmark]
    public void WriteA()
    {
        TextBuilder text = stackalloc char[TextBuilder.MinCapacity];
        text.WriteSpanA("abc 123");
    }

    [Fact]
    public void CanWriteB()
    {
        TextBuilder text = stackalloc char[TextBuilder.MinCapacity];
        text.WriteSpanB("abc 123");
        Assert.Equal('a', text[0]);
        Assert.Equal('b', text[1]);
        Assert.Equal('c', text[2]);
        Assert.Equal(' ', text[3]);
        Assert.Equal('1', text[4]);
        Assert.Equal('2', text[5]);
        Assert.Equal('3', text[6]);
        Assert.Equal(7, text.Length);
    }

    [Benchmark]
    public void WriteB()
    {
        TextBuilder text = stackalloc char[TextBuilder.MinCapacity];
        text.WriteSpanB("abc 123");
    }

    [Fact]
    public void CanWriteC()
    {
        TextBuilder text = stackalloc char[TextBuilder.MinCapacity];
        text.WriteSpanC("abc 123");
        Assert.Equal('a', text[0]);
        Assert.Equal('b', text[1]);
        Assert.Equal('c', text[2]);
        Assert.Equal(' ', text[3]);
        Assert.Equal('1', text[4]);
        Assert.Equal('2', text[5]);
        Assert.Equal('3', text[6]);
        Assert.Equal(7, text.Length);
    }

    [Benchmark]
    public void WriteC()
    {
        TextBuilder text = stackalloc char[TextBuilder.MinCapacity];
        text.WriteSpanC("abc 123");
    }

    [Fact]
    public void CanWriteD()
    {
        TextBuilder text = stackalloc char[TextBuilder.MinCapacity];
        text.WriteSpanD("abc 123");
        Assert.Equal('a', text[0]);
        Assert.Equal('b', text[1]);
        Assert.Equal('c', text[2]);
        Assert.Equal(' ', text[3]);
        Assert.Equal('1', text[4]);
        Assert.Equal('2', text[5]);
        Assert.Equal('3', text[6]);
        Assert.Equal(7, text.Length);
    }

    [Benchmark]
    public void WriteD()
    {
        TextBuilder text = stackalloc char[TextBuilder.MinCapacity];
        text.WriteSpanD("abc 123");
    }

    [Fact]
    public void CanWriteE()
    {
        TextBuilder text = stackalloc char[TextBuilder.MinCapacity];
        text.WriteSpanE("abc 123");
        Assert.Equal('a', text[0]);
        Assert.Equal('b', text[1]);
        Assert.Equal('c', text[2]);
        Assert.Equal(' ', text[3]);
        Assert.Equal('1', text[4]);
        Assert.Equal('2', text[5]);
        Assert.Equal('3', text[6]);
        Assert.Equal(7, text.Length);
    }

    [Benchmark]
    public void WriteE()
    {
        TextBuilder text = stackalloc char[TextBuilder.MinCapacity];
        text.WriteSpanE("abc 123");
    }

    [Fact]
    public void CanWriteF()
    {
        TextBuilder text = stackalloc char[TextBuilder.MinCapacity];
        text.WriteSpanF("abc 123");
        Assert.Equal('a', text[0]);
        Assert.Equal('b', text[1]);
        Assert.Equal('c', text[2]);
        Assert.Equal(' ', text[3]);
        Assert.Equal('1', text[4]);
        Assert.Equal('2', text[5]);
        Assert.Equal('3', text[6]);
        Assert.Equal(7, text.Length);
    }

    [Benchmark]
    public void WriteF()
    {
        TextBuilder text = stackalloc char[TextBuilder.MinCapacity];
        text.WriteSpanF("abc 123");
    }
}