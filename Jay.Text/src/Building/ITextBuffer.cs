namespace Jay.Text.Building;

public interface ITextBuffer : ITextWriter, IBuildingText
{
    ref char this[int index] { get; }
    Span<char> this[Range range] { get; set; }
    int Length { get; set; }
    int Capacity { get; }
    Span<char> Written { get; }
    Span<char> Available { get; }

    /// <summary>
    /// Allocates a new <see cref="char"/> at the beginning of <see cref="Available"/>,
    /// increases <see cref="Length"/> by 1,
    /// and returns a <c>ref</c> to that <see cref="char"/>
    /// </summary>
    /// <returns></returns>
    ref char Allocate();

    /// <summary>
    /// Allocates a new <see cref="char"/> at <paramref name="index"/>,
    /// shifts existing chars to make an empty hole,
    /// increases <see cref="Length"/> by 1,
    /// and returns a <c>ref</c> to that <see cref="char"/>
    /// </summary>
    /// <param name="index">The index to allocate a character at</param>
    /// <returns></returns>
    ref char AllocateAt(int index);
    
    /// <summary>
    /// Allocates a <c>Span&lt;char&gt;</c> at the beginning of <see cref="Available"/>,
    /// increases <see cref="Length"/> by <paramref name="count"/>
    /// and returns the allocated <c>Span&lt;char&gt;</c>
    /// </summary>
    /// <param name="count">The number of characters to allocate space for</param>
    Span<char> Allocate(int count);

    /// <summary>
    /// Allocates a <c>Span&lt;char&gt;</c> at <paramref name="index"/>,
    /// shifts existing chars to make an empty hole,
    /// increases <see cref="Length"/> by <paramref name="length"/>
    /// and returns the allocated <c>Span&lt;char&gt;</c>
    /// </summary>
    /// <param name="index">The index to allocate the span at</param>
    /// <param name="length">The number of characters to allocate space for</param>
    Span<char> AllocateRange(int index, int length);

    Span<char> AllocateRange(Range range);


    /// <summary>
    /// Removes the <see cref="char"/> at the given <paramref name="index"/>
    /// and shifts existing <see cref="Written"/> to cover the hole
    /// </summary>
    /// <param name="index">The index of the char to delete</param>
    void RemoveAt(int index);

    /// <summary>
    /// Removes the <see cref="char"/>s from the given <paramref name="index"/> for the given <paramref name="length"/>
    /// and shifts existing <see cref="Written"/> to cover the hole
    /// </summary>
    /// <param name="index">The index of the first char to delete</param>
    /// <param name="length">The number of chars to delete</param>
    void RemoveRange(int index, int length);

    void RemoveRange(Range range);

    void TrimStart();
    void TrimEnd();
    
    void Replace(char oldChar, char newChar);

    void Replace(
        scoped ReadOnlySpan<char> oldText,
        scoped ReadOnlySpan<char> newText,
        StringComparison comparison = StringComparison.Ordinal);

    void Replace(
        string oldStr,
        string newStr,
        StringComparison comparision = StringComparison.Ordinal);

    void Clear();
}