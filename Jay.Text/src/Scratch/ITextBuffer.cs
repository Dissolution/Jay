namespace Jay.Text.Scratch;

public interface ITextBuffer : ITextWriter
{
    /// <summary>
    /// Gets or sets the <see cref="char"/> at <paramref name="index"/>
    /// </summary>
    /// <param name="index">
    /// The character's index in <see cref="Written"/> to get or set
    /// </param>
    char this[int index] { get; set; }
    
    /// <summary>
    /// Gets or sets the <see cref="Span{T}">Span&lt;char&gt;</see> at <paramref name="range"/>
    /// </summary>
    Span<char> this[Range range] { get; set; }
    
    /// <summary>
    /// Gets the count of characters written
    /// </summary>
    int Length { get; }
    
    /// <summary>
    /// Gets the current capacity to store characters
    /// </summary>
    /// <remarks>
    /// This will automatically be increased as necessary
    /// </remarks>
    int Capacity { get; }

    /// <summary>
    /// Gets the <see cref="Span{T}">Span&lt;char&gt;</see> that have been written
    /// </summary>
    Span<char> Written { get; }

    /// <summary>
    /// Gets the <see cref="Span{T}">Span&lt;char&gt;</see> that is currently available
    /// </summary>
    /// <remarks>
    /// If this is written to in any way, <see cref="Length"/> must be increased!
    /// </remarks>
    Span<char> Available { get; }

    /// <summary>
    /// Allocates a new <see cref="char"/> at the beginning of <see cref="Available"/>,
    /// increases <see cref="Length"/> by 1,
    /// and returns a <c>ref</c> to that <see cref="char"/>
    /// </summary>
    ref char Allocate();

    /// <summary>
    /// Allocates a <see cref="Span{T}">Span&lt;char&gt;</see> at the beginning of <see cref="Available"/>,
    /// increases <see cref="Length"/> by <paramref name="length"/>
    /// and returns the allocated <see cref="Span{T}">Span&lt;char&gt;</see>
    /// </summary>
    /// <param name="length">The number of characters to allocate space for</param>
    Span<char> Allocate(int length);

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
    /// Allocates a <see cref="Span{T}">Span&lt;char&gt;</see> at <paramref name="index"/>,
    /// shifts existing chars to make an empty hole,
    /// increases <see cref="Length"/> by <paramref name="length"/>
    /// and returns the allocated <see cref="Span{T}">Span&lt;char&gt;</see>
    /// </summary>
    /// <param name="index">The index to allocate the span at</param>
    /// <param name="length">The number of characters to allocate space for</param>
    Span<char> AllocateAt(int index, int length);
    
    void RemoveFirst(int count);

    void RemoveLast(int count);
    
    /// <summary>
    /// Removes the <see cref="char"/> at the given <paramref name="index"/>
    /// and shifts existing <see cref="Written"/> to cover the hole
    /// </summary>
    /// <param name="index">The index of the char to delete</param>
    void RemoveAt(Index index);

    /// <summary>
    /// Removes the <see cref="char"/>s from the given <paramref name="index"/> for the given <paramref name="length"/>
    /// and shifts existing <see cref="Building.TextWriter.Written"/> to cover the hole
    /// </summary>
    /// <param name="index">The index of the first char to delete</param>
    /// <param name="length">The number of chars to delete</param>
    void RemoveRange(int index, int length);

    void RemoveRange(Range range);

    void RemoveWhere(Func<char, bool> matchChar);

    void RemoveWhere(char matchChar);

    void RemoveWhere(IReadOnlyCollection<char> matchChars);
    
    void RemoveWhere(scoped ReadOnlySpan<char> matchText, StringComparison comparison = StringComparison.Ordinal);

    void RemoveWhere(string matchStr, StringComparison comparison = StringComparison.Ordinal);

    void TrimStart();

    void TrimEnd();
    
    /// <summary>
    /// Gets the written <see cref="string"/> and <see cref="Dispose"/> this <see cref="ITextBuffer"/>
    /// </summary>
    /// <returns></returns>
    string ToStringAndDispose();
}