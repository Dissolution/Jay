using System;
using System.Reflection;
using Jay.Reflection.Emission;
using Jay.Text;

namespace Jay.Debugging.Dumping.Temp
{
    public sealed class DefaultDumper : IDumper
    {
        public static IDumper Instance { get; } = new DefaultDumper();
        
        /// <inheritdoc />
        public bool CanDump(Type? type) => true;

        /// <inheritdoc />
        public TextBuilder Dump(TextBuilder textBuilder, 
                                object? value, 
                                DumpOptions dumpOptions = DumpOptions.Surface)
        {
            if (value is null)
            {
                if (dumpOptions == DumpOptions.Surface)
                    return textBuilder;
                return textBuilder.Append("[null]");
            }
            
            if (dumpOptions == DumpOptions.Surface)
            {
                return textBuilder.Append(value);
            }

            var valueType = value.GetType();
            return textBuilder.AppendDump(valueType)
                              .AppendNewLine()
                              .Append('{')
                              .Indent("\t", iText =>
                              {
                                  iText.AppendNewLine(valueType.GetProperties(BindingFlags.Public | BindingFlags.Instance),
                                                   (piText, pi) =>
                                                   {
                                                       piText.Append(pi.Name)
                                                           .Append(": ")
                                                           .AppendDump(pi.GetValue(value), dumpOptions);
                                                   });
                              }).AppendNewLine()
                              .Append('}')
                              .AppendNewLine();
        }
    }
}