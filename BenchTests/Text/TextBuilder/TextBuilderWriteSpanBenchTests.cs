using System.Diagnostics.CodeAnalysis;
using BenchmarkDotNet.Attributes;
using Jay.Text;
using TextBuilder = Jay.Text.Scratch.TextBuilder;

namespace Jay.BenchTests.Text;

[LongRunJob]
[SuppressMessage("Usage", "xUnit1013:Public method should be marked as test")]
public class TextBuilderWriteSpanBenchTests
{
    public const int Iterations = 256;
    
    [GlobalSetup]
    public void GlobalSetup()
    {
        
    }


    [Benchmark(Baseline = true)]
    public void WriteSpan_3Locals()
    {
        using TextBuilder text = stackalloc char[TextBuilder.MinCapacity];
        for (var i = 0; i < Iterations; i++)
        {
            text.WriteSpan_3Locals("88888888");
            text.WriteSpan_3Locals("\r\n");
            text.WriteSpan_3Locals("666666");
            text.WriteSpan_3Locals(",");
            text.WriteSpan_3Locals("7777777");
            text.WriteSpan_3Locals(",");
            text.WriteSpan_3Locals("1616161616161616");
            text.WriteSpan_3Locals("\r\n");
            text.WriteSpan_3Locals(TextHelper.UpperCase);
            text.WriteSpan_3Locals(TextHelper.LowerCase);
            text.WriteSpan_3Locals(TextHelper.Digits);
            text.WriteSpan_3Locals(TextHelper.UpperCase);
            text.WriteSpan_3Locals(TextHelper.LowerCase);
            text.WriteSpan_3Locals(TextHelper.Digits);
            text.WriteSpan_3Locals(TextHelper.UpperCase);
            text.WriteSpan_3Locals(TextHelper.LowerCase);
            text.WriteSpan_3Locals(TextHelper.Digits);
            text.WriteSpan_3Locals(TextHelper.UpperCase);
            text.WriteSpan_3Locals(TextHelper.LowerCase);
            text.WriteSpan_3Locals(TextHelper.Digits);
            text.WriteSpan_3Locals("\r\n");
            text.WriteSpan_3Locals(Guid.Empty.ToString());
        }
    }
    
   
    
    [Benchmark]
    public void WriteSpan_SpanLen_Len()
    {
        using TextBuilder text = stackalloc char[TextBuilder.MinCapacity];
        for (var i = 0; i < Iterations; i++)
        {
            text.WriteSpan_SpanLen_Len("88888888");
            text.WriteSpan_SpanLen_Len("\r\n");
            text.WriteSpan_SpanLen_Len("666666");
            text.WriteSpan_SpanLen_Len(",");
            text.WriteSpan_SpanLen_Len("7777777");
            text.WriteSpan_SpanLen_Len(",");
            text.WriteSpan_SpanLen_Len("1616161616161616");
            text.WriteSpan_SpanLen_Len("\r\n");
            text.WriteSpan_SpanLen_Len(TextHelper.UpperCase);
            text.WriteSpan_SpanLen_Len(TextHelper.LowerCase);
            text.WriteSpan_SpanLen_Len(TextHelper.Digits);
            text.WriteSpan_SpanLen_Len(TextHelper.UpperCase);
            text.WriteSpan_SpanLen_Len(TextHelper.LowerCase);
            text.WriteSpan_SpanLen_Len(TextHelper.Digits);
            text.WriteSpan_SpanLen_Len(TextHelper.UpperCase);
            text.WriteSpan_SpanLen_Len(TextHelper.LowerCase);
            text.WriteSpan_SpanLen_Len(TextHelper.Digits);
            text.WriteSpan_SpanLen_Len(TextHelper.UpperCase);
            text.WriteSpan_SpanLen_Len(TextHelper.LowerCase);
            text.WriteSpan_SpanLen_Len(TextHelper.Digits);
            text.WriteSpan_SpanLen_Len("\r\n");
            text.WriteSpan_SpanLen_Len(Guid.Empty.ToString());
        }
    }
    
