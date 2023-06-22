#if NET7_0_OR_GREATER

namespace Jay.Randomization;

public partial class Rando
{
    static Rando()
    {
        var r = new Rando();
        char ch = r.Between(char.MinValue, char.MaxValue);
    }


    public T Value<T>()
        where T : unmanaged
    {
        
    }

    public T Between<T>(T inclusiveMin, T inclusiveMax)
        where T : INumber<T>, IMultiplyOperators<T, double, double>
    {
        if (inclusiveMax < inclusiveMin)
            throw new ArgumentOutOfRangeException(nameof(inclusiveMax));
        if (inclusiveMin == inclusiveMax)
            return inclusiveMin;
        T range = (inclusiveMax - inclusiveMin) + T.One;
        double value = range * this.DoublePercent();
        

    }
}

#endif

public partial class Rando
{

    /// <summary>
    /// Returns a <see cref="double"/> in the range <c>[0.0..1.0)</c>
    /// </summary>
    public double DoublePercent()
    {
        
    }
}