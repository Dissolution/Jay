using System.Diagnostics.CodeAnalysis;
using BenchmarkDotNet.Attributes;

namespace Jay.BenchTests.Text;

//[ShortRunJob]
[SuppressMessage("Usage", "xUnit1013:Public method should be marked as test")]
public class TextBuilderWriteCharBenchTests
{
    public TextBuilderWriteCharBenchTests()
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
    
    // [Benchmark]
    // public void WriteA()
    // {
    //     TextBuilder text = stackalloc char[TextBuilder.MinCapacity];
    //     text.WriteCharA(' ');
    // }
    
    [Fact]
    public void CanWriteB()
    {
        TextBuilder text = stackalloc char[TextBuilder.MinCapacity];
        text.WriteCharB(' ');
        Assert.Equal(' ', text[0]);
        Assert.Equal(1, text.Length);
    }
    
    // [Benchmark]
    // public void WriteB()
    // {
    //     TextBuilder text = stackalloc char[TextBuilder.MinCapacity];
    //     text.WriteCharB(' ');
    // }
    
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
        text.WriteCharC('a');
        text.WriteCharC('b');
        text.WriteCharC('c');
        text.WriteCharC(' ');
        text.WriteCharC('1');
        text.WriteCharC('2');
        text.WriteCharC('3');
    }

    [Benchmark]
    public void WriteC1()
    {
        TextBuilder text = stackalloc char[TextBuilder.MinCapacity];
        text.WriteCharC1('a');
        text.WriteCharC1('b');
        text.WriteCharC1('c');
        text.WriteCharC1(' ');
        text.WriteCharC1('1');
        text.WriteCharC1('2');
        text.WriteCharC1('3');
    }

    [Benchmark]
    public void WriteC2()
    {
        TextBuilder text = stackalloc char[TextBuilder.MinCapacity];
        text.WriteCharC2('a');
        text.WriteCharC2('b');
        text.WriteCharC2('c');
        text.WriteCharC2(' ');
        text.WriteCharC2('1');
        text.WriteCharC2('2');
        text.WriteCharC2('3');
    }

    [Benchmark]
    public void WriteC3()
    {
        TextBuilder text = stackalloc char[TextBuilder.MinCapacity];
        text.WriteCharC3('a');
        text.WriteCharC3('b');
        text.WriteCharC3('c');
        text.WriteCharC3(' ');
        text.WriteCharC3('1');
        text.WriteCharC3('2');
        text.WriteCharC3('3');
    }

    [Fact]
    public void CanWriteC1()
    {
        TextBuilder text = stackalloc char[TextBuilder.MinCapacity];
        text.WriteCharC1(' ');
        Assert.Equal(' ', text[0]);
        Assert.Equal(1, text.Length);
    }

    [Fact]
    public void CanWriteC2()
    {
        TextBuilder text = stackalloc char[TextBuilder.MinCapacity];
        text.WriteCharC2(' ');
        Assert.Equal(' ', text[0]);
        Assert.Equal(1, text.Length);
    }

   



    [Fact]
    public void CanWriteD()
    {
        TextBuilder text = stackalloc char[TextBuilder.MinCapacity];
        text.WriteCharD(' ');
        Assert.Equal(' ', text[0]);
        Assert.Equal(1, text.Length);
    }
    
    // [Benchmark]
    // public void WriteD()
    // {
    //     TextBuilder text = stackalloc char[TextBuilder.MinCapacity];
    //     text.WriteCharD(' ');
    // }
    
    [Fact]
    public void CanWriteE()
    {
        TextBuilder text = stackalloc char[TextBuilder.MinCapacity];
        text.WriteCharE(' ');
        Assert.Equal(' ', text[0]);
        Assert.Equal(1, text.Length);
    }
    
    // [Benchmark]
    // public void WriteE()
    // {
    //     TextBuilder text = stackalloc char[TextBuilder.MinCapacity];
    //     text.WriteCharE(' ');
    // }
}