using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using BenchmarkDotNet.Attributes;
using Jay.Debugging;
using Jay.Reflection;
using Jay.Reflection.Runtime;

namespace Jay.Benchmarks
{
    [ShortRunJob]
    public class TextCopyBenchmarks
    {

        [ParamsSource(nameof(Strings))]
        public string Text { get; set; }

        private char[] _destinationArray;
        
        public IEnumerable<string> Strings => new string[]
        {
            string.Empty, 
            Environment.NewLine,
            Guid.NewGuid().ToString("N").ToUpper(),
            new string('®', 256),
            new string('x', 1024),
            new string('_', 9999),
        };

        public TextCopyBenchmarks()
        {
            _destinationArray = new char[10_000];
        }

        private delegate void BlockCopyDelegate(ReadOnlySpan<char> source, Span<char> dest);

        private delegate void StringCopyDelegate(string source, char[] dest);

        private BlockCopyDelegate _blockCopy;
        private StringCopyDelegate _stringCopy;
        
        [GlobalSetup]
        public void Setup()
        {
            var rosGetPinnableReference = typeof(ReadOnlySpan<>).MakeGenericType(typeof(char))
                                               .GetMethod(nameof(ReadOnlySpan<char>.GetPinnableReference), Reflect.InstanceFlags)
                                               .ThrowIfNull();
            var rosLength = typeof(ReadOnlySpan<>).MakeGenericType(typeof(char))
                                                  .GetField("_length", Reflect.InstanceFlags)
                                                  .ThrowIfNull();
            var sGetPinnableReference = typeof(Span<>).MakeGenericType(typeof(char))
                                     .GetMethod(nameof(Span<char>.GetPinnableReference), Reflect.InstanceFlags)
                                     .ThrowIfNull();
            _blockCopy = DelegateBuilder.Emit<BlockCopyDelegate>(emitter =>
            {
                emitter.LoadArgumentAddress(1)
                       .Call(rosGetPinnableReference)
                       .LoadArgumentAddress(0)
                       .Call(sGetPinnableReference)
                       .LoadArgumentAddress(0)
                       .LoadField(rosLength)
                       .LoadConstant(sizeof(char))
                       .Multiply()
                       .Generate(gen => gen.Cpblk())
                       .Return();
            });

            var strFirstCharField = typeof(string).GetField("_firstChar",
                                                            Reflect.InstanceFlags)
                                                  .ThrowIfNull();
            var strLengthField = typeof(string).GetField("_stringLength",
                                                         Reflect.InstanceFlags)
                                               .ThrowIfNull();
            _stringCopy = DelegateBuilder.Emit<StringCopyDelegate>(emitter =>
            {
                emitter.LoadArgument(1)
                       .Call(typeof(MemoryMarshal)
                             .GetMethod(nameof(MemoryMarshal.GetArrayDataReference), Reflect.StaticFlags)
                             .MakeGenericMethod(typeof(char)))
                       .LoadArgument(0)
                       .LoadFieldAddress(strFirstCharField)
                       .LoadArgument(0)
                       .LoadField(strLengthField)
                       .LoadConstant(sizeof(char))
                       .Multiply()
                       .Generate(gen => gen.Cpblk())
                       .Return();
                var str = emitter.ToString();
                Hold.Debug(str);
            });
        }
        
        [Benchmark]
        public TextCopyBenchmarks UnmanagedBlockCopy()
        {
            NotSafe.Unmanaged.BlockCopy(in Text.GetPinnableReference(),
                                        ref MemoryMarshal.GetArrayDataReference(_destinationArray),
                                        Text.Length);
            return this;
        }
        
        [Benchmark]
        public TextCopyBenchmarks ConstructedBlockCopy()
        {
            _blockCopy(Text, _destinationArray);
            return this;
        }
        
        [Benchmark]
        public TextCopyBenchmarks ConstructedStringCopy()
        {
            _stringCopy(Text, _destinationArray);
            return this;
        }
        
        [Benchmark]
        public TextCopyBenchmarks StringCopyTo()
        {
            Text.CopyTo(0, _destinationArray, 0, Text.Length);
            return this;
        }
        
        [Benchmark]
        public TextCopyBenchmarks ReadOnlySpanCopyTo()
        {
            Text.AsSpan().CopyTo(_destinationArray);
            return this;
        }

        
      

        // [Benchmark]
        // public char[] ArrayCopy()
        // {
        //     for (var i = 0; i < Text.Length; i++)
        //     {
        //         _destinationArray[i] = Text[i];
        //     }
        //     return _destinationArray;
        // }
        
        [Benchmark]
        public TextCopyBenchmarks BufferMemoryCopy()
        {
            unsafe
            {
                fixed (char* sourcePtr = Text)
                fixed (char* destPtr = _destinationArray)
                {
                    Buffer.MemoryCopy(sourcePtr, destPtr, _destinationArray.Length * sizeof(char), Text.Length * sizeof(char));
                }
            }
            return this;
        }
        
        [Benchmark]
        public TextCopyBenchmarks FixedUnmanagedBlockCopy()
        {
            unsafe
            {
                fixed (char* sourcePtr = Text)
                fixed (char* destPtr = _destinationArray)
                {
                    NotSafe.Unmanaged.BlockCopy(sourcePtr, destPtr, Text.Length);
                }
            }
            return this;
        }
    }
}