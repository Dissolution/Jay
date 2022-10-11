using Jay.Text;

namespace Jay.Dumping;



public static partial class Dumper
{
    private static void DumpArrayTo(Array? array, TextBuilder textBuilder)
    {
        if (array is null)
        {
            textBuilder.Write("null");
            return;
        }
        // T
        DumpTypeTo(array.GetType().GetElementType(), textBuilder);
        textBuilder.Append('[')
            .AppendDelimit(",", Enumerable.Range(0, array.Rank), (tb, dimension) =>
            {
                tb.Write(array.GetLength(dimension));
            })
            .Append("]");
        if (array.Rank == 1)
        {
            textBuilder.Append('{')
                .AppendDelimit(",", array, (tb, obj) => DumpObjectTo(obj, tb))
                .Append('}');
        }
        else
        {
            // Todo: Proper multi-rank array support
            throw new NotImplementedException();
        }
    }

    private static void DumpEnumerableTo<T>(IEnumerable<T> enumerable, TextBuilder textBuilder)
    {
        // TEnumerable<T>
        DumpTypeTo(enumerable.GetType(), textBuilder);
        
        // Can we show count?
        if (enumerable.TryGetNonEnumeratedCount(out int count))
        {
            textBuilder.Append('[')
                .Append(count)
                .Append(']');
        }
        
        // Dump each item in turn
        textBuilder.Append('{')
            .AppendDelimit(",", enumerable, (tb, obj) => DumpObjectTo(obj, tb))
            .Append('}');
    }
}