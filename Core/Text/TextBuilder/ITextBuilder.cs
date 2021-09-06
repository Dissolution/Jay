using System;
using System.Collections.Generic;

namespace Jay.Text
{
    public interface ITextBuilder<out TBuilder> : ITextWriter<TBuilder>,
                                                  // IList<char>, IReadOnlyList<char>,
                                                  // ICollection<char>, IReadOnlyCollection<char>,
                                                  // IEnumerable<char>,
                                                  IEquatable<string>, IEquatable<char[]>, 
                                                  IDisposable
        where TBuilder : class, ITextBuilder<TBuilder>, new()
    {
        ref char this[int index] { get; }

        Span<char> this[Range range] { get; }

        /// <summary>
        /// Gets the total number of characters that have been written
        /// </summary>
        int Length { get; }
        
        TBuilder Trim();
        
        TBuilder TrimStart();
        TBuilder TrimStart(char character);
        TBuilder TrimStart(ReadOnlySpan<char> text, 
                           StringComparison comparison = StringComparison.CurrentCulture);
        TBuilder TrimStart(Func<char, bool> isTrimChar);
        
        TBuilder TrimEnd();
        TBuilder TrimEnd(char character);
        TBuilder TrimEnd(ReadOnlySpan<char> text, 
                         StringComparison comparison = StringComparison.CurrentCulture);
        TBuilder TrimEnd(Func<char, bool> isTrimChar);
        
        TBuilder Terminate(ReadOnlySpan<char> text,
                           StringComparison comparison = StringComparison.CurrentCulture);
        
        TBuilder Clear();
        
        TBuilder Transform(Func<char, char> transform);
        TBuilder Transform(Func<char, int, char> transform);
        TBuilder Transform(RefAction<char> transform);
    }


}