using System.Runtime.InteropServices;
using static Lyph.Scratch.Weight;
using Jay;
using Jay.Comparision;

namespace Lyph.Scratch;

public readonly struct Genome : IEquatable<Genome>
{
    public static Genome FromString(string code)
    {
        byte[] bytes = Convert.FromBase64String(code);
        Glyph[] glyphs = new Glyph[bytes.Length];
        for (var i = 0; i < bytes.Length; i++)
        {
            glyphs[i] = Glyph.Glyphs[i];
        }
        return new Genome(glyphs);
    }

    public static bool TryParse(string code, out Genome genome)
    {
        Span<byte> bytes = stackalloc byte[code.Length * 3];
        if (Convert.TryFromBase64Chars(code, bytes, out int bytesWritten))
        {
            Glyph[] glyphs = new Glyph[bytesWritten];
            for (var i = 0; i < bytesWritten; i++)
            {
                glyphs[i] = Glyph.Glyphs[i];
            }
            genome = new Genome(glyphs);
            return true;
        }

        genome = default;
        return false;
    }

    private readonly Glyph[] _glyphs;

    public Genome(Glyph[] glyphs)
    {
        _glyphs = glyphs;
        
    }

    public bool Equals(Genome genome)
    {
        return EnumerableEqualityComparer<Glyph>.Default.Equals(_glyphs, genome._glyphs);
    }

    public override string ToString()
    {
        var glyphs = _glyphs;
        Span<byte> glyphBytes = stackalloc byte[glyphs.Length];
        for (var i = 0; i < glyphs.Length; i++)
        {
            glyphBytes[i] = (byte)glyphs[i].GetHashCode();
        }
        return Convert.ToBase64String(glyphBytes);
    }
}

[Flags]
public enum Weight : byte
{
    None = 0,
    Light = 1 << 0,
    Heavy = 1 << 1,
    Double = 1 << 2,
    Dash2 = 1 << 3,
    Dash3 = 1 << 4,
    Dash4 = 1 << 5,
}


public enum Direction : byte
{
    Left,
    Top,
    Right,
    Bottom,
}

public static class DirectionExtensions
{
    public static Direction Opposite(this Direction direction)
    {
        return direction switch
        {
            Direction.Left => Direction.Right,
            Direction.Top => Direction.Bottom,
            Direction.Right => Direction.Left,
            Direction.Bottom => Direction.Top,
            _ => throw new ArgumentOutOfRangeException(nameof(direction), direction, null)
        };
    }
}

public readonly partial struct Glyph
{
    public static bool operator ==(Glyph x, Glyph y) => x.Symbol == y.Symbol;
    public static bool operator !=(Glyph x, Glyph y) => x.Symbol != y.Symbol;
    public static bool operator ==(Glyph glyph, char ch) => glyph.Equals(ch);
    public static bool operator !=(Glyph glyph, char ch) => !glyph.Equals(ch);

    private static readonly Glyph[] _glyphs;

    public static readonly Glyph Empty;

    public static IReadOnlyList<Glyph> Glyphs => _glyphs;

    static Glyph()
    {
        _glyphs = new Glyph[]
        {
            new(' ', None),

            new('─', Light, None),
            new('━', Heavy, None),
            new('│', None, Light),
            new('┃', None, Heavy),

            new('┄', Light | Dash3, None),
            new('┅', Heavy | Dash3, None),
            new('┆', None, Light | Dash3),
            new('┇', None, Heavy | Dash3),

            new('┈', Light | Dash4, None),
            new('┉', Heavy | Dash4, None),
            new('┊', None, Light | Dash4),
            new('┋', None, Heavy | Dash4),

            new('┌', None, None, Light, Light),
            new('┍', None, None, Heavy, Light),
            new('┎', None, None, Light, Heavy),
            new('┏', None, None, Heavy, Heavy),

            new('┐', Light, None, None, Light),
            new('┑', Heavy, None, None, Light),
            new('┒', Light, None, None, Heavy),
            new('┓', Heavy, None, None, Heavy),

            new('└', None, Light, Light, None),
            new('┕', None, Light, Heavy, None),
            new('┖', None, Heavy, Light, None),
            new('┗', None, Heavy, Heavy, None),

            new('┘', Light, Light, None, None),
            new('┙', Heavy, Light, None, None),
            new('┚', Light, Heavy, None, None),
            new('┛', Heavy, Heavy, None, None),

            new('├', None, Light, Light, Light),
            new('┝', None, Light, Heavy, Light),
            new('┞', None, Heavy, Light, Light),
            new('┟', None, Light, Light, Heavy),
            new('┠', None, Heavy, Light, Heavy),
            new('┡', None, Heavy, Heavy, Light),
            new('┢', None, Light, Heavy, Heavy),
            new('┣', None, Heavy, Heavy, Heavy),

            /*
U+2523
    ┤
U+2524
    ┥
U+2525
    ┦
U+2526
    ┧
U+2527
    ┨
U+2528
    ┩
U+2529
    ┪
U+252A
    ┫
U+252B
    ┬
U+252C
    ┭
U+252D
    ┮
U+252E
    ┯
U+252F
    ┰
U+2530
    ┱
U+2531
    ┲
U+2532
    ┳
U+2533
    ┴
U+2534
    ┵
U+2535
    ┶
U+2536
    ┷
U+2537
    ┸
U+2538
    ┹
U+2539
    ┺
U+253A
    ┻
U+253B
    ┼
U+253C
    ┽
U+253D
    ┾
U+253E
    ┿
U+253F
    ╀
U+2540
    ╁
U+2541
    ╂
U+2542
    ╃
U+2543
    ╄
U+2544
    ╅
U+2545
    ╆
U+2546
    ╇
U+2547
    ╈
U+2548
    ╉
U+2549
    ╊
U+254A
    ╋
U+254B
    Light and heavy dashed lines
    ╌
U+254C
    ╍
U+254D
    ╎
U+254E
    ╏
U+254F
Double lines
    ═
U+2550
    ║
U+2551
Light and double line box components
    ╒
U+2552
    ╓
U+2553
    ╔
U+2554
    ╕
U+2555
    ╖
U+2556
    ╗
U+2557
    ╘
U+2558
    ╙
U+2559
    ╚
U+255A
    ╛
U+255B
    ╜
U+255C
    ╝
U+255D
    ╞
U+255E
    ╟
U+255F
    ╠
U+2560
    ╡
U+2561
    ╢
U+2562
    ╣
U+2563
    ╤
U+2564
    ╥
U+2565
    ╦
U+2566
    ╧
U+2567
    ╨
U+2568
    ╩
U+2569
    ╪
U+256A
    ╫
U+256B
    ╬
U+256C
    Character cell arcs
    ╭
U+256D
    ╮
U+256E
    ╯
U+256F
    ╰
U+2570
Character cell diagonals
    ╱
U+2571
    ╲
U+2572
    ╳
U+2573
Light and heavy half lines
    ╴
U+2574
    ╵
U+2575
    ╶
U+2576
    ╷
U+2577
    ╸
U+2578
    ╹
U+2579
    ╺
U+257A
    ╻
U+257B
    Mixed light and heavy lines
    ╼
U+257C
    ╽
U+257D
    ╾
U+257E
    ╿
U+257F
*/
        };

        Empty = _glyphs[0];
    }

    public static bool TryParse(char ch, out Glyph glyph)
    {
        if (ch == ' ' || ch == default)
        {
            glyph = Empty;
            return true;
        }
        return _glyphs.TryGetItem(ch - '━', out glyph);
    }

    public static bool Connects(Glyph x, Direction xToY, Glyph y)
    {
        return x.Weight(xToY.Opposite()) == y.Weight(xToY);
    }
}