    [Benchmark]
    public void WriteSpan_Len_NewLen()
    {
        using TextBuilder text = stackalloc char[TextBuilder.MinCapacity];
        for (var i = 0; i < Iterations; i++)
        {
            text.WriteSpan_Len_NewLen("88888888");
            text.WriteSpan_Len_NewLen("\r\n");
            text.WriteSpan_Len_NewLen("666666");
            text.WriteSpan_Len_NewLen(",");
            text.WriteSpan_Len_NewLen("7777777");
            text.WriteSpan_Len_NewLen(",");
            text.WriteSpan_Len_NewLen("1616161616161616");
            text.WriteSpan_Len_NewLen("\r\n");
            text.WriteSpan_Len_NewLen(TextHelper.UpperCase);
            text.WriteSpan_Len_NewLen(TextHelper.LowerCase);
            text.WriteSpan_Len_NewLen(TextHelper.Digits);
            text.WriteSpan_Len_NewLen(TextHelper.UpperCase);
            text.WriteSpan_Len_NewLen(TextHelper.LowerCase);
            text.WriteSpan_Len_NewLen(TextHelper.Digits);
            text.WriteSpan_Len_NewLen(TextHelper.UpperCase);
            text.WriteSpan_Len_NewLen(TextHelper.LowerCase);
            text.WriteSpan_Len_NewLen(TextHelper.Digits);
            text.WriteSpan_Len_NewLen(TextHelper.UpperCase);
            text.WriteSpan_Len_NewLen(TextHelper.LowerCase);
            text.WriteSpan_Len_NewLen(TextHelper.Digits);
            text.WriteSpan_Len_NewLen("\r\n");
            text.WriteSpan_Len_NewLen(Guid.Empty.ToString());
        }
    }
    
    [Benchmark]
    public void WriteSpan_SpanLen_NewLen()
    {
        using TextBuilder text = stackalloc char[TextBuilder.MinCapacity];
        for (var i = 0; i < Iterations; i++)
        {
            text.WriteSpan_SpanLen_NewLen("88888888");
            text.WriteSpan_SpanLen_NewLen("\r\n");
            text.WriteSpan_SpanLen_NewLen("666666");
            text.WriteSpan_SpanLen_NewLen(",");
            text.WriteSpan_SpanLen_NewLen("7777777");
            text.WriteSpan_SpanLen_NewLen(",");
            text.WriteSpan_SpanLen_NewLen("1616161616161616");
            text.WriteSpan_SpanLen_NewLen("\r\n");
            text.WriteSpan_SpanLen_NewLen(TextHelper.UpperCase);
            text.WriteSpan_SpanLen_NewLen(TextHelper.LowerCase);
            text.WriteSpan_SpanLen_NewLen(TextHelper.Digits);
            text.WriteSpan_SpanLen_NewLen(TextHelper.UpperCase);
            text.WriteSpan_SpanLen_NewLen(TextHelper.LowerCase);
            text.WriteSpan_SpanLen_NewLen(TextHelper.Digits);
            text.WriteSpan_SpanLen_NewLen(TextHelper.UpperCase);
            text.WriteSpan_SpanLen_NewLen(TextHelper.LowerCase);
            text.WriteSpan_SpanLen_NewLen(TextHelper.Digits);
            text.WriteSpan_SpanLen_NewLen(TextHelper.UpperCase);
            text.WriteSpan_SpanLen_NewLen(TextHelper.LowerCase);
            text.WriteSpan_SpanLen_NewLen(TextHelper.Digits);
            text.WriteSpan_SpanLen_NewLen("\r\n");
            text.WriteSpan_SpanLen_NewLen(Guid.Empty.ToString());
        }
    }
    
    [Benchmark]
    public void WriteSpan_SpanLen()
    {
        using TextBuilder text = stackalloc char[TextBuilder.MinCapacity];
        for (var i = 0; i < Iterations; i++)
        {
            text.WriteSpan_SpanLen("88888888");
            text.WriteSpan_SpanLen("\r\n");
            text.WriteSpan_SpanLen("666666");
            text.WriteSpan_SpanLen(",");
            text.WriteSpan_SpanLen("7777777");
            text.WriteSpan_SpanLen(",");
            text.WriteSpan_SpanLen("1616161616161616");
            text.WriteSpan_SpanLen("\r\n");
            text.WriteSpan_SpanLen(TextHelper.UpperCase);
            text.WriteSpan_SpanLen(TextHelper.LowerCase);
            text.WriteSpan_SpanLen(TextHelper.Digits);
            text.WriteSpan_SpanLen(TextHelper.UpperCase);
            text.WriteSpan_SpanLen(TextHelper.LowerCase);
            text.WriteSpan_SpanLen(TextHelper.Digits);
            text.WriteSpan_SpanLen(TextHelper.UpperCase);
            text.WriteSpan_SpanLen(TextHelper.LowerCase);
            text.WriteSpan_SpanLen(TextHelper.Digits);
            text.WriteSpan_SpanLen(TextHelper.UpperCase);
            text.WriteSpan_SpanLen(TextHelper.LowerCase);
            text.WriteSpan_SpanLen(TextHelper.Digits);
            text.WriteSpan_SpanLen("\r\n");
            text.WriteSpan_SpanLen(Guid.Empty.ToString());
        }
    }
    
