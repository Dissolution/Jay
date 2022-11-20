using Jay.Text;

namespace Jay.Reflection.Conversion;

public readonly record struct FormatOptions(string? Format,
                                            IFormatProvider? Provider,
                                            Alignment Alignment,
                                            int? Width);
