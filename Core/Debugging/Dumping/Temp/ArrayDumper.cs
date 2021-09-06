using System;
using System.Collections;
using Jay.Text;

namespace Jay.Debugging.Dumping.Temp
{
    public class ArrayDumper : IDumper<Array>
    {
        /// <inheritdoc />
        public TextBuilder Dump(TextBuilder textBuilder, 
                                Array array, 
                                DumpOptions options = DumpOptions.Surface)
        {
            if (options.HasFlag<DumpOptions>(DumpOptions.Type))
            {
                textBuilder.AppendDump(array.GetType().GetElementType());
            }
            return textBuilder.Append('[')
                              .AppendDelimit(", ",
                                             array.AsObjectEnumerable(), 
                                             (tb, obj) => tb.AppendDump(obj, options.WithoutFlag(DumpOptions.Type)))
                              .Append(']');
        }
    }
    
    // public class EnumerableDumper : IDumper<IEnumerable>
    // {
    //     /// <inheritdoc />
    //     public TextBuilder Dump(TextBuilder textBuilder, 
    //                             IEnumerable value, 
    //                             DumpOptions options = DumpOptions.Surface)
    //     {
    //         if (value is IList list)
    //         {
    //             if (options.HasFlag<DumpOptions>(DumpOptions.Type))
    //             {
    //                 textBuilder.AppendDump(list.GetType().GenericTypeArguments[0]);
    //             }
    //             return textBuilder.Append('[')
    //                               .AppendDelimit(", ",
    //                                              list, 
    //                                              (tb, obj) => tb.AppendDump(obj, options.WithoutFlag(DumpOptions.Type)))
    //                               .Append(']');
    //         }
    //     }
    // }
}