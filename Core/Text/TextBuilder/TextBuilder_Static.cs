using System;
using System.Buffers;

namespace Jay.Text
{
    public sealed partial class TextBuilder
    {
        public delegate void TextStateBuildText(TextBuilder builder, ReadOnlySpan<char> text);
        
        private const int DefaultCapacity = 1024;
        // Keep them out of the Large Object Heap
        private static readonly ArrayPool<char> _charArrayPool = ArrayPool<char>.Create(85_000 / sizeof(char), 50);

        static TextBuilder()
        {
            
        }

        public static string Build(Action<TextBuilder> buildText)
        {
            using (var builder = new TextBuilder())
            {
                buildText(builder);
                return builder.ToString();
            }
        }
        
        public static string Build<TState>(TState state, Action<TextBuilder, TState> buildText)
        {
            using (var builder = new TextBuilder())
            {
                buildText(builder, state);
                return builder.ToString();
            }
        }
        
        public static string Build(ReadOnlySpan<char> text, TextStateBuildText buildText)
        {
            using (var builder = new TextBuilder())
            {
                buildText(builder, text);
                return builder.ToString();
            }
        }

        public static TextBuilder Rent() => new TextBuilder();
    }
}