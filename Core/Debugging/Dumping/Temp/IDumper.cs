using System;
using System.Diagnostics.CodeAnalysis;
using Jay.Text;

namespace Jay.Debugging.Dumping.Temp
{
    public interface IDumper
    {
        bool CanDump(Type type);

        TextBuilder Dump(TextBuilder textBuilder, 
                         object value,
                         DumpOptions dumpOptions = DumpOptions.Surface);
    }

    public interface IDumper<in T> : IDumper
    {
        bool IDumper.CanDump(Type? type) => type == typeof(T);
        
        TextBuilder Dump(TextBuilder textBuilder, 
                         T value,
                         DumpOptions options = DumpOptions.Surface);

        TextBuilder IDumper.Dump(TextBuilder textBuilder, 
                                 object value,
                                 DumpOptions options)
        {
            if (value is T typed)
            {
                return Dump(textBuilder, typed, options);
            }
            throw new ArgumentException($"Value is not a {typeof(T).Name}", nameof(value));
        }
    }
}