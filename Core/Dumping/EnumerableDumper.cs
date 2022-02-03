using System.Collections;
using Jay.Text;

namespace Jay.Dumping;

public class EnumerableDumper : IDumper<IEnumerable>
{
    public void DumpValue(TextBuilder text, IEnumerable? value, DumpLevel level = DumpLevel.Default)
    {
        if (Dump.DumpNull(text, value, level)) return;
        if (value is Array array)
        {
            if (array.Rank == 1)
            {
                text.Write('[');
                for (var i = 0; i < array.Length; i++)
                {
                    if (i > 0)
                    {
                        text.Write(',');
                    }
                    text.Write(array.GetValue(i));
                }
                text.Write(']');
            }
            else
            {
                throw new NotImplementedException();
            }
        }
        else if (value is IList list)
        {
            text.Write('[');
            for (var i = 0; i < list.Count; i++)
            {
                if (i > 0)
                {
                    text.Write(',');
                }
                text.Write(list[i]);
            }
            text.Write(']');
        }
        else if (value is ICollection collection)
        {
            text.Append('(').AppendDelimit(",", collection.OfType<object?>()).Append(')');
        }
        else
        {
            text.Append('{').AppendDelimit(",", value.OfType<object?>()).Append('}');
        }
    }
}