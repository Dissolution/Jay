

//#define ASYNC

using Jay.Collections;
using Jay.Comparison;
using Jay.Debugging;
using Jay.Randomization;
using Jay.Reflection;
using Jay.Sandbox.T4;
using Jay.Text;
using JetBrains.Annotations;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Dynamic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using BenchmarkDotNet.Running;
using Jay.Debugging.Dumping;
using Jay.IO;
using Jay.Reflection.Cloning;
using Jay.Reflection.Emission;
using Jay.Sandbox.Reflection;
using Corvidae;
using Jay.Benchmarks;
using Jay.Reflection.Runtime;
using Microsoft.Extensions.Logging;
using Serilog;
using Serilog.Core;
using Serilog.Extensions.Logging;
using Serilog.Sinks.SystemConsole.Themes;
using ILogger = Microsoft.Extensions.Logging.ILogger;


#pragma warning disable 1998

namespace Jay.Sandbox
{
    public class ExtList<T> : List<T>
    {
        
    }
    
    internal class Program
    {
        private static readonly ILoggerFactory _loggerFactory;

        static Program()
        {
            var serilogConfig = new LoggerConfiguration()
                                .MinimumLevel.Verbose()
                                .WriteTo.Console(theme: SystemConsoleTheme.Colored);
            var serilogLogger = serilogConfig.CreateLogger();
            _loggerFactory = new SerilogLoggerFactory(serilogLogger);
        }
       
#if ASYNC
        public static async Task<int> Main(params string?[] args)
        {
        }
#else
        private delegate void BlockCopyDelegate(ReadOnlySpan<char> source, Span<char> dest);
        private delegate void StringCopyDelegate(string source, char[] dest);

        private delegate ref T GetRef<T>(ReadOnlySpan<char> span);

        private delegate string ToStringDel(ReadOnlySpan<char> span);
        
        private static int Main(params string?[] args)
        {
        //     var strFirstCharField = typeof(string).GetField("_firstChar",
        //                                                     Reflect.InstanceFlags)
        //                                           .ThrowIfNull();
        //     var strLengthField = typeof(string).GetField("_stringLength",
        //                                                  Reflect.InstanceFlags)
        //                                        .ThrowIfNull();
        //     var stringCopy = DelegateBuilder.Emit<StringCopyDelegate>(emitter =>
        //     {
        //         emitter.LoadArgument(1)
        //                .Call(typeof(MemoryMarshal)
        //                      .GetMethod(nameof(MemoryMarshal.GetArrayDataReference), Reflect.StaticFlags)
        //                      .MakeGenericMethod(typeof(char)))
        //                .LoadArgument(0)
        //                .LoadFieldAddress(strFirstCharField)
        //                .LoadArgument(0)
        //                .LoadField(strLengthField)
        //                .LoadConstant(sizeof(char))
        //                .Multiply()
        //                .Generate(gen => gen.Cpblk())
        //                .Return();
        //         var str = emitter.ToString();
        //         Hold.Debug(str);
        //     });
        //
        //     var dest = new char[2000];
        //     stringCopy("alphabet_maximum", dest);
        //     Hold.Debug(dest);

            var results = BenchmarkHelper.RunAndOpen<TextCopyBenchmarks>();
            if (results.HasCriticalValidationErrors)
            {
                foreach (var ve in results.ValidationErrors)
                {
                    Console.WriteLine(ve);
                }
            }

            // string t = "joes";
            // var output = TextBuilder.Build(text => text.AppendFormat($"Eat at {t,12}"));
            //
            // Hold.Debug(output);
            
            //Console.WriteLine("Press Enter to close");
            //Console.ReadLine();
            return 0;
        }
#endif
    }
    
    
    

    [StructLayout(LayoutKind.Explicit)]
    public struct TestUnmanaged
    {
        [FieldOffset(0)] 
        public int FirstInt;

        // [FieldOffset(4)]
        // public int SecondInt;
        //
        // [FieldOffset(8)]
        // public int ThirdInt;
        //
        // [FieldOffset(12)]
        // public int FourthInt;
        //
        // [FieldOffset(16)]
        // public int FifthInt;
    }
    
    public static class TestMethods
    {
        public static int HalfRoundUp(int value)
        {
            return (value >> 1) + (value & 1);
        }
        
        public static int HalfRoundDown(int value)
        {
            return (value >> 1);
        }
    }


    public interface ITestClass
    {
        int Id { get; set; }
        string Name { get; set; }
    }
    
    public class ExtTestClass : TestClass
    {
        
    }
    
    public class TestClass : INotifyPropertyChanged, ITestClass
    {
        public int Id { get; set; }
        public string Name { get; set; }
        
        public event PropertyChangedEventHandler? PropertyChanged;

        public TestClass()
        {
            this.Id = Randomizer.Instance.Int();
            this.Name = Randomizer.Instance.GuidString();
        }
        
        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string? propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public void Mutate()
        {
            this.Id = Randomizer.Instance.Int();
            this.Name = Randomizer.Instance.GuidString();
        }
        
        /// <inheritdoc />
        public override string ToString()
        {
            return $"#{Id} - {Name}";
        }
    }
}