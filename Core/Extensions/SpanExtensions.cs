using System;

namespace Jay
{
    public static class SpanExtensions
    {
        //public static Span<T> Slice<T>(this Span<T> span, int index) => ((Span<T>) array).Slice(index);
        public static Span<T> Slice<T>(this Span<T> span, Index index) => span[Range.StartAt(index)];
        //public static Span<T> Slice<T>(this Span<T> span, int index, int length) => ((Span<T>) array).Slice(index, length);
        public static Span<T> Slice<T>(this Span<T> span, Range range) => span[range];

    }
}