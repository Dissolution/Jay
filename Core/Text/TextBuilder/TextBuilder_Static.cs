using System;
using System.Buffers;
using System.Diagnostics.CodeAnalysis;

namespace Jay.Text
{
    public sealed partial class TextBuilder
    {
        public static implicit operator string(TextBuilder? textBuilder)
        {
            if (textBuilder is null)
                return string.Empty;
            return new string(textBuilder._characters, 0, textBuilder._length);
        }
        
        private const int DefaultCapacity = 1024;

        internal static readonly ArrayPool<char> CharArrayPool;

        static TextBuilder()
        {
            // The Large Object Heap is 85,000 bytes, we want to stay out of that
            const int maxBytes = 85_000 / sizeof(char);
            CharArrayPool = ArrayPool<char>.Create(maxBytes, 50); // 50 is the default
        }

        public static string Build(BuildText buildText)
        {
            using (var builder = new TextBuilder())
            {
                buildText(builder);
                return builder.ToString();
            }
        }
        
        public static string Build<TState>(TState state, StateBuildText<TState> stateBuildText)
        {
            using (var builder = new TextBuilder())
            {
                stateBuildText(builder, state);
                return builder.ToString();
            }
        }

        public static string Build<T>(ReadOnlySpan<T> readOnlySpan, SpanBuildText<T> spanBuildText)
        {
            using (var builder = new TextBuilder())
            {
                spanBuildText(builder, readOnlySpan);
                return builder.ToString();
            }
        }

        public static TextBuilder Rent() => new TextBuilder();

        public static string Start(ReadOnlySpan<char> text, BuildText buildText)
        {
            using (var builder = new TextBuilder())
            {
                builder.Write(text);
                buildText(builder);
                return builder.ToString();
            }
        }
    }
}