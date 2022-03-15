using System.Globalization;

namespace Jay.Conversion;

public readonly record struct ParseOptions(NumberStyles NumberStyle,
                                           DateTimeStyles DateTimeStyle);