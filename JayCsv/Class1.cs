using Jay;
using Jay.Comparision;

namespace JayCsv;

/*http://super-csv.github.io/super-csv/csv_specification.html
 * 1. Each record is located on a separate line, delimited by a line break (CrLf).
 *
 *
 *
 *
 */

public class CsvOptions
{
    public static CsvOptions Default => new CsvOptions
    {
        Quote = "\"", 
        FieldDelimiter = ",", 
        LineDelimiter = Environment.NewLine
    };
    
    public string Quote { get; init; }
    public string FieldDelimiter { get; init; }
    public string LineDelimiter { get; init; }

    public CsvOptions()
    {

    }
}

public class FieldOptions
{
    public Type FieldType { get; init; }

}

public sealed class NameOptions
{
    private readonly string[] _options;

    public NameOptions(params string[] options)
    {
        if (options.Length < 1)
            throw new ArgumentException("There must be at least one Name Option", nameof(options));
        _options = options;
    }

    public override bool Equals(object? obj)
    {
        return EnumerableEqualityComparer<string>.Default.Equals(_options, 
    }

    public override int GetHashCode()
    {
        return Hasher.Create<string>(_options);
    }

    public override string ToString()
    {
        return _options[0];
    }
}



public ref struct TextScanner
{
    private readonly ReadOnlySpan<char> _text;
    private int _index;

    public TextScanner(ReadOnlySpan<char> text)
    {
        _text = text;
        _index = 0;
    }

   
}