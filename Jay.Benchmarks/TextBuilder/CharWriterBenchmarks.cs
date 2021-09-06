namespace Jay.Benchmarks.TextBuilder
{
 using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using BenchmarkDotNet.Attributes;


    //[ShortRunJob]
    public class CharWriterBenchmarks
    {
        [ParamsSource(nameof(Counts))]
        public int Count { get; set; }
        
        public IEnumerable<int> Counts => new int[]
        {
            0,
            1,
            2,
            8,
            64,
            1024,
        };

        
        [GlobalSetup]
        public void Setup()
        {
          
        }
        
        [Benchmark]
        public string CharWriterBase()
        {
            using (var writer = new CharWriterBase())
            {
                int count = this.Count;
                for (var i = 0; i < count; i++)
                {
                    writer.Write(char.MaxValue);
                }
                return writer.ToString();
            }
        }
        
        [Benchmark]
        public string AggCharWriterBase()
        {
            using (var writer = new AggCharWriterBase())
            {
                int count = this.Count;
                for (var i = 0; i < count; i++)
                {
                    writer.Write(char.MaxValue);
                }
                return writer.ToString();
            }
        }
        
        [Benchmark]
        public string AggTCharWriterBase()
        {
            using (var writer = new AggTCharWriterBase())
            {
                int count = this.Count;
                for (var i = 0; i < count; i++)
                {
                    writer.Write(char.MaxValue);
                }
                return writer.ToString();
            }
        }
        
        [Benchmark]
        public string SplitCharWriterBase()
        {
            using (var writer = new SplitCharWriterBase())
            {
                int count = this.Count;
                for (var i = 0; i < count; i++)
                {
                    writer.Write(char.MaxValue);
                }
                return writer.ToString();
            }
        }
        
        [Benchmark]
        public string NoAggSplitCharWriterBase()
        {
            using (var writer = new NoAggSplitCharWriterBase())
            {
                int count = this.Count;
                for (var i = 0; i < count; i++)
                {
                    writer.Write(char.MaxValue);
                }
                return writer.ToString();
            }
        }
        
        [Benchmark]
        public string NoAggTSplitCharWriterBase()
        {
            using (var writer = new NoAggTSplitCharWriterBase())
            {
                int count = this.Count;
                for (var i = 0; i < count; i++)
                {
                    writer.Write(char.MaxValue);
                }
                return writer.ToString();
            }
        }
    }
}
