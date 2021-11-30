global using text = System.ReadOnlySpan<char>;
global using System.Text;
using System.Runtime.CompilerServices;

namespace Jay.Text;

public delegate void TextAction(text text);

public delegate void RefChar(ref char ch);

public delegate void RefSlider(ref TextSlider slider);

public ref struct TextSlider
{
    private readonly Span<char> _text;

    public int Index;

    public ref char Current
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        get => ref _text[Index];
    }
    public ref char this[int index] => ref _text[index];

    public Span<char> ToHere => (uint)Index < (uint)_text.Length ? _text.Slice(0, Index) : default;
    public Span<char> FromHere => (uint)Index < (uint)_text.Length ? _text.Slice(Index) : default;
    public Span<char> PastHere => (uint)Index < (uint)_text.Length - 1 ? _text.Slice(Index + 1) : default;

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public TextSlider(Span<char> text)
    {
        _text = text;
        Index = -1;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public bool MoveNext()
    {
        int index = Index + 1;
        if (index < _text.Length)
        {
            Index = index;
            return true;
        }

        return false;
    }
}