using Jay.Text;

namespace Jay.Dumping;

public readonly struct DumpOptions : IEquatable<DumpOptions>
{
    public readonly bool Detailed = false;
    public readonly string? Format = null;
    
    public DumpOptions(bool detailed, string? format = null)
    {
        this.Detailed = detailed;
        this.Format = format;
    }

    /// <inheritdoc />
    public bool Equals(DumpOptions options)
    {
        return this.Detailed == options.Detailed &&
               string.Equals(this.Format, options.Format, StringComparison.Ordinal);
    }

    /// <inheritdoc />
    public override bool Equals(object? obj)
    {
        return obj is DumpOptions options && Equals(options);
    }

    /// <inheritdoc />
    public override int GetHashCode()
    {
        return Hasher.Create(Detailed, Format);
    }

    /// <inheritdoc />
    public override string ToString()
    {
        using var text = new TextBuilder();
        text.Write('{');
        if (Detailed)
            text.Write("Detailed,");
        if (!string.IsNullOrWhiteSpace(Format))
            text.Append("Format='").Append(Format).Write("',");
        text.Write('}');
        return text.ToString();
    }
}