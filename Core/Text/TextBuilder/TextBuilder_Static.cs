using System;
using System.Buffers;
using System.Diagnostics.CodeAnalysis;

namespace Jay.Text
{
    public delegate void TextStateBuildText(TextBuilder builder, ReadOnlySpan<char> text);

    public delegate void BuildText([NotNull] TextBuilder builder);
    public delegate void BuildText<in TState>([NotNull] TextBuilder builder, [AllowNull, MaybeNull] TState state);
    
    public sealed partial class TextBuilder
    {
       

        private const int DefaultCapacity = 1024;
        // Keep them out of the Large Object Heap
        private static readonly ArrayPool<char> _charArrayPool = ArrayPool<char>.Create(85_000 / sizeof(char), 50);

        static TextBuilder()
        {
            
        }

        public static string Build(BuildText buildText)
        {
            using (var builder = new TextBuilder())
            {
                buildText(builder);
                return builder.ToString();
            }
        }
        
        public static string Build<TState>([AllowNull] TState state, BuildText<TState> buildText)
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