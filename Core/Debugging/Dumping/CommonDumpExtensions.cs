using System;
using Jay.Text;

namespace Jay.Debugging.Dumping
{
    public static class CommonDumpExtensions
    {
          public static TextBuilder AppendDump(this TextBuilder builder, TimeSpan timeSpan, DumpOptions? options = default)
          {
              options ??= DumpOptions.Default;
              string? format = options.Format;
              if (format.IsNullOrWhiteSpace())
              {
                  format = "g";
              }
              if (timeSpan.TryFormat(builder.Available,
                                     out int charsWritten,
                                     format,
                                     options.FormatProvider))
              {
                  builder.Length += charsWritten;
                  return builder;
              }
  
              return builder.Append(timeSpan.ToString(format, options.FormatProvider));
        }
        
        public static TextBuilder AppendDump(this TextBuilder builder, DateTime dateTime, DumpOptions? options = default)
        {
            options ??= DumpOptions.Default;
            string? format = options.Format;
            if (format.IsNullOrWhiteSpace())
            {
                format = "yyyy-MM-dd HH:mm:ss";
            }
            if (dateTime.TryFormat(builder.Available,
                                   out int charsWritten,
                                   format,
                                   options.FormatProvider))
            {
                builder.Length += charsWritten;
                return builder;
            }

            return builder.Append(dateTime.ToString(format, options.FormatProvider));
        }
        
        public static TextBuilder AppendDump(this TextBuilder builder, Guid guid, DumpOptions? options = default)
        {
            options ??= DumpOptions.Default;
            string format = options.Format ?? "D";
            
            if (guid.TryFormat(builder.Available,
                               out int charsWritten,
                               format))
            {
                builder.Available.Slice(0, charsWritten).ConvertToUpper();
                builder.Length += charsWritten;
                return builder;
            }
            return builder.Append(guid.ToString(format).ToUpper());
        }
    }
}