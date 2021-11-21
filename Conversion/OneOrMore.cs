using System.Collections;
using Conversion.Comparison;

namespace Conversion;

public readonly struct FormatStrings
{
    public static implicit operator FormatStrings(string? format) => new FormatStrings(format);
    public static implicit operator string?(FormatStrings formats) => formats.FirstOrDefault(null);

    private readonly string[]? _formats;

    public FormatStrings(string? format)
    {
        if (string.IsNullOrWhiteSpace(format))
        {
            _formats = null;
        }
        else
        {
            _formats = new string[1] {format};
        }
    }

    public FormatStrings(params string?[]? formats)
    {
        _formats = formats?.Where(f => !string.IsNullOrWhiteSpace(f)).ToArray()!;
    }

    public string? FirstOrDefault(string? @default = null)
    {
        if (_formats?.Length >= 1)
        {
            return _formats[0];
        }
        return @default;
    }
}