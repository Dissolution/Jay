using System.Diagnostics;

using static Lyph.Weight;

namespace Lyph;

/// <summary>
/// 
/// </summary>
/// <see cref="https://unicode-table.com/en/blocks/box-drawing/"/>
public sealed partial class Glyph
{
    private static readonly Glyph[] _glyphs;

    public static Glyph Empty { get; }

    static Glyph()
    {
        _glyphs = new Glyph[]
        {
            new Glyph(' ', None, None, None, None),
            new Glyph('─', Light, None, Light, None),
            new Glyph('━', Heavy, None, Heavy, None),
            new Glyph('│', None, Light, None, Light),
            new Glyph('┃', None, Heavy, None, Heavy),

        };
        Debug.Assert(_glyphs.Distinct().Count() == _glyphs.Length);
        Empty = _glyphs[0];
    }

    public static IEnumerable<Glyph> ByWeight(params Weight[] allowedWeights)
    {
        return _glyphs.Where(glyph => glyph.Weights.All(allowedWeights.Contains));
    }

    public static Glyph FromSymbol(char symbol)
    {
        return _glyphs.FirstOrDefault(glyph => glyph.Symbol == symbol) ?? Glyph.Empty;
    }
}

public sealed partial class Glyph : IEquatable<Glyph>
{
    public static bool operator ==(Glyph left, Glyph right) => left.Symbol == right.Symbol;
    public static bool operator !=(Glyph left, Glyph right) => left.Symbol != right.Symbol;

    public char Symbol { get; protected init; }
    public Cardinals<Weight> Weights { get; protected init; }

    internal Glyph(char symbol, params Weight[] weights)
    {
        Symbol = symbol;
        Debug.Assert(weights is not null && weights.Length == 4);
        Weights = new(weights[0], weights[1], weights[2], weights[3]);
    }

    public bool Equals(Glyph? glyph)
    {
        return glyph is not null && glyph.Symbol == Symbol;
    }

    public override bool Equals(object? obj)
    {
        return obj is Glyph glyph && Equals(glyph);
    }

    public override int GetHashCode()
    {
        return (int)Symbol;
    }

    public override string ToString()
    {
        return Symbol.ToString();
    }
}