using System.Collections;

namespace Jay.Text;

public interface ITextComparer : IComparer<string?>,
                                 IComparer<char[]?>,
                                 IComparer
{
    int Compare(ReadOnlySpan<char> x, ReadOnlySpan<char> y);
}