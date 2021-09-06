using System;
using System.Collections;
using System.Reflection;
using Jay.Text;

namespace Jay.Debugging.Dumping
{
    public static class ObjectDumpExtensions
    {
        public static TextBuilder AppendDump(this TextBuilder builder,
                                             object? obj,
                                             DumpOptions? options = default)
        {
            if (!Dumper.CheckNull<object>(builder, obj))
                return builder;
            
            switch (obj)
            {
                case Type type:
                    return builder.AppendDump(type, options as MemberDumpOptions);
                case MemberInfo memberInfo:
                    return builder.AppendDump(memberInfo, options as MemberDumpOptions);
                case TimeSpan timeSpan:
                    return builder.AppendDump(timeSpan, options);
                case DateTime dateTime:
                    return builder.AppendDump(dateTime, options);
                case Guid guid:
                    return builder.AppendDump(guid, options);
                case Exception exception:
                    return builder.AppendDump(exception, options);
                case Array array:
                    return builder.AppendDump(array, options);
                case IEnumerable enumerable:
                    return builder.AppendDump(enumerable, options);
            }
            
            // Fallback
            if (options is MemberDumpOptions memberDumpOptions && memberDumpOptions.InstanceType)
            {
                builder.Append('(')
                       .AppendDump(obj.GetType())
                       .Append(") ");
            }

            return builder.Append(obj);
        }

    }
}