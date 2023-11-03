using System.ComponentModel;

namespace Jay.SourceGen.Text;

/// <summary>
/// An <see cref="IEnumerator{T}">IEnumerator&lt;ReadOnlySpan&lt;char&gt;&gt;</see>
/// that enumerates over a <see cref="ReadOnlySpan{T}">ReadOnlySpan&lt;char&gt;</see>
/// split by a <see cref="ReadOnlySpan{T}">Separator</see>
/// with 
/// </summary>
public ref struct TextSplitEnumerator
    //: IEnumerator<ReadOnlySpan<char>>, IEnumerator
{
    public readonly ReadOnlySpan<char> SourceText;
    public readonly ReadOnlySpan<char> Separator;
    public readonly SplitOptions SplitOptions;
    public readonly StringComparison StringComparison;

    // current read position in SourceText 
    private int _position = 0;
    private Range _currentRange = default;
    
    // just for IEnumerator support
    [EditorBrowsable(EditorBrowsableState.Never)]
    public ReadOnlySpan<char> Current => Text;
    
    /// <summary>
    /// Current slice's <see cref="Range"/> in <see cref="SourceText"/>
    /// </summary>
    public Range Range => _currentRange;

    /// <summary>
    /// Current slice's <see cref="ReadOnlySpan{T}">ReadOnlySpan&lt;char&gt;</see> from <see cref="SourceText"/>
    /// </summary>
    public ReadOnlySpan<char> Text => SourceText[_currentRange];
    
    public TextSplitEnumerator(
        ReadOnlySpan<char> sourceText,
        ReadOnlySpan<char> separator,
        SplitOptions splitOptions = SplitOptions.None,
        StringComparison stringComparison = StringComparison.Ordinal)
    {
        this.SourceText = sourceText;
        this.Separator = separator;
        this.SplitOptions = splitOptions;
        this.StringComparison = stringComparison;
    }
    
    public bool MoveNext()
    {
        int sourceLength = SourceText.Length;
        // inclusive start index
        int sliceStart;
        // exclusive end index
        int sliceEnd;

        // scan
        while (true)
        {
            // slice starts where we left off
            sliceStart = _position;

            // After the end = done enumerating
            if (sliceStart > sourceLength)
            {
                // clear after enumeration ends
                _currentRange = default;
                return false;
            }

            // At end = might need to yield a last empty slice
            if (sliceStart == sourceLength)
            {
                // Finish enumeration                 
                _position = sliceStart + 1;
                
                // If we are not removing empty lines
                if (!SplitOptions.HasFlag(SplitOptions.RemoveEmptyLines))
                {
                    // Empty
                    _currentRange = new Range(start: sliceStart, end: sliceStart);
                    return true;
                }

                // clear
                _currentRange = default;
                return false;
            }

            // Scan for next separator
            var separatorIndex = SourceText.NextIndexOf(
                Separator,
                _position,
                StringComparison);
            // None found or an empty separator yield the original
            if (separatorIndex == -1 || Separator.Length == 0)
            {
                // End of slice is end of text
                sliceEnd = SourceText.Length;
                // We're done enumerating
                _position = sliceEnd + 1;
            }
            else
            {
                // This slice ends where the separator starts
                sliceEnd = separatorIndex;
                // We'll start again where the separator ends
                _position = sliceEnd + Separator.Length;
            }
            
            // Respect StringSplitOptions
            if (SplitOptions.HasFlag(SplitOptions.TrimLines))
            {
                // Copied from ReadOnlySpan<char>.Trim()
                for (; sliceStart < sliceEnd; sliceStart++)
                {
                    if (!char.IsWhiteSpace(SourceText[sliceStart]))
                    {
                        break;
                    }
                }

                for (; sliceEnd > sliceStart; sliceEnd--)
                {
                    if (!char.IsWhiteSpace(SourceText[sliceEnd - 1]))
                    {
                        break;
                    }
                }
            }

            _currentRange = new Range(
                /* inclusive */
                start: sliceStart,
                /* exclusive */
                end: sliceEnd);

            // Respect StringSplitOptions
            if ((sliceEnd-sliceStart) > 0 || !SplitOptions.HasFlag(SplitOptions.RemoveEmptyLines))
            {
                // This is a valid return slice
                return true;
            }
            // We're not going to return this slice, told not to

            // _position has been updated, start the next scan
        }
    }
    
    public void Reset()
    {
        _position = 0;
        _currentRange = default;
    }

    [EditorBrowsable(EditorBrowsableState.Never)]
    public TextSplitEnumerator GetEnumerator() => this;
}