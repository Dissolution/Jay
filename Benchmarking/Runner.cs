using System.Diagnostics;
using BenchmarkDotNet.Running;

namespace Jay.Benchmarking;

public static class Runner
{
    public static Result RunAndOpenHtml()
    {
        var summary = BenchmarkRunner.Run<TextCopyBenchmarks>();
        if (summary.HasCriticalValidationErrors)
        {
            var ex = new AggregateException("", summary.ValidationErrors.Select(ve => new Exception(ve.Message)));
            return ex;
        }

        var outputPath = summary.ResultsDirectoryPath;
        var dash = summary.Title.IndexOf('-');
        Debug.Assert(dash >= 0);
        string name = summary.Title.Substring(0, dash);
        //Console.WriteLine(name);
        var file = $"{name}-report.html";
        Process.Start(new ProcessStartInfo
        {
            FileName = Path.Combine(outputPath, file),
            UseShellExecute = true,
        });
       
        return true;
    }
}