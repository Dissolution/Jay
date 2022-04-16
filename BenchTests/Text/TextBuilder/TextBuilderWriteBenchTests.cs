using System.Diagnostics.CodeAnalysis;
using BenchmarkDotNet.Attributes;

namespace Jay.BenchTests.Text;

//[ShortRunJob]
[LongRunJob]
[SuppressMessage("Usage", "xUnit1013:Public method should be marked as test")]
public class TextBuilderWriteBenchTests
{
    public TextBuilderWriteBenchTests()
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
        text.WriteCharA(' ');
        Assert.Equal(' ', text[0]);
        Assert.Equal(1, text.Length);
    }
    
    [Benchmark]
    public void WriteA()
    {
        TextBuilder text = stackalloc char[TextBuilder.MinCapacity];
        text.WriteCharA(' ');
        text.WriteCharA(' ');
        text.WriteCharA(' ');
        text.WriteCharA(' ');
        text.WriteCharA(' ');
    }
    
    [Fact]
    public void CanWriteB()
    {
        TextBuilder text = stackalloc char[TextBuilder.MinCapacity];
        text.WriteCharB(' ');
        Assert.Equal(' ', text[0]);
        Assert.Equal(1, text.Length);
    }
    
    [Benchmark]
    public void WriteB()
    {
        TextBuilder text = stackalloc char[TextBuilder.MinCapacity];
        text.WriteCharB(' ');
        text.WriteCharB(' ');
        text.WriteCharB(' ');
        text.WriteCharB(' ');
        text.WriteCharB(' ');
    }
    
    [Fact]
    public void CanWriteC()
    {
        TextBuilder text = stackalloc char[TextBuilder.MinCapacity];
        text.WriteCharC(' ');
        Assert.Equal(' ', text[0]);
        Assert.Equal(1, text.Length);
    }
    
    [Benchmark(Baseline = true)]
    public void WriteC()
    {
        TextBuilder text = stackalloc char[TextBuilder.MinCapacity];
        text.WriteCharC(' ');
        text.WriteCharC(' ');
        text.WriteCharC(' ');
        text.WriteCharC(' ');
        text.WriteCharC(' ');
    }
    
    [Fact]
    public void CanWriteD()
    {
        TextBuilder text = stackalloc char[TextBuilder.MinCapacity];
        text.WriteCharD(' ');
        Assert.Equal(' ', text[0]);
        Assert.Equal(1, text.Length);
    }
    
    [Benchmark]
    public void WriteD()
    {
        TextBuilder text = stackalloc char[TextBuilder.MinCapacity];
        text.WriteCharD(' ');
        text.WriteCharD(' ');
        text.WriteCharD(' ');
        text.WriteCharD(' ');
        text.WriteCharD(' ');
    }
    
    [Fact]
    public void CanWriteE()
    {
        TextBuilder text = stackalloc char[TextBuilder.MinCapacity];
        text.WriteCharE(' ');
        Assert.Equal(' ', text[0]);
        Assert.Equal(1, text.Length);
    }
    
    [Benchmark]
    public void WriteE()
    {
        TextBuilder text = stackalloc char[TextBuilder.MinCapacity];
        text.WriteCharE(' ');
        text.WriteCharE(' ');
        text.WriteCharE(' ');
        text.WriteCharE(' ');
        text.WriteCharE(' ');
    }
}