namespace Conversion;

public readonly struct ConvTypes : IEquatable<ConvTypes>
{
    public static implicit operator ConvTypes((Type? InType, Type? OutType) tuple) => new ConvTypes(tuple.InType, tuple.OutType);

    public static bool operator ==(ConvTypes x, ConvTypes y) => x.InType == y.InType && x.OutType == y.OutType;
    public static bool operator !=(ConvTypes x, ConvTypes y) => x.InType != y.InType || x.OutType != y.OutType;

    public readonly Type? InType;
    public readonly Type? OutType;

    public ConvTypes(Type? inType, Type? outType)
    {
        this.InType = inType;
        this.OutType = outType;
    }

    public void Deconstruct(out Type? inType, out Type? outType)
    {
        inType = this.InType;
        outType = this.OutType;
    }

    public bool Equals(ConvTypes types)
    {
        return types.InType == this.InType && types.OutType == this.OutType;
    }

    public override bool Equals(object? obj)
    {
        return obj is ConvTypes types && Equals(types);
    }

    public override int GetHashCode()
    {
        return HashCode.Combine(InType, OutType);
    }

    public override string ToString()
    {
        return $"({InType},{OutType})";
    }
}