using System;

namespace Jay.Debugging
{
    public readonly struct DumpOptions
    {
        public readonly bool Verbose;
        public readonly string[]? Formats;
        public readonly IFormatProvider? FormatProvider;

        public string? Format
        {
            get
            {
                if (Formats is null)
                    return null;
                if (Formats.Length == 0)
                    return null;
                return Formats[0];
            }
        }

    }
}