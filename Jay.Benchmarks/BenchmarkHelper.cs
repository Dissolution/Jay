using System;
using System.IO;
using BenchmarkDotNet.Configs;
using BenchmarkDotNet.Reports;
using BenchmarkDotNet.Running;
using Jay.Debugging;
using Jay.IO;

namespace Jay.Benchmarks
{
    public static class BenchmarkHelper
    {
        private static readonly IConfig _config;

        static BenchmarkHelper()
        {
            _config = DefaultConfig.Instance;
        }

        public static Summary RunAndOpen<TBenchmark>()
        {
            var summary = BenchmarkRunner.Run<TBenchmark>(_config);

            //Console.WriteLine(Hold.Dump(summary));

            var htmlReport = Path.Combine(summary.ResultsDirectoryPath);

            FileSystem.Execute(htmlReport);
            return summary;
        }
    }
}