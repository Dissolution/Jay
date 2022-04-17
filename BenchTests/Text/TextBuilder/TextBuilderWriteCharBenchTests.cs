using System.Diagnostics.CodeAnalysis;
using BenchmarkDotNet.Attributes;
using Jay.Text.Scratch;

namespace Jay.BenchTests.Text;

[ShortRunJob]
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

    [Benchmark]
    public void WriteA()
    {
        using TextBuilder text = stackalloc char[TextBuilder.MinCapacity];
        for (int i = 0; i <= char.MaxValue; i++)
        {
            text.WriteCharA((char)i);
        }
    }
   
    
    [Benchmark(Baseline = true)]
    public void WriteB()
    {
        using TextBuilder text = stackalloc char[TextBuilder.MinCapacity];
        for (int i = 0; i <= char.MaxValue; i++)
        {
            text.WriteCharB((char)i);
        }
    }
    
    [Benchmark]
    public void Write_BB()
    {
        using TextBuilder text = stackalloc char[TextBuilder.MinCapacity];
        for (int i = 0; i <= char.MaxValue; i++)
        {
            text.WriteChar_BB((char)i);
        }
    }
    
    [Benchmark]
    public void WriteC()
    {
        using TextBuilder text = stackalloc char[TextBuilder.MinCapacity];
        for (int i = 0; i <= char.MaxValue; i++)
        {
            text.WriteCharC((char)i);
        }
    }
    
    [Benchmark()]
    public void Write_CB()
    {
        using TextBuilder text = stackalloc char[TextBuilder.MinCapacity];
        for (int i = 0; i <= char.MaxValue; i++)
        {
            text.WriteChar_CB((char)i);
        }
    }

    [Benchmark]
    public void WriteC1()
    {
        using TextBuilder text = stackalloc char[TextBuilder.MinCapacity];
        for (int i = 0; i <= char.MaxValue; i++)
        {
            text.WriteCharC1((char)i);
        }
    }
    
    [Benchmark]
    public void Write_C1B()
    {
        using TextBuilder text = stackalloc char[TextBuilder.MinCapacity];
        for (int i = 0; i <= char.MaxValue; i++)
        {
            text.WriteChar_C1B((char)i);
        }
    }

    /*
    [Benchmark]
    public void WriteC2()
    {
        using TextBuilder text = stackalloc char[TextBuilder.MinCapacity];
        for (int i = 0; i <= char.MaxValue; i++)
        {
            text.WriteCharC2((char)i);
        }
    }
    */

    [Benchmark]
    public void WriteC3()
    {
        using TextBuilder text = stackalloc char[TextBuilder.MinCapacity];
        for (int i = 0; i <= char.MaxValue; i++)
        {
            text.WriteCharC3((char)i);
        }
    }

    
    [Benchmark]
    public void WriteD()
    {
        using TextBuilder text = stackalloc char[TextBuilder.MinCapacity];
        for (int i = 0; i <= char.MaxValue; i++)
        {
            text.WriteCharD((char)i);
        }
    }

    [Benchmark]
    public void WriteE()
    {
        using TextBuilder text = stackalloc char[TextBuilder.MinCapacity];
        for (int i = 0; i <= char.MaxValue; i++)
        {
            text.WriteCharE((char)i);
        }
    }
}