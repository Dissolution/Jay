using System;
using System.Diagnostics.CodeAnalysis;

namespace Jay.Text
{
    public partial class TextBuilder
    {
        public delegate void BuildText([NotNull] TextBuilder textBuilder);

        public delegate void StateBuildText<TState>([NotNull] TextBuilder textBuilder, TState state);

        public delegate void TextBuildText([NotNull] TextBuilder textBuilder, ReadOnlySpan<char> text);

        public delegate void SpanBuildText<T>([NotNull] TextBuilder textBuilder, ReadOnlySpan<T> readOnlySpan);
    }
}