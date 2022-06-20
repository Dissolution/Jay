/*using Jay.Text;

namespace Jay.Dumping.Refactor;

public sealed class EnumerableDumper : ValueDumper<IEnumerable>,
                                       IDumper<IEnumerable>
{
    public static void Dump(IEnumerable? enumerable, TextBuilder text, DumpOptions? options = default)
    {
        if (DumpNull(enumerable, text, options)) return;
        if (enumerable is Array array)
        {
            // T
            TypeDumper.Dump(array.GetType().GetElementType(), text, options);
            text.Append('[')
                .AppendDelimit(",", Enumerable.Range(0, array.Rank), (tb, dimension) =>
                {
                    tb.Write(array.GetLength(dimension));
                })
                .Append("]");
            // Todo: Proper multi-rank array support
            text.Append('{')
                .AppendDelimit(",", array, (tb, obj) => tb.AppendDump(obj, options))
                .Append('}');
        }
        else if (enumerable is IList list)
        {
            // IList<T>
            TypeDumper.Dump(list.GetType(), text, options);
            text.Append('[')
                .Append(list.Count)
                .Append("]{")
                .AppendDelimit(",", list, (tb, obj) => tb.AppendDump(obj, options))
                .Append(']');
        }
        else if (enumerable is ICollection collection)
        {
            // ICollection<T>
            TypeDumper.Dump(collection.GetType(), text, options);
            text.Append('(')
                .Append(collection.Count)
                .Append("){")
                .AppendDelimit(",", collection, (tb, obj) => tb.AppendDump(obj, options))
                .Append(']');
        }
        else
        {
            TypeDumper.Dump(enumerable.GetType(), text, options);
            text.Append('{')
                .AppendDelimit(",", enumerable, (tb, obj) => tb.AppendDump(obj, options))
                .Append('}');
        }
    }
}*/