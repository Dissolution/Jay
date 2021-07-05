using System;
using System.Collections.Generic;
using System.Linq;
using BenchmarkDotNet.Attributes;
using Jay.Text;

namespace Jay.Benchmarks
{
    [ShortRunJob]
    public class TextEqualsBenchmarks
    {
        [ParamsSource(nameof(Strings))]
        public string First { get; set; }
       
        [ParamsSource(nameof(Strings))]
        public string Second { get; set; }
        
        public IEnumerable<string> Strings => new string[3] { string.Empty, Guid.NewGuid().ToString(), new string('x', 1024) };
        
        [Benchmark]
        public bool EnumerableSequenceEquals()
        {
            return First.SequenceEqual(Second);
        }
        
        [Benchmark]
        public bool StringEquals()
        {
            return string.Equals(First, Second);
        }
        
        [Benchmark]
        public bool TextHelperEquals()
        {
            return TextHelper.Equals(First, Second);
        }
        
        [Benchmark]
        public bool SpanSequenceEquals()
        {
            return MemoryExtensions.SequenceEqual<char>(First, Second);
        }
        
        [Benchmark]
        public bool SpanOrdinalEquals()
        {
            return MemoryExtensions.Equals(First, Second, StringComparison.Ordinal);
        }
        
        [Benchmark]
        public bool SpanCurrentEquals()
        {
            return MemoryExtensions.Equals(First, Second, StringComparison.CurrentCulture);
        }
        
        [Benchmark]
        public bool SpanInvariantEquals()
        {
            return MemoryExtensions.Equals(First, Second, StringComparison.InvariantCulture);
        }
    }
}