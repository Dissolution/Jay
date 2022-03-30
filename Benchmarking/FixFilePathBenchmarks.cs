using System.Text.RegularExpressions;
using BenchmarkDotNet.Attributes;

namespace Jay.Benchmarking;

[ShortRunJob]
public class FixFilePathBenchmarks
{
    private static readonly string[] _fileNames;

    static FixFilePathBenchmarks()
    {
        _fileNames = new string[]
        {
            "FileName",
            "FileName.txt",
            "File<>Name",
        };
    }

    public IEnumerable<string> FileNames => _fileNames;

    public FixFilePathBenchmarks()
    {

    }

    [Benchmark]
    [ArgumentsSource(nameof(FileNames))]
    public string SafeFileName(string input)
    {
        var invalidChars = Regex.Escape(new string(Path.GetInvalidFileNameChars()));
        var invalidRegStr = string.Format(@"([{0}]*\.+$)|([{0}]+)", invalidChars);
        return Regex.Replace(input, invalidRegStr, "_");
    }

    private static readonly Regex _fixFileNameRegex = 
        new Regex(string.Format(@"([{0}]*\.+$)|([{0}]+)", 
                                Regex.Escape(new string(Path.GetInvalidFileNameChars()))),
                  RegexOptions.Compiled);

    [Benchmark]
    [ArgumentsSource(nameof(FileNames))]
    public string StaticSafeFileName(string input)
    {
        return _fixFileNameRegex.Replace(input, "_");
    }

    [Benchmark]
    [ArgumentsSource(nameof(FileNames))]
    public string SpanSetFix(string input)
    {
        return string.Create(input.Length, input, (span, fileName) =>
        {
            var badChars = Path.GetInvalidFileNameChars().ToHashSet();
            for (var i = span.Length - 1; i >= 0; i--)
            {
                var f = fileName[i];
                if (badChars.Contains(f))
                    span[i] = '_';
                else
                    span[i] = f;
            }
        });
    }

    [Benchmark]
    [ArgumentsSource(nameof(FileNames))]
    public string SpanFix(string input)
    {
        return string.Create(input.Length, input, (span, fileName) =>
        {
            var badChars = Path.GetInvalidFileNameChars();
            for (var i = span.Length - 1; i >= 0; i--)
            {
                var f = fileName[i];
                if (badChars.Contains(f))
                    span[i] = '_';
                else
                    span[i] = f;
            }
        });
    }
}