[StructLayout(LayoutKind.Explicit, Size = 6)]
public readonly partial struct Glyph : IEquatable<Glyph>,
                                       ISpanFormattable, IFormattable
{
    [FieldOffset(0)]
    public readonly char Symbol;
    [FieldOffset(2)]
    public readonly Weight Left;
    [FieldOffset(3)]
    public readonly Weight Top;
    [FieldOffset(4)]
    public readonly Weight Right;
    [FieldOffset(5)]
    public readonly Weight Bottom;

    public Weight CombinedWeight => Left | Top | Right | Bottom;

    private Glyph(char symbol, Weight all)
    {
        this.Symbol = symbol;
        this.Left = all;
        this.Top = all;
        this.Right = all;
        this.Bottom = all;
    }

    private Glyph(char symbol, Weight leftRight, Weight topBottom)
    {
        this.Symbol = symbol;
        this.Left = leftRight;
        this.Top = topBottom;
        this.Right = leftRight;
        this.Bottom = topBottom;
    }

    private Glyph(char symbol, Weight left, Weight top, Weight right, Weight bottom)
    {
        this.Symbol = symbol;
        this.Left = left;
        this.Top = top;
        this.Right = right;
        this.Bottom = bottom;
    }

    public Weight Weight(Direction direction)
    {
        return direction switch
        {
            Direction.Left => Left,
            Direction.Top => Top,
            Direction.Right => Right,
            Direction.Bottom => Bottom,
            _ => throw new ArgumentOutOfRangeException(nameof(direction), direction, null)
        };
    }

    public bool Equals(Glyph glyph)
    {
        return Symbol == glyph.Symbol;
    }

    public bool Equals(char ch)
    {
        return Symbol == ch;
    }

    public override bool Equals(object? obj)
    {
        if (obj is Glyph glyph || (obj is char ch && TryParse(ch, out glyph)))
        {
            return Equals(glyph);
        }
        return false;
    }

    public override int GetHashCode()
    {
        if (Symbol == ' ') return 0;
        return Symbol - '━';
    }


    public bool TryFormat(Span<char> destination, out int charsWritten, ReadOnlySpan<char> format, IFormatProvider? provider)
    {
        charsWritten = 1;
        if (destination.Length > 0)
        {
            destination[0] = Symbol;
            return true;
        }
        return false;
    }


    public string ToString(string? format, IFormatProvider? formatProvider)
    {
        return Symbol.ToString();
    }

    public override string ToString()
    {
        return Symbol.ToString();
    }
}

