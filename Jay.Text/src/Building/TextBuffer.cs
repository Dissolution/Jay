using Jay.Collections;

namespace Jay.Text.Building;

public class TextBuffer : Buffer<char>, IBuildingText
{
    public TextBuffer()
        : base()
    {
    }

    public TextBuffer(int minCapacity)
        : base(minCapacity)
    {
    }

    public TextBuffer(int literalLength, int formattedCount)
        : base(literalLength + (formattedCount * 16))
    {
    }

    public void Add(string? str) => base.AddMany(str.AsSpan());

    public bool Contains(string str, StringComparison comparison = StringComparison.Ordinal)
    {
        return this.AsSpan().NextIndexOf(str.AsSpan(), 0, comparison) >= 0;
    }

    public int FirstIndexOf(string str, StringComparison comparison = StringComparison.Ordinal)
    {
        return this.AsSpan().NextIndexOf(str.AsSpan(), 0, comparison);
    }

    public int LastIndexOf(string str, StringComparison comparison = StringComparison.Ordinal)
    {
        return this.AsSpan().PreviousIndexOf(str.AsSpan(), Count - 1, comparison);
    }


    public string ToStringAndDispose()
    {
        string str = this.ToString();
        this.Dispose();
        return str;
    }
}