    [Benchmark]
    public void WriteSpan_NewLen()
    {
        using TextBuilder text = stackalloc char[TextBuilder.MinCapacity];
        for (var i = 0; i < Iterations; i++)
        {
            text.WriteSpan_NewLen("88888888");
            text.WriteSpan_NewLen("\r\n");
            text.WriteSpan_NewLen("666666");
            text.WriteSpan_NewLen(",");
            text.WriteSpan_NewLen("7777777");
            text.WriteSpan_NewLen(",");
            text.WriteSpan_NewLen("1616161616161616");
            text.WriteSpan_NewLen("\r\n");
            text.WriteSpan_NewLen(TextHelper.UpperCase);
            text.WriteSpan_NewLen(TextHelper.LowerCase);
            text.WriteSpan_NewLen(TextHelper.Digits);
            text.WriteSpan_NewLen(TextHelper.UpperCase);
            text.WriteSpan_NewLen(TextHelper.LowerCase);
            text.WriteSpan_NewLen(TextHelper.Digits);
            text.WriteSpan_NewLen(TextHelper.UpperCase);
            text.WriteSpan_NewLen(TextHelper.LowerCase);
            text.WriteSpan_NewLen(TextHelper.Digits);
            text.WriteSpan_NewLen(TextHelper.UpperCase);
            text.WriteSpan_NewLen(TextHelper.LowerCase);
            text.WriteSpan_NewLen(TextHelper.Digits);
            text.WriteSpan_NewLen("\r\n");
            text.WriteSpan_NewLen(Guid.Empty.ToString());
        }
    }
    
    [Benchmark]
    public void WriteSpan_Len()
    {
        using TextBuilder text = stackalloc char[TextBuilder.MinCapacity];
        for (var i = 0; i < Iterations; i++)
        {
            text.WriteSpan_Len("88888888");
            text.WriteSpan_Len("\r\n");
            text.WriteSpan_Len("666666");
            text.WriteSpan_Len(",");
            text.WriteSpan_Len("7777777");
            text.WriteSpan_Len(",");
            text.WriteSpan_Len("1616161616161616");
            text.WriteSpan_Len("\r\n");
            text.WriteSpan_Len(TextHelper.UpperCase);
            text.WriteSpan_Len(TextHelper.LowerCase);
            text.WriteSpan_Len(TextHelper.Digits);
            text.WriteSpan_Len(TextHelper.UpperCase);
            text.WriteSpan_Len(TextHelper.LowerCase);
            text.WriteSpan_Len(TextHelper.Digits);
            text.WriteSpan_Len(TextHelper.UpperCase);
            text.WriteSpan_Len(TextHelper.LowerCase);
            text.WriteSpan_Len(TextHelper.Digits);
            text.WriteSpan_Len(TextHelper.UpperCase);
            text.WriteSpan_Len(TextHelper.LowerCase);
            text.WriteSpan_Len(TextHelper.Digits);
            text.WriteSpan_Len("\r\n");
            text.WriteSpan_Len(Guid.Empty.ToString());
        }
    }
    
    [Benchmark]
    public void WriteSpan()
    {
        using TextBuilder text = stackalloc char[TextBuilder.MinCapacity];
        for (var i = 0; i < Iterations; i++)
        {
            text.WriteSpan("88888888");
            text.WriteSpan("\r\n");
            text.WriteSpan("666666");
            text.WriteSpan(",");
            text.WriteSpan("7777777");
            text.WriteSpan(",");
            text.WriteSpan("1616161616161616");
            text.WriteSpan("\r\n");
            text.WriteSpan(TextHelper.UpperCase);
            text.WriteSpan(TextHelper.LowerCase);
            text.WriteSpan(TextHelper.Digits);
            text.WriteSpan(TextHelper.UpperCase);
            text.WriteSpan(TextHelper.LowerCase);
            text.WriteSpan(TextHelper.Digits);
            text.WriteSpan(TextHelper.UpperCase);
            text.WriteSpan(TextHelper.LowerCase);
            text.WriteSpan(TextHelper.Digits);
            text.WriteSpan(TextHelper.UpperCase);
            text.WriteSpan(TextHelper.LowerCase);
            text.WriteSpan(TextHelper.Digits);
            text.WriteSpan("\r\n");
            text.WriteSpan(Guid.Empty.ToString());
        }
    }
}