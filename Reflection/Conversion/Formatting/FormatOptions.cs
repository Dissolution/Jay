using Jay.Text;

namespace Jay.Reflection.Conversion.Formatting;

public readonly record struct FormatOptions(string? Format,
                                            IFormatProvider? Provider,
                                            Alignment Alignment,
                                            int? Width);
