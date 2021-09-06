using System;
using System.Collections.Generic;
using Jay.Collections;
using Jay.Text;

namespace Jay.Debugging.Dumping.Temp
{
    public static class Dumpers
    {
        private static readonly List<IDumper> _dumpers;
        private static readonly ConcurrentTypeCache<IDumper?> _typeDumperCache;

        static Dumpers()
        {
            _typeDumperCache = new ConcurrentTypeCache<IDumper?>();
            _dumpers = new List<IDumper>
            {
                new TypeDumper(),
                new ArrayDumper(),
            };
        }

        private static TextBuilder DumpNull(TextBuilder textBuilder,
                                            DumpOptions dumpOptions)
        {
            if (dumpOptions == DumpOptions.Surface)
                return textBuilder;
            return textBuilder.Append("null");
        }

        private static IDumper? FindDumper(Type type)
        {
            for (var i = 0; i < _dumpers.Count; i++)
            {
                var dumper = _dumpers[i];
                if (dumper.CanDump(type))
                {
                    return dumper;
                }
            }
            return null;
        }
        
        public static TextBuilder AppendDump(this TextBuilder textBuilder, 
                                             object? value,
                                             DumpOptions dumpOptions = DumpOptions.Surface)
        {
            if (value is null)
                return DumpNull(textBuilder, dumpOptions);
            Type valueType = value.GetType();
            if (!_typeDumperCache.TryGetValue(valueType, out var dumper) ||
                dumper is null)
            {
                dumper = FindDumper(valueType);
                if (dumper is null)
                {
                    dumper = DefaultDumper.Instance;
                }
                else
                {
                    _typeDumperCache.TryAdd(valueType, dumper);
                }
            }
            return dumper!.Dump(textBuilder, value, dumpOptions);
        }
    }
}