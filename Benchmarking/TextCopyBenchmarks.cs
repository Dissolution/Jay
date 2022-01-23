using System.Diagnostics;
using System.Diagnostics.Tracing;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Security.Cryptography;
using BenchmarkDotNet.Attributes;
using Jay.Text;

namespace Jay.Benchmarking;

//[ShortRunJob]
public class TextCopyBenchmarks
{
    private static readonly string[] _sources;

    static TextCopyBenchmarks()
    {
        _sources = new string[]
        {
            string.Empty,
            ",",
            Environment.NewLine,
            "0123456789ABCDEF",
            string.Create(1000, '?', (span, c) =>
            {
                var wroteLen = span.Length.TryFormat(span, out int charsWritten);
                Debug.Assert(wroteLen);
                span[charsWritten++] = 'x';
                for (var i = charsWritten; i < span.Length; i++)
                {
                    span[i] = c;
                }
            })
        };
    }

    public IEnumerable<string> Sources() => _sources;


    protected char[] _charArray;

    public TextCopyBenchmarks()
    {

    }

    [GlobalSetup]
    public void Setup()
    {
        _charArray = new char[1024];
    }

    [Benchmark(Baseline = true)]
    [ArgumentsSource(nameof(Sources))]
    public void Write(string text)
    {
        text.CopyTo(_charArray);
    }

    /*
    [Benchmark]
    [ArgumentsSource(nameof(Sources))]
    public void Write_Bytes(string text)
    {
        var sourceBytes = MemoryMarshal.AsBytes<char>(text);
        var destBytes = MemoryMarshal.AsBytes<char>(_charArray);
        sourceBytes.CopyTo(destBytes);
    }

    [Benchmark]
    [ArgumentsSource(nameof(Sources))]
    public void Write_Span(string text)
    {
        text.AsSpan().CopyTo(_charArray.AsSpan());
    }
    */

    [Benchmark]
    [ArgumentsSource(nameof(Sources))]
    public void Write_TextHelper(string text)
    {
        TextHelper.CopyTo(in text.GetPinnableReference(),
                          ref MemoryMarshal.GetArrayDataReference<char>(_charArray),
                          text.Length);
    }


    // [Benchmark]
    // [ArgumentsSource(nameof(Things))]
    // public void CopyTo(string source, char[] dest)
    // {
    //     source.CopyTo(dest);
    // }
    //
    // [Benchmark]
    // [ArgumentsSource(nameof(Things))]
    // public void CopyToCast(string source, char[] dest)
    // {
    //     source.AsSpan().CopyTo(dest.AsSpan());
    // }
}