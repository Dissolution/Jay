namespace Jay.Benchmarks.TextBuilder
{
 using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using BenchmarkDotNet.Attributes;


    [ShortRunJob]
    public class TBWriteBenchmarks
    {
        [ParamsSource(nameof(Strings))]
        public string Text { get; set; }
        
        public IEnumerable<string> Strings => new string[]
        {
            string.Empty,
            "J",
            Environment.NewLine,
            Guid.NewGuid().ToString("N").ToUpper(),
            new string('®', 256),
            new string('x', 1024),
            new string('_', 9999),
        };

        
        [GlobalSetup]
        public void Setup()
        {
          
        }
        
        [Benchmark]
        public string WriteA()
        {
            using (var writer = new BuilderA())
            {
                writer.Write('[');
                for (var i = 0; i < 16; i++)
                {
                    if (i > 0)
                        writer.Write(',');
                    writer.Write(Text);
                }
                writer.Write(']');
                return writer.ToString();
            }
        }
        
        // [Benchmark]
        // public string WriteA2()
        // {
        //     using (var writer = new BuilderA2())
        //     {
        //         writer.Write('[');
        //         for (var i = 0; i < 16; i++)
        //         {
        //             if (i > 0)
        //                 writer.Write(',');
        //             writer.Write(Text);
        //         }
        //         writer.Write(']');
        //         return writer.ToString();
        //     }
        // }
        
        [Benchmark]
        public string WriteB()
        {
            using (var writer = new BuilderB())
            {
                writer.Write('[');
                for (var i = 0; i < 16; i++)
                {
                    if (i > 0)
                        writer.Write(',');
                    writer.Write(Text);
                }
                writer.Write(']');
                return writer.ToString();
            }
        }
        
        // [Benchmark]
        // public string WriteB2()
        // {
        //     using (var writer = new BuilderB2())
        //     {
        //         writer.Write('[');
        //         for (var i = 0; i < 16; i++)
        //         {
        //             if (i > 0)
        //                 writer.Write(',');
        //             writer.Write(Text);
        //         }
        //         writer.Write(']');
        //         return writer.ToString();
        //     }
        // }
        
        [Benchmark]
        public string WriteC()
        {
            using (var writer = new BuilderC())
            {
                writer.Write('[');
                for (var i = 0; i < 16; i++)
                {
                    if (i > 0)
                        writer.Write(',');
                    writer.Write(Text);
                }
                writer.Write(']');
                return writer.ToString();
            }
        }
        
        [Benchmark]
        public string WriteD()
        {
            using (var writer = new BuilderD())
            {
                writer.Write('[');
                for (var i = 0; i < 16; i++)
                {
                    if (i > 0)
                        writer.Write(',');
                    writer.Write(Text);
                }
                writer.Write(']');
                return writer.ToString();
            }
        }
        
        [Benchmark]
        public string WriteE()
        {
            using (var writer = new BuilderE())
            {
                writer.Write('[');
                for (var i = 0; i < 16; i++)
                {
                    if (i > 0)
                        writer.Write(',');
                    writer.Write(Text);
                }
                writer.Write(']');
                return writer.ToString();
            }
        }
        
        [Benchmark]
        public string WriteF()
        {
            using (var writer = new BuilderF())
            {
                writer.Write('[');
                for (var i = 0; i < 16; i++)
                {
                    if (i > 0)
                        writer.Write(',');
                    writer.Write(Text);
                }
                writer.Write(']');
                return writer.ToString();
            }
        }
    
    }
